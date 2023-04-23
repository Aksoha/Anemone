using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Threading;
using Anemone.Configuration;
using Anemone.Configuration.Registrations;
using Anemone.Models;
using Anemone.Settings;
using Anemone.UI.Calculation;
using Anemone.UI.DataImport;
using Anemone.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Unity;
using Serilog;
using Serilog.Events;
using Unity;
using Unity.Microsoft.DependencyInjection;
using LoggerConfiguration = Anemone.Configuration.LoggerConfiguration;

namespace Anemone;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App
{
    private ApplicationArguments _arguments = default!;

    /// <summary>
    ///     a queue of logging data used for storing logging messages before logger is created
    /// </summary>
    private readonly Queue<LoggingQueueItem> _loggerQueue = new();


    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        new RegistrationsFacade(containerRegistry, _arguments, Container.Resolve<ILogger<RegistrationsFacade>>())
            .RegisterAllServices();
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
        LoggerConfiguration.AddLogging(serviceCollection);
        container.BuildServiceProvider(serviceCollection);
        return new UnityContainerExtension(container);
    }


    protected override void OnStartup(StartupEventArgs e)
    {
        _arguments = ApplicationArguments.Parse(e);

        _loggerQueue.Enqueue(new LoggingQueueItem
            { LogEventLevel = LogEventLevel.Information, Message = "starting application" });

        if (_arguments.AttachDebugger)
            ConsoleInterop.AllocConsole();

        var config = AppSettingsConfiguration.Configure();
        LoggerConfiguration.Configure(config);


        while (_loggerQueue.Count > 0)
        {
            var item = _loggerQueue.Dequeue();
            // ReSharper disable once TemplateIsNotCompileTimeConstantProblem
            Log.Logger.Write(item.LogEventLevel, item.Exception, item.Message);
        }

        base.OnStartup(e);
    }

    private void UnhandledExceptionHandler(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        var ex = e.Exception;
        Log.Logger.Error(ex, "application terminated due to unhandled exception");
        e.Handled = true;

        if (_arguments.AttachDebugger)
        {
            System.Console.WriteLine("press any key to close");
            System.Console.ReadKey();
        }

        Current.Shutdown(1);
    }

    protected override void OnExit(ExitEventArgs e)
    {
        Log.Logger.Information("stopping application");

        if (_arguments.AttachDebugger)
        {
            SaveDebuggingSettings();
            ConsoleInterop.FreeConsole();
        }

        base.OnExit(e);
    }

    private void SaveDebuggingSettings()
    {
        var consoleSettings = Container.Resolve<DebuggingConsoleSettings>();
        var consolePtr = ConsoleInterop.GetConsoleWindow();
        ConsoleInterop.GetWindowRect(new HandleRef(this, consolePtr), out var rect);
        consoleSettings.X = rect.Left;
        consoleSettings.Y = rect.Top;
        consoleSettings.Cx = rect.Right - rect.Left;
        consoleSettings.Cy = rect.Bottom - rect.Top;
    }
}