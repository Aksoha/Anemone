using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Anemone.Configuration;

public static class LoggerConfiguration
{
    public static void Configure(IConfiguration configuration)
    {
        Log.Logger = new Serilog.LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();
    }

    public static void AddLogging(IServiceCollection serviceCollection)
    {
        serviceCollection.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));
    }
}