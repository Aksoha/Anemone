using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using Anemone.Algorithms;
using Anemone.Core;
using Anemone.Core.Components;
using Anemone.Core.ViewModels;
using Anemone.DataImport;
using Anemone.Repository;
using Anemone.Services;
using Anemone.Settings;
using Anemone.Startup;
using Anemone.Views;
using Holize.PersistenceFramework;
using Holize.PersistenceFramework.Extensions.Prism;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Unity;
using Serilog;
using Serilog.Events;
using Unity;
using Unity.Microsoft.DependencyInjection;
using Rect = Anemone.Startup.Rect;

namespace Anemone;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App
{
    /// <summary>
    ///     a queue of logging data used for storing logging messages before logger is created
    /// </summary>
    private Queue<LoggingQueueItem> _loggerQueue = new();

    private ApplicationArguments _arguments = default!;

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        var persistenceOptions = new PersistenceOptions();

#if DEBUG
        persistenceOptions.LocalFilesDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
#endif

        containerRegistry.RegisterInstance(persistenceOptions);
        containerRegistry.RegisterSettings(new ShellSettings
        {
            Top = 0,
            Left = 0,
            Height = 500,
            Width = 800,
            WindowState = WindowState.Maximized,
            NavigationDrawerExpanded = true
        });

        if(_arguments.AttachDebugger)
            RegisterDebuggingSettings(containerRegistry);
        
        containerRegistry.RegisterDialog<TextBoxDialog, TextBoxDialogViewModel>();
        containerRegistry.RegisterDialog<ConfirmationDialog, ConfirmationDialogViewModel>();
        containerRegistry.RegisterDialogWindow<DialogWindow>();
        containerRegistry.RegisterSingleton<INavigationRegistrations, NavigationRegistrations>();
        containerRegistry.RegisterSingleton<ISnackbarMessageQueue, SnackbarMessageQueue>();
        containerRegistry.RegisterSingleton<IToastService, ToastService>();
        containerRegistry.RegisterSingleton<IApplicationCommands, ApplicationCommands>();
        containerRegistry.Register<IOpenFileDialog, OpenFileDialog>();
        containerRegistry.Register<IDialogService, PrismDialogWrapper>();
        containerRegistry.Register<IFile, FileWrapper>();
        containerRegistry.Register<IDirectory, DirectoryWrapper>();
        containerRegistry.Register<IProcess, ProcessWrapper>();


        var serviceCollection = new ServiceCollection();
        containerRegistry.RegisterInstance<IServiceCollection>(serviceCollection);

        Container.Resolve<IServiceCollection>().ConfigureDatabase(new RepositoryOptions
            { ConnectionString = ApplicationInfo.ConnectionString });

        foreach (var service in serviceCollection)
            Register(containerRegistry, service);

        var serviceProvider = serviceCollection.BuildServiceProvider();


        containerRegistry.RegisterInstance<IServiceProvider>(serviceProvider);
        Container.Resolve<IServiceProvider>().UseDatabase();
    }

    /// <summary>
    /// Registers a service from <see cref="IServiceCollection"/> in the <paramref name="containerRegistry"/>.
    /// </summary>
    /// <param name="containerRegistry">The container.</param>
    /// <param name="service">The service to register.</param>
    private static void Register(IContainerRegistry containerRegistry, ServiceDescriptor service)
    {
        if (service.ImplementationInstance is not null)
        {
            containerRegistry.RegisterInstance(service.ServiceType, service.ImplementationInstance);
        }
        else if (service.ImplementationFactory is not null)
        {
            var factory = service.ImplementationFactory;
            Func<IContainerProvider, object> factoryMethod = containerProvider =>
            {
                var provider = containerProvider.Resolve<IServiceProvider>();
                return factory(provider);
            };
            containerRegistry.Register(service.ServiceType, factoryMethod);
        }
        else
        {
            containerRegistry.Register(service.ServiceType, service.ImplementationType);
        }
    }

    protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
    {
        moduleCatalog.AddModule<DataImportModule>();
        moduleCatalog.AddModule<AlgorithmsModule>();
    }

    protected override Window CreateShell()
    {
        Log.Logger.Debug("creating shell");
        var w = Container.Resolve<ShellView>();
        return w;
    }

    protected override IContainerExtension CreateContainerExtension()
    {
        var container = new UnityContainer();
        IServiceCollection serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));
        container.BuildServiceProvider(serviceCollection);
        return new UnityContainerExtension(container);
    }

    private static IConfiguration CreateConfiguration()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile("secrets.json", true, true)
            .AddUserSecrets<App>()
            .Build();
        return configuration;
    }

    private void ConfigureLogger(IConfiguration configuration)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();


        ArgumentNullException.ThrowIfNull(_loggerQueue);
        while (_loggerQueue.Count > 0)
        {
            var item = _loggerQueue.Dequeue();
            // ReSharper disable once TemplateIsNotCompileTimeConstantProblem
            Log.Logger.Write(item.LogEventLevel, item.Exception, item.Message);
        }
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        _arguments = ApplicationArguments.Parse(e);

        _loggerQueue.Enqueue(new LoggingQueueItem
            { LogEventLevel = LogEventLevel.Information, Message = "starting application" });

        if (_arguments.AttachDebugger)
            AllocConsole();
        var configuration = CreateConfiguration();
        ConfigureLogger(configuration);

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        base.OnStartup(e);
    }

    private void UnhandledExceptionHandler(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        var ex = e.Exception;
        Log.Logger.Error(ex, "application terminated due to unhandled exception");
        e.Handled = true;

        if (_arguments.AttachDebugger)
        {
            Console.WriteLine("press any key to close");
            Console.ReadKey();
        }

        Current.Shutdown(1);
    }

    protected override void OnExit(ExitEventArgs e)
    {
        Log.Logger.Information("stopping application");
        SaveDebuggingSettings();
        
        if(_arguments.AttachDebugger)
            FreeConsole();
        base.OnExit(e);
    }


    private void SaveDebuggingSettings()
    {
        var consoleSettings = Container.Resolve<DebuggingConsoleSettings>();
        var consolePtr = GetConsoleWindow();
        GetWindowRect(new HandleRef(this, consolePtr), out var rect);
        consoleSettings.X = rect.Left;
        consoleSettings.Y = rect.Top;
        consoleSettings.Cx = rect.Right - rect.Left;
        consoleSettings.Cy = rect.Bottom - rect.Top;
    }


    private void RegisterDebuggingSettings(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterSettings(new DebuggingConsoleSettings
        {
            X = 0,
            Y = 0,
            Cx = 800,
            Cy = 500
        });
        var container = containerRegistry.GetContainer();
        var settings = container.Resolve<DebuggingConsoleSettings>();

        var consolePtr = GetConsoleWindow();
        SetWindowPos(consolePtr, nint.Zero, settings.X, settings.Y, settings.Cx, settings.Cy, 0x0004);
    }

    [DllImport("Kernel32")]
    private static extern void AllocConsole();

    [DllImport("Kernel32")]
    private static extern void FreeConsole();

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetWindowPos(nint hWnd, nint hWndInsertAfter, int x, int y, int cx, int cy, int wFlags);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetWindowRect(HandleRef hWnd, out Rect lpRect);

    [DllImport("Kernel32")]
    private static extern nint GetConsoleWindow();
}