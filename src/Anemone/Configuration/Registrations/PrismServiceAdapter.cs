using System;
using Microsoft.Extensions.DependencyInjection;
using Prism.Ioc;

namespace Anemone.Configuration.Registrations;

public static class ServiceConverter
{
    public static void RegisterAsPrismService(IContainerRegistry containerRegistry, ServiceDescriptor service)
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
            RegisterType(containerRegistry, service, factoryMethod);
        }
        else
        {
            RegisterType(containerRegistry, service);
        }
    }
    
    private static void RegisterType(IContainerRegistry containerRegistry, ServiceDescriptor service,
        Func<IContainerProvider, object> factoryMethod)
    {
        switch (service.Lifetime)
        {
            case ServiceLifetime.Singleton:
                containerRegistry.RegisterSingleton(service.ServiceType, factoryMethod);
                break;
            case ServiceLifetime.Scoped:
                containerRegistry.RegisterScoped(service.ServiceType, factoryMethod);
                break;
            case ServiceLifetime.Transient:
                containerRegistry.Register(service.ServiceType, factoryMethod);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }


    private static void RegisterType(IContainerRegistry containerRegistry, ServiceDescriptor service)
    {
        switch (service.Lifetime)
        {
            case ServiceLifetime.Singleton:
                containerRegistry.RegisterSingleton(service.ServiceType, service.ImplementationType);
                break;
            case ServiceLifetime.Scoped:
                containerRegistry.RegisterScoped(service.ServiceType, service.ImplementationType);
                break;
            case ServiceLifetime.Transient:
                containerRegistry.Register(service.ServiceType, service.ImplementationType);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}