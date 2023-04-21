using System.IO.Abstractions;
using Anemone.Core.Common.Entities;
using Anemone.Core.EnergyMatching;
using Anemone.Core.Export;
using Anemone.Core.Import;
using Anemone.Core.Persistence;
using Anemone.Core.Persistence.HeatingSystem;
using Anemone.Core.Process;
using Anemone.Core.ReportGenerator;
using Anemone.Infrastructure.EnergyMatching;
using Anemone.Infrastructure.EnergyMatching.Builders;
using Anemone.Infrastructure.Export;
using Anemone.Infrastructure.Import;
using Anemone.Infrastructure.Persistence;
using Anemone.Infrastructure.Persistence.HeatingSystemData;
using Anemone.Infrastructure.Process;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Anemone.Infrastructure;

public static class Configuration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection serviceCollection, RepositoryOptions options)
    {
        serviceCollection.AddTransient<ILlcMatchingBuilder, LlcMatchingBuilder>();
        serviceCollection.AddTransient<ILlcMatchingCalculator, LlcMatchingCalculator>();

        serviceCollection.AddTransient<IDataExporter, DataExporter>();
        serviceCollection.AddTransient<IReportGenerator, ReportGenerator>();

        serviceCollection.AddTransient<IProcess, ProcessWrapper>();

        serviceCollection.AddTransient<IFileReader, FileReader>();
        
        serviceCollection.AddSingleton<IFile, FileWrapper>();
        serviceCollection.AddSingleton<IDirectory, DirectoryWrapper>();
        
        serviceCollection.AddScoped<IDbConnectionFactory, DbConnectionFactory>();
        serviceCollection.AddSingleton(options);
        
        serviceCollection.AddScoped<IHeatingSystemRepository, HeatingSystemSqliteRepository>();
        serviceCollection.AddScoped<IRepository<HeatingSystem>>(x => x.GetService<IHeatingSystemRepository>()!);
        
        AddEntityFramework(serviceCollection, options);
        
        return serviceCollection;
    }
    

    public static void UseInfrastructure(this IServiceProvider serviceProvider)
    {
        var heatingSystemContext = serviceProvider.GetService<HeatingSystemContext>()!;
        heatingSystemContext.Database.Migrate();
    }

    

    private static void AddEntityFramework(IServiceCollection serviceCollection, RepositoryOptions options)
    {
        serviceCollection.AddDbContext<HeatingSystemContext>(opts => opts.UseSqlite(options.ConnectionString));
    }
    
    
}