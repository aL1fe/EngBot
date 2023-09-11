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
            ? _configuration["BOT_TOKEN"]
            : _configuration.GetSection("BotToken")["TestBot"];

    public static string OpenAiToken =>
        Env == "Production"
        ? _configuration["OPEN_AI_TOKEN"]
        : _configuration.GetSection("OpenAI")["OpenAIToken"];

    public static string ConnectionString =>
        Env == "Production"
            ? _configuration["CONNECTION_STRING"]
            : _configuration.GetConnectionString("TestDbDocker");

    public static string NeuralModelHost =>
        Env == "Production"
            ? _configuration["NEURAL_MODEL_HOST"]
            : _configuration["NeuralModelHost"];
    
    public static string OpenAIPromt => _configuration.GetSection("OpenAI")["OpenAIPromt"];

    public static int Start => _configuration.GetValue<int>("Weight:Start");
    public static int KnowDecrease => _configuration.GetValue<int>("Weight:KnowDecrease");
    public static int NotKnowIncrease => _configuration.GetValue<int>("Weight:NotKnowIncrease");
    public static int AverageWeight => _configuration.GetValue<int>("Weight:AverageWeight");
}