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
            Telegram.Bot.Exceptions.RequestException {InnerException: System.Net.Http.HttpRequestException httpException} =>
                $"Telegram HTTP request error: {httpException.Message}",
            Telegram.Bot.Exceptions.ApiRequestException apiRequestException =>
                $"Telegram API error: {apiRequestException.ErrorCode} - {apiRequestException.Message}",
            System.Net.Sockets.SocketException socketException =>
                $"Network error: {socketException.Message}",
            System.Net.Http.HttpRequestException httpException =>
                $"HTTP request error: {httpException.Message}",
            _ => exception.ToString()
        };

        logger.Warn(errorMessage);
        Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ms") + " " + errorMessage);
        return Task.CompletedTask;
    }
}