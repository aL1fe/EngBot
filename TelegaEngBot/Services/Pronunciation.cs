using NLog;
using TelegaEngBot.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegaEngBot.Services;

internal static class Pronunciation
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    internal static async Task PronUs(ITelegramBotClient botClient, Message message, Article article)
    {
        var engWordTransform = Validator.ValidateAndTransform(article.EngWord);
        if (engWordTransform != "error")
        {
            var uri = Parser.ParsHtml(engWordTransform);

            try
            {
                if (uri != null) 
                    await botClient.SendAudioAsync(message.Chat.Id, audio: uri);
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
                await botClient.SendTextMessageAsync(message.Chat.Id, "*Sorry, cannot play this word.*", ParseMode.Markdown);
            }
        }
        else
            await botClient.SendTextMessageAsync(message.Chat.Id, "*Cannot play sentences.*", ParseMode.Markdown);
    }
}