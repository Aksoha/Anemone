#if DEBUG
#define AttachConsole
#endif

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Threading;
using Anemone.Core;
using Anemone.Core.Components;
using Anemone.Core.ViewModels;
using Anemone.DataImport;
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
    private List<LoggingQueueItem>? _loggerQueue = new();

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

        RegisterDebuggingSettings(containerRegistry);
        containerRegistry.RegisterDialog<ChangeNameDialog, ChangeNameDialogViewModel>();
        containerRegistry.RegisterDialogWindow<DialogWindow>();
        containerRegistry.RegisterSingleton<INavigationRegistrations, NavigationRegistrations>();
        containerRegistry.RegisterSingleton<ISnackbarMessageQueue, SnackbarMessageQueue>();
        containerRegistry.RegisterSingleton<IToastService, ToastService>();
        containerRegistry.RegisterSingleton<IApplicationCommands, ApplicationCommands>();
        containerRegistry.Register<IOpenFileDialog, OpenFileDialog>();
        containerRegistry.Register<IDialogService, PrismDialogWrapper>();
        containerRegistry.Register<IFile, FileWrapper>();
        containerRegistry.Register<IProcess, ProcessWrapper>();
    }

    protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
    {
        moduleCatalog.AddModule<DataImportModule>();
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
        foreach (var item in _loggerQueue)
            // ReSharper disable once TemplateIsNotCompileTimeConstantProblem
            Log.Logger.Write(item.LogEventLevel, item.Exception, item.Message);

        _loggerQueue = null;
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        _loggerQueue!.Add(new LoggingQueueItem
            { LogEventLevel = LogEventLevel.Information, Message = "starting application" });

        AllocConsole();
        var configuration = CreateConfiguration();
        ConfigureLogger(configuration);

        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        
        base.OnStartup(e);
    }

    private void UnhandledExceptionHandler(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        var ex = e.Exception;
        Log.Logger.Error(ex, "application terminated due to unhandled exception");
        e.Handled = true;

#if AttachConsole
        Console.WriteLine("press any key to close");
        Console.ReadKey();
#endif
        Current.Shutdown(1);
    }

    protected override void OnExit(ExitEventArgs e)
    {
        Log.Logger.Information("stopping application");
        SaveDebuggingSettings();
        FreeConsole();
        base.OnExit(e);
    }


    [Conditional("AttachConsole")]
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


    [Conditional("AttachConsole")]
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
    [Conditional("AttachConsole")]
    private static extern void AllocConsole();

    [DllImport("Kernel32")]
    [Conditional("AttachConsole")]
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