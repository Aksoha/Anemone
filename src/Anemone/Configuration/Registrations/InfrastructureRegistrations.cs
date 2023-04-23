using Anemone.Core.Persistence;
using Anemone.Infrastructure;
using Anemone.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Prism.Ioc;

namespace Anemone.Configuration.Registrations;

internal static class InfrastructureRegistrations
{
    public static void Register(IContainerRegistry container, ApplicationArguments arguments,
        ILogger<RegistrationsFacade> logger)
    {
        var serviceCollection = new ServiceCollection();
        
        serviceCollection.AddInfrastructure(new RepositoryOptions { ConnectionString = ApplicationInfo.ConnectionString });

        RegisterServices(container, logger, serviceCollection);
        UseServices(serviceCollection);
    }

    
    private static void RegisterServices(IContainerRegistry container, ILogger<RegistrationsFacade> logger, ServiceCollection serviceCollection)
    {
        foreach (var service in serviceCollection)
        {
            ServiceConverter.RegisterAsPrismService(container, service);
            logger.LogDebug("registered {Lifetime} service {ServiceType} as {RegistrationType}", service.Lifetime,
                service.ServiceType, service.ImplementationType?.ToString() ?? "factory method");
        }
    }
    
    private static void UseServices(IServiceCollection serviceCollection)
    {
        var serviceProvider = serviceCollection.BuildServiceProvider();
        serviceProvider.UseInfrastructure();
    }
}