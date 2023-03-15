using Telegram.Bot;
using Telegram.Bot.Exceptions;

namespace TelegaEngBot.Handlers;

internal static class ErrorHandler
{
    internal static Task HandleError(ITelegramBotClient client, Exception exception, CancellationToken ct)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException => $"Error code: {apiRequestException.ErrorCode} - {apiRequestException.Message}",
            _ => exception.ToString()
        };
        Console.WriteLine(errorMessage);
        return Task.CompletedTask;
    }
}