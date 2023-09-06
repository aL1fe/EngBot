using System.Text.RegularExpressions;
using NLog;
using TelegaEngBot.AppConfigurations;
using TelegaEngBot.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

namespace TelegaEngBot.Services;

public class Pronunciation
{
    private ITelegramBotClient _botClient;
    private Message _message;
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public Pronunciation(
        ITelegramBotClient botClient,
        Message message)
    {
        _botClient = botClient;
        _message = message;
    }
    
    public async Task TextToSpeech(Article article)
    {
        try
        {
            var client = new HttpClient();
            var query = article.EngWord;

            var url = $"{AppConfig.NeuralModelHost}/?query={query}";

            Console.WriteLine(url);
            client.DefaultRequestHeaders.Add("accept", "application/json");

            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                await using var audioStream = await response.Content.ReadAsStreamAsync();
                using var memoryStream = new MemoryStream();
                await audioStream.CopyToAsync(memoryStream);
                memoryStream.Position = 0;
                // await _botClient.SendAudioAsync(_message.Chat.Id, new InputOnlineFile(memoryStream, "Pronunciation.mp3"));
                await _botClient.SendAudioAsync(_message.Chat.Id, new InputOnlineFile(memoryStream, GenerateFileName(query)));
            }
            else
            {
                Console.WriteLine($"Request failed with status code: {response.StatusCode}");
            }
        }
        catch (System.Net.Http.HttpRequestException ex)
        {
            await _botClient.SendTextMessageAsync(_message.Chat.Id, "Sorry, but Pronunciation server unavailable");
            Console.WriteLine($"No connection could be made because the target machine actively refused it. {AppConfig.NeuralModelHost}");
            Logger.Warn($"No connection could be made because the target machine actively refused it. {AppConfig.NeuralModelHost}");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    static string GenerateFileName(string sentence)
    {
        var regex = new Regex("[^a-zA-Zа-яА-Я]");
        var fileName = regex.Replace(sentence, "_") + ".mp3" ;
        return fileName;
    }
    
    public static async Task PronUs(ITelegramBotClient botClient, Message message, Article article)
    {
        var engWordNormalized  = Validator.Normalize(article.EngWord);
        if (engWordNormalized != "error")
        {
            var parser = new Parser();
            var uri = parser.ParsHtml(engWordNormalized);

            try
            {
                if (uri != null) 
                    await botClient.SendAudioAsync(message.Chat.Id, audio: uri);
                else
                    await botClient.SendTextMessageAsync(message.Chat.Id, "*Sorry, cannot play this word.*", ParseMode.Markdown);
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
