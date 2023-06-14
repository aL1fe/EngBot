using Microsoft.Extensions.Configuration;

namespace TelegaEngBot.AppConfigurations;

public static class AppConfig
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
    
    //From user secrets
    public static string BotToken => _configuration.GetSection("BotToken")["TestBot"];
    public static string OpenAIToken => _configuration["OpenAIToken"];
    
    //From appsettings.json
    public static string ConnectionString => _configuration.GetConnectionString("DefaultConnection");
    public static string PronunciationFolderPath => _configuration["PronunciationFolderPath"];
    public static string NeuralModelHost => _configuration["NeuralModelHost"];
}