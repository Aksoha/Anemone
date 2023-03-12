using System;
using Anemone.Repository.HeatingSystemData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Anemone.Repository;

public static class Configuration
{
    public static void ConfigureDatabase(this IServiceCollection serviceCollection, RepositoryOptions options)
    {
        AddRepositoryServices(serviceCollection, options);
        AddEntityFramework(serviceCollection, options);
    }

    public static void UseDatabase(this IServiceProvider serviceProvider)
    {
        var heatingSystemContext = serviceProvider.GetService<HeatingSystemContext>()!;
        heatingSystemContext.Database.Migrate();
    }


    private static void AddRepositoryServices(IServiceCollection serviceCollection, RepositoryOptions options)
    {
        serviceCollection.AddScoped<IDbConnectionFactory, DbConnectionFactory>();
        serviceCollection.AddSingleton(options);
        
        serviceCollection.AddScoped<IHeatingSystemRepository, HeatingSystemSqliteRepository>();
        serviceCollection.AddScoped<IRepository<HeatingSystem>>(x => x.GetService<IHeatingSystemRepository>()!);
    }

    private static void AddEntityFramework(IServiceCollection serviceCollection, RepositoryOptions options)
    {
        serviceCollection.AddDbContext<HeatingSystemContext>(opts => opts.UseSqlite(options.ConnectionString));
    }
}