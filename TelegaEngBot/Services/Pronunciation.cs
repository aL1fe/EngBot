using TelegaEngBot.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegaEngBot.Services;

internal static class Pronunciation
{
    internal static async Task PronUs(ITelegramBotClient botClient, Message message, Article article)
    {
        var engWordTransform = Validator.ValidateAndTransform(article.EngWord);
        if (engWordTransform != "error")
        {
            var uri = Parser.ParsHtml(engWordTransform);

            try
            {
                await botClient.SendAudioAsync(message.Chat.Id, audio: uri);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                await botClient.SendTextMessageAsync(message.Chat.Id, "*Sorry, cannot play this word.*", ParseMode.Markdown);
            }
        }
        else
            await botClient.SendTextMessageAsync(message.Chat.Id, "*Cannot play sentences.*", ParseMode.Markdown);
    }
}