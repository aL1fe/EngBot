using NLog;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegaEngBot.Handlers;

public class MainErrorHandler
{
    private Exception _exception;
    private ITelegramBotClient _botClient;
    private Message _message;

    public MainErrorHandler(
        Exception exception,
        ITelegramBotClient botClient,
        Message message)
    {
        _exception = exception;
        _botClient = botClient;
        _message = message;
    }

    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public async Task HandleError()
    {
        switch (_exception)
        {
            case (Microsoft.Data.SqlClient.SqlException sqlException):
                _logger.Error($"Database is unavailable.");
                await _botClient.SendTextMessageAsync(_message.Chat.Id,
                    "<strong>Sorry. Database is unavailable. Please try again later.</strong>", ParseMode.Html);
                break;
            default:
                Console.WriteLine(_exception.Message);
                break;
        }
    }
}