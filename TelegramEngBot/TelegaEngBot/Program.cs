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
        if (AppConfig.Env == "Production") 
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
            Console.WriteLine($"Bot token = ******{botToken.Substring(botToken.Length - 5)}");

            var botClient = new TelegramBotClient(botToken);
            
            using var cts = new CancellationTokenSource();
            var receiverOptions = new ReceiverOptions() {AllowedUpdates = { }};
            botClient.StartReceiving(
                HandleUpdate,
                ErrorHandler.HandleError,
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
            await HandleMessage(botClient, update.Message);
    }

    private static async Task HandleMessage(ITelegramBotClient botClient, Message message)
    {
        // Identity
        var identity = new IdentityServer(message.From.Id);
        if (!identity.CheckAuth())
        {
            await botClient.SendTextMessageAsync(message.Chat.Id, 
                "*Access denied.* You should request access and then restart bot using the command //start", 
                ParseMode.Markdown);
            _logger.Warn(
                $"New user tried to connect. User Id: {message.From.Id}; Username: {message.From.Username}; FirstName: {message.From.FirstName}; LastName: {message.From.LastName}");
            return;
        }
        
        try
        {
            var user = _dbContext.UserList.FirstOrDefault(x => x.TelegramUserId == message.From.Id);
            var userService = new UserService(_dbContext, botClient, message);
            
            // Check if user exist
            if (user == null)
            {
                await userService.CreateUser();
                return;
            }

            //todo make check if last message is processed
            //check unhandled updates (messages)
            // var updates = await botClient.GetUpdatesAsync();
            // if (updates.Any(x => x.Message.Chat.Id == message.Chat.Id)) return;

            var messageHandler = new MessageHandler(botClient, message, _dbContext, user);
            if (user.UserSettings.DifficultyLevel == null || !user.UserVocabulary.Any())
            {
                try
                {
                    var level = (Level)Enum.Parse(typeof(Level), message.Text);
                    user.UserSettings.DifficultyLevel = level;
                    await _dbContext.SaveChangesAsync();
                    await userService.FillUserVocabularyAndShowNewArticle(user);
                }
                catch (ArgumentException)
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Please choose difficulty level.");
                    return;
                }
                message.Text = "/start";
            }

            if (user.UserVocabulary.Average(x => x.Weight) < 5 
                && user.UserVocabulary.Count != _dbContext.CommonVocabulary.Count()) // UserVocabulary has all the articles from CommonVocabulary
            {
                await userService.FillUserVocabularyAndShowNewArticle(user);
            }
            
            switch (message.Text)
            {
                case "/start":
                    await messageHandler.Start();
                    break;
                case "Know":
                    await messageHandler.Know();
                    break;
                case "Don't know":
                    await messageHandler.NotKnow();
                    break;
                case "Pronunciation":
                    messageHandler.TextToSpeech();
                    break;
                case "/smile":
                    user.UserSettings.IsSmileOn = !user.UserSettings.IsSmileOn;
                    await _dbContext.SaveChangesAsync();
                    await botClient.SendTextMessageAsync(message.Chat.Id,
                        $"Smiles is {(user.UserSettings.IsSmileOn ? "On" : "Off")}");
                    break;
                case "/pronunciation":
                    user.UserSettings.IsPronunciationOn = !user.UserSettings.IsPronunciationOn;
                    await _dbContext.SaveChangesAsync();
                    await messageHandler.RedrawKeyboard(false);
                    break;
                case "/hard":
                    await messageHandler.Hard();
                    break;
                case "/camb":
                    await messageHandler.CambridgePron();
                    break;
                case "/ex":
                    if (message.Chat.Id == 450056320 || message.Chat.Id == 438560103 || message.Chat.Id == 906180277)
                        await messageHandler.Example();
                    break;
                case "/changeLevel":
                    await userService.ChooseLanguageLevel(user);
                    break;
            }
        }
        catch (Exception exception)
        {
            var mainErrorHandler = new MainErrorHandler(exception, botClient, message);
            await mainErrorHandler.HandleError();
        }
    }
}

// https://t.me/my_aL1fe_bot
// https://t.me/PhrasesAndWords_bot

// Menu
/*
start - Restart
smile - Smile On/Off
pronunciation - Pronunciation On/Off
hard - Show 10 hard-to-remember words
*/