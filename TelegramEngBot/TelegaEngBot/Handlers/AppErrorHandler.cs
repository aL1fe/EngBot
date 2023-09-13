using NLog;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegaEngBot.Handlers;

public class AppErrorHandler
{
    private Exception _exception;
    private ITelegramBotClient _botClient;
    private Message _message;

    public AppErrorHandler(
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
            case Microsoft.Data.SqlClient.SqlException sqlException:
                _logger.Error("Database is unavailable");
                await _botClient.SendTextMessageAsync(_message.Chat.Id,
                    "<strong>Sorry. Database is unavailable. Please try again later.</strong>", ParseMode.Html);
                break;
            case ApiRequestException apiException
                when (apiException.Message.Contains("Forbidden: bot was blocked by the user")):
                _logger.Error("Forbidden: bot was blocked by the user");
                Console.WriteLine(_exception.Message);
                break;
            case System.Security.Authentication.AuthenticationException authException:
                _logger.Error($"OpenAI authentication error: {authException.Message}");
                Console.WriteLine($"OpenAI authentication error: {authException.Message}");
                break;
            default:
                Console.WriteLine(_exception.Message);
                break;
        }
    }
}