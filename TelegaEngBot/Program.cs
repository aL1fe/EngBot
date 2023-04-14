using Microsoft.EntityFrameworkCore;
using NLog;
using TelegaEngBot.AppConfigurations;
using TelegaEngBot.DataAccessLayer;
using TelegaEngBot.Handlers;
using TelegaEngBot.Identity;
using TelegaEngBot.Services;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegaEngBot;

class Program
{
    private static AppDbContext _dbContext;
    private static Logger _logger = LogManager.GetCurrentClassLogger();

    static async Task Main()
    {
        _dbContext = new AppDbContext();
        if (!_dbContext.CommonVocabulary.Any())
        {
            Console.WriteLine("Database is empty.");
            //Seeder.Seed(_dbContext);
            return;
        }

        // TelegramBot init
        var botToken = AppConfig.BotToken;
        if (botToken != null)
        {
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
            _logger.Info("Start listening for @" + me.Username + ".");
            Console.WriteLine("Start listening for @" + me.Username);
        }
        else
        {
            _logger.Fatal("Bot token not found.");
        }

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
        _logger.Info("Stop program.");
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
        //Check authorization
        if (!IdentityServer.CheckAuth(message.From.Id))
        {
            await botClient.SendTextMessageAsync(message.Chat.Id, "*Access denied.*", ParseMode.Markdown);
            _logger.Info("New user tried to connect. User Id: " + message.From.Id
                                                                + " Username: " + message.From.Username
                                                                + " FirstName: " + message.From.FirstName
                                                                + " LastName: " + message.From.LastName);
            return;
        }

        //var commonVac = _dbContext.CommonVocabulary;
        var userList = _dbContext.UserList
            .Where(x => x.TelegramUserId == message.From.Id)
            .Include(x => x.UserVocabulary)!
                .ThenInclude(y => y.Article)
            .Include(x => x.UserSettings);
        var user = userList.FirstOrDefault(x => x.TelegramUserId == message.From.Id);

        if (user == null) // If null Create User in database
        {
            var userService = new UserService(_dbContext);
            user = userService.CreateUser(message.From.Id, message);
            message.Text = "/start";
        }

        switch (message.Text)
        {
            case "/start":
                await MessageHandler.Start(botClient, message, _dbContext, user);
                break;
            case "Know":
                await MessageHandler.Know(botClient, message, _dbContext, user);
                break;
            case "Don't know":
                await MessageHandler.NotKnow(botClient, message, _dbContext, user);
                break;
            case "Pronunciation":
                await MessageHandler.Pron(botClient, message);
                break;
            case "/smile":
                user.UserSettings.IsSmileOn = !user.UserSettings.IsSmileOn;
                break;
            case "/pronunciation":
                MessageHandler.IsPronunciationOn = !MessageHandler.IsPronunciationOn; //todo
                await MessageHandler.RedrawKeyboard(botClient, message, false);
                break;
        }
    }
}
//https://t.me/my_aL1fe_bot
//https://t.me/PhrasesAndWords_bot