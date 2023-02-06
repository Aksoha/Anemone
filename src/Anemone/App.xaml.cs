#if DEBUG
#define AttachConsole
#endif

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using Anemone.Startup;
using Anemone.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Prism.Ioc;
using Prism.Unity;
using Serilog;
using Serilog.Events;
using Unity;
using Unity.Microsoft.DependencyInjection;

namespace Anemone;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    /// <summary>
    ///     a queue of logging data used for storing logging messages before logger is created
    /// </summary>
    private List<LoggingQueueItem>? _loggerQueue = new();
    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
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

        base.OnStartup(e);
    }
        
    protected override void OnExit(ExitEventArgs e)
    {
        Log.Logger.Information("stopping application");
        FreeConsole();
        base.OnExit(e);
    }
    
    [DllImport("Kernel32")]
    [Conditional("AttachConsole")]
    private static extern void AllocConsole();

    [DllImport("Kernel32")]
    [Conditional("AttachConsole")]
    private static extern void FreeConsole();
}
    
