using System.IO;
using System.Reflection;
using System.Windows;
using Anemone.Models;
using Anemone.Settings;
using Holize.PersistenceFramework;
using Holize.PersistenceFramework.Extensions.Prism;
using Microsoft.Extensions.Logging;
using Prism.Ioc;
using Prism.Unity;
using Unity;

namespace Anemone.Configuration.Registrations;

internal static class SettingsRegistrations
{
    public static void Register(IContainerRegistry container, ApplicationArguments arguments, ILogger<RegistrationsFacade> logger)
    {
        var persistenceOptions = new PersistenceOptions();
#if DEBUG
        persistenceOptions.LocalFilesDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
#endif
        container.RegisterInstance(persistenceOptions);
        
        
        
        RegisterSettings(container, logger, new ShellSettings
        {
            Top = 0,
            Left = 0,
            Height = 500,
            Width = 800,
            WindowState = WindowState.Maximized
        });

        RegisterSettings(container, logger, new SidebarSettings
        {
            IsExpanded = true,
            MinWidth = 60,
            MaxWidth = 230
        });


        if (arguments.AttachDebugger)
        {
            RegisterSettings(container, logger, new DebuggingConsoleSettings
            {
                X = 0,
                Y = 0,
                Cx = 800,
                Cy = 500
            });
            var settings = container.GetContainer().Resolve<DebuggingConsoleSettings>();

            var consolePtr = ConsoleInterop.GetConsoleWindow();
            ConsoleInterop.SetWindowPos(consolePtr, nint.Zero, settings.X, settings.Y, settings.Cx, settings.Cy, 0x0004);
        }

    }
    private static void RegisterSettings<T>(IContainerRegistry container, ILogger<RegistrationsFacade> logger, T defaultValue)
        where T : Holize.PersistenceFramework.Settings
    {
        container.RegisterSettings(defaultValue);
        logger.LogDebug("registered {Type} settings", defaultValue.GetType());
    }
}