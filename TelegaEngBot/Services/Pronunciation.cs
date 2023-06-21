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
            var fileName = Guid.NewGuid().ToString();

            var url = AppConfig.NeuralModelHost + $"/?query={query}&file_name={fileName}";
            client.DefaultRequestHeaders.Add("accept", "application/json");

            var response = await client.GetAsync(url);
            
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " " + responseBody);
                Logger.Trace(responseBody);
                var filePath = AppConfig.PronunciationFolderPath + fileName + ".mp3";
                await using var fileStream = System.IO.File.OpenRead(filePath);
                await _botClient.SendDocumentAsync(_message.Chat.Id, new InputOnlineFile(fileStream, @"Pronunciation.mp3"));
                fileStream.Close();
                System.IO.File.Delete(filePath);
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

/* for testing
greet

*/