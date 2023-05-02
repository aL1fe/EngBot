using Microsoft.Extensions.Configuration;

namespace TelegaEngBot.AppConfigurations;

internal static class AppConfig
{
    private static IConfiguration _configuration;

    static AppConfig()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddUserSecrets<Program>();
        _configuration = builder.Build();
    }
    
    internal static string? BotToken => _configuration.GetSection("BotToken")["TestBot"];
    internal static string? ConnectionString => _configuration.GetConnectionString("DefaultConnection");
}