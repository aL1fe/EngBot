using NLog;
using TelegaEngBot.AppConfigurations;
using TelegaEngBot.DataAccessLayer;
using TelegaEngBot.Handlers;
using TelegaEngBot.Identity;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegaEngBot;

class Program
{
    private static AppDbContext _dbContext;
    private static bool _isSmileOn = false;
    private static Logger _logger = LogManager.GetCurrentClassLogger();

    static async Task Main(string[] args)
    {
        _dbContext = new AppDbContext();
        if (!_dbContext.Dictionary.Any())
        {
            Console.WriteLine("Database is empty.");
            return;
        }

        // TelegramBot init
        var botClient = new TelegramBotClient(AppConfig.BotToken);
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

        Console.ReadLine();
        _logger.Info("Stop program.");
    }

    private static async Task HandleUpdate(ITelegramBotClient botClient, Update update, CancellationToken ct)
    {
        if (update.Type == UpdateType.Message && update?.Message?.Text != null)
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

        switch (message.Text)
        {
            case "/start":
                await MessageHandler.Start(botClient, message, _dbContext);
                break;
            case "Know":
                await MessageHandler.Know(botClient, message, _dbContext, _isSmileOn);
                break;
            case "Don't know":
                await MessageHandler.NotKnow(botClient, message, _dbContext, _isSmileOn);
                break;
            case "Pronunciation":
                await MessageHandler.Pron(botClient, message);
                break;
            case "/smile":
                _isSmileOn = !_isSmileOn;
                break;
            case "/pronunciation":
                MessageHandler.IsPronunciationOn = !MessageHandler.IsPronunciationOn;
                break;
        }
    }
}
//https://t.me/my_aL1fe_bot
//https://t.me/PhrasesAndWords_bot