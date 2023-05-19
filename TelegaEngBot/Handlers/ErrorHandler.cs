using NLog;
using Telegram.Bot;
using Telegram.Bot.Exceptions;

namespace TelegaEngBot.Handlers;

internal static class ErrorHandler
{
    private static Logger _logger = LogManager.GetCurrentClassLogger();
    private static string _lastErrorMessage = string.Empty;
    private static DateTime _lastErrorTimestamp = DateTime.MinValue;

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
            System.Threading.Tasks.TaskCanceledException taskCanceledException =>
                $"The request was canceled: {taskCanceledException.Message}",
            _ => exception.ToString()
        };

        if (errorMessage != _lastErrorMessage || (DateTime.Now - _lastErrorTimestamp).TotalSeconds >= 1)
        {
            _lastErrorMessage = errorMessage;
            _lastErrorTimestamp = DateTime.Now;

            _logger.Warn(errorMessage);
            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ms") + " " + errorMessage);
        }
        
        return Task.CompletedTask;
    }
}