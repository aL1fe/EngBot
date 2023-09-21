using Microsoft.EntityFrameworkCore;
using NLog;
using TelegaEngBot.AppConfigurations;
using TelegaEngBot.DataAccessLayer;
using TelegaEngBot.Handlers;
using TelegaEngBot.Identity;
using TelegaEngBot.Models;
using TelegaEngBot.Services;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegaEngBot;

class Program
{
    private static AppDbContext _dbContext;
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    static async Task Main()
    {
        _dbContext = new AppDbContext();
        //if (AppConfig.Env == "Production") 
            await _dbContext.Database.MigrateAsync();
        
        // Check database is not empty and available
        var check = new DatabaseService(_dbContext);
        check.CheckDatabase();

        // Check if "user vocabulary" match "common vocabulary"
        //check.MatchVocabulary();

        // TelegramBot init
        var botToken = AppConfig.BotToken;
        if (botToken != null)
        {
            Console.WriteLine($"Bot token = ******{botToken.Substring(botToken.Length - 5)}"); // Show 5 symbols of bot token

            var botClient = new TelegramBotClient(botToken);
            
            using var cts = new CancellationTokenSource();
            var receiverOptions = new ReceiverOptions() {AllowedUpdates = { }};
            botClient.StartReceiving(
                HandleUpdate,
                BotErrorHandler.HandleError,
                receiverOptions,
                cancellationToken: cts.Token
            );
            
            var me = await botClient.GetMeAsync(cancellationToken: cts.Token);
            _logger.Info($"Start listening for @{me.Username}");
            Console.WriteLine($"Start listening for @{me.Username}");
        }
        else
        {
            _logger.Fatal("Bot token not found");
            Console.WriteLine("Bot token not found");
        }

        if (AppConfig.Env == "Production") while (true) {}

        Console.WriteLine("Press \"Enter\" to exit...");
        Console.Read();
    }

    private static async Task HandleUpdate(ITelegramBotClient botClient, Update update, CancellationToken ct)
    {
        if (update.Type == UpdateType.Message
            && update.Message?.Text != null
            && update.Message.From != null)
        {
            var updateHandlerMessage = new UpdateHandler_Message(botClient, update.Message, _dbContext);
            await updateHandlerMessage.HandleMessage();
            return;
        }
        
        if (update.Type == UpdateType.CallbackQuery)
        {
            var handleCallbackQuery = new UpdateHandler_CallbackQuery(botClient, update.CallbackQuery, _dbContext);
            await handleCallbackQuery.HandleCallbackQuery();
        }
    }
}
