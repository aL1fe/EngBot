using NLog;
using Telegram.Bot;
using Telegram.Bot.Exceptions;

namespace TelegaEngBot.Handlers;

public static class ErrorHandler
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private static string _lastErrorMessage = string.Empty;
    private static DateTime _lastErrorTimestamp = DateTime.MinValue;

    public static Task HandleError(ITelegramBotClient client, Exception exception, CancellationToken ct)
    {
        var errorMessage = exception switch
        {
            Telegram.Bot.Exceptions.RequestException {InnerException: System.Net.Http.HttpRequestException httpException} =>
                $"Telegram HTTP request error: {httpException.Message}",
            Telegram.Bot.Exceptions.RequestException {InnerException: System.Threading.Tasks.TaskCanceledException canceledException} =>
                $"Telegram request canceled: {canceledException.Message}",
            Telegram.Bot.Exceptions.RequestException {InnerException: System.IO.IOException ioException} =>
                $"Telegram I/O error: {ioException.Message}",
            Telegram.Bot.Exceptions.ApiRequestException apiRequestException =>
                $"Telegram API error: {apiRequestException.ErrorCode} - {apiRequestException.Message}",
            System.Net.Sockets.SocketException socketException =>
                $"Network error: {socketException.Message}",
            System.Net.Http.HttpRequestException httpException =>
                $"HTTP request error: {httpException.Message}",
            System.Net.WebException webException =>
                $"Web error: {webException.Message}",
            System.Security.Authentication.AuthenticationException authException =>
                $"OpenAI authentication error: {authException.Message}",
            _ => exception.ToString()
        };

        if (errorMessage != _lastErrorMessage || (DateTime.Now - _lastErrorTimestamp).TotalSeconds >= 1)
        {
            _lastErrorMessage = errorMessage;
            _lastErrorTimestamp = DateTime.Now;

            Logger.Warn(errorMessage);
            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ms") + " " + errorMessage);
        }
        
        return Task.CompletedTask;
    }
}