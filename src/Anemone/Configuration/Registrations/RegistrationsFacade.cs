using Anemone.Models;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.Logging;
using Prism.Ioc;

namespace Anemone.Configuration.Registrations;

internal class RegistrationsFacade
{
    private IContainerRegistry Container { get; }
    private ApplicationArguments Arguments { get; }
    private ILogger<RegistrationsFacade> Logger { get; }

    public RegistrationsFacade(IContainerRegistry container, ApplicationArguments arguments, ILogger<RegistrationsFacade> logger)
    {
        Container = container;
        Arguments = arguments;
        Logger = logger;
    }
    public void RegisterAllServices()
    {

        SettingsRegistrations.Register(Container, Arguments, Logger);
        InfrastructureRegistrations.Register(Container, Arguments, Logger);
        UiRegistrations.Register(Container, Arguments, Logger);
        
        Container.RegisterSingleton<ISnackbarMessageQueue, SnackbarMessageQueue>();
    }
}