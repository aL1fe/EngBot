using NLog;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegaEngBot.Handlers;

public class MainErrorHandler
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public async Task HandleError(
        Exception exception,
        ITelegramBotClient botClient,
        Message message)
    {
        switch (exception)
        {
            case (Microsoft.Data.SqlClient.SqlException sqlException):
                _logger.Error($"Database is unavailable.");
                await botClient.SendTextMessageAsync(message.Chat.Id,
                    "<strong>Sorry. Database is unavailable. Please try again later.</strong>", ParseMode.Html);
                break;
            default:
                Console.WriteLine(exception.Message);
                break;
        }
    }
}