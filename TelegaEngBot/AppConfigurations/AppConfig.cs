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
            .AddEnvironmentVariables()
            .AddUserSecrets<Program>();
        _configuration = builder.Build();
    }

    public static string Env => _configuration["ENVIRONMENT"];

    public static string BotToken =>
        Env == "Production"
            ? _configuration.GetSection("BotToken")["ProdBot"]
            : _configuration.GetSection("BotToken")["TestBot"];

    public static string OpenAiToken => _configuration.GetSection("OpenAI")["OpenAIToken"];

    public static string ConnectionString =>
        Env == "Production"
            ? _configuration["CONNECTION_STRING"]
            : _configuration.GetConnectionString("TestDbDocker");

    public static string NeuralModelHost =>
        Env == "Production"
            ? _configuration["NEURAL_MODEL_HOST"]
            : _configuration["NeuralModelHost"];
    
    public static string OpenAiPromt => _configuration.GetSection("OpenAI")["OpenAIPromt"];
}