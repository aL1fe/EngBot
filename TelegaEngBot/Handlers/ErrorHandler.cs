using NLog;
using Telegram.Bot;
using Telegram.Bot.Exceptions;

namespace TelegaEngBot.Handlers;

internal static class ErrorHandler
{
    static Logger logger = LogManager.GetCurrentClassLogger();
    internal static Task HandleError(ITelegramBotClient client, Exception exception, CancellationToken ct)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException =>
                $"Telegram API error: {apiRequestException.ErrorCode} - {apiRequestException.Message}",
            HttpRequestException httpException =>
                $"HTTP request error: {httpException.Message}",
            System.Net.Sockets.SocketException socketException =>
                $"Network error: {socketException.Message}",
            _ => exception.ToString()
        };
        logger.Fatal(errorMessage);
        Console.WriteLine(errorMessage);
        return Task.CompletedTask;
    }
}