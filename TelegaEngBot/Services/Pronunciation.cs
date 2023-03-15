using TelegaEngBot.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegaEngBot.Services;

internal static class Pronunciation
{
    internal static async Task PronUs(ITelegramBotClient botClient, Message message, Word word)
    {
        var engWordTransform = Validator.ValidateAndTransform(word.EngWord);
        if (engWordTransform != "error")
        {
            var uri = Parser.ParsHtml(engWordTransform);

            //uri = "https://dictionary.cambridge.org/media/english/us_pron/d/dog/dog__/g.mp3";  // delete
            //uri = null; // delete
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