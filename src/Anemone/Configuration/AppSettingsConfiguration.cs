using System.IO;
using Microsoft.Extensions.Configuration;

namespace Anemone.Configuration;

public static class AppSettingsConfiguration
{
    public static IConfiguration Configure()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile("secrets.json", true, true)
            .AddUserSecrets<App>()
            .Build();
        return configuration;
    }
}