using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using NLog;
using OpenAI_API;
using TelegaEngBot.AppConfigurations;
using TelegaEngBot.DataAccessLayer;
using TelegaEngBot.Models;
using TelegaEngBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegaEngBot.Handlers;

public class AppMessageHandler
{
    private ITelegramBotClient _botClient;
    private Message _message;
    private AppDbContext _dbContext;
    private AppUser _user;

    private KeyboardButton _btnKnow;
    private KeyboardButton _btnNotKnow;
    private KeyboardButton _btnPron;
    private ReplyKeyboardMarkup _stdKbd;
    private ReplyKeyboardMarkup _extKbdPron;
    
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public AppMessageHandler(
        ITelegramBotClient botClient,
        Message message,
        AppDbContext dbContext,
        AppUser user)
    {
        _botClient = botClient;
        _message = message;
        _dbContext = dbContext;
        _user = user;
        
        _btnKnow = new KeyboardButton("Know");
        _btnNotKnow = new KeyboardButton("Don't know");
        _btnPron = new KeyboardButton("Pronunciation");

        // Create rows
        var row1 = new[] {_btnKnow, _btnNotKnow};
        var row2 = new[] {_btnPron};

        // Keyboards
        _stdKbd = new ReplyKeyboardMarkup(new[] {row1}) {ResizeKeyboard = true}; // [Know], [Don't know]
        _extKbdPron = new ReplyKeyboardMarkup(new[] {row1, row2}) {ResizeKeyboard = true}; // [Pronunciation]
    }

    public async Task Start()
    {
        await GetNewArticle();
    }

    public async Task Know()
    {
        var article = _user.LastArticle;
        if (article != null)
        {
            var userArticle = _user.UserVocabulary.FirstOrDefault(x => x.Article == article);

            if (userArticle.Weight > AppConfig.KnowDecrease)
            {
                userArticle.Weight -= AppConfig.KnowDecrease;
                await _dbContext.SaveChangesAsync();
            }

            if (_user.UserSettings.IsSmileOn) //Happy smile https://apps.timwhitlock.info/emoji/tables/unicode
                await _botClient.SendTextMessageAsync(_message.Chat.Id, char.ConvertFromUtf32(0x1F642));

            _logger.Trace(
                $"UserId: {_message.Chat.Id}, UserFirstName: {_message.Chat.FirstName}; EngWord: {article.EngWord}, RusWord: {article.RusWord}");
            await GetNewArticle();
        }
    }

    public async Task NotKnow()
    {
        var article = _user.LastArticle;
        if (article != null)
        {
            var userArticle = _user.UserVocabulary.FirstOrDefault(x => x.Article == article);

            userArticle.Weight += AppConfig.NotKnowIncrease;
            await _dbContext.SaveChangesAsync();

            if (_user.UserSettings.IsSmileOn) //Sad smile
                await _botClient.SendTextMessageAsync(_message.Chat.Id, char.ConvertFromUtf32(0x1F622));

            _logger.Trace(
                $"UserId: {_message.Chat.Id}, UserFirstName: {_message.Chat.FirstName}; EngWord: {article.EngWord}, RusWord: {article.RusWord}");
            await GetNewArticle();
        }
    }

    public async Task CambridgePron()
    {
        var article = _user.LastArticle;
        if (article == null) return;
        await Pronunciation.PronUs(_botClient, _message, article);
    }

    private async Task GetNewArticle()
    {
        var article = WeightedRandomSelector.SelectArticle(_user.UserVocabulary, _user.LastArticle).Article;
        _user.LastArticle = article;
        _user.LastActivity = DateTime.Now;
        await _dbContext.SaveChangesAsync();
        await _botClient.SendTextMessageAsync(_message.Chat.Id, article.RusWord);
        await _botClient.SendTextMessageAsync(_message.Chat.Id, "👇 ❓");
        await RedrawKeyboard(true);
    }

    public async Task GetLastArticle()
    {
        var article = _user.LastArticle;
        _user.LastActivity = DateTime.Now;
        await _dbContext.SaveChangesAsync();
        await _botClient.SendTextMessageAsync(_message.Chat.Id, article.RusWord);
        await _botClient.SendTextMessageAsync(_message.Chat.Id, "👇 ❓");
        await RedrawKeyboard(true);
    }

    public async Task RedrawKeyboard(bool ifTypeWord) //todo
    {
        var article = _user.LastArticle;
        if (article == null) return;

        if (_user.UserSettings.IsPronunciationOn)
        {
            // Draw extKbdPron keyboard
            if (ifTypeWord)
            {
                await _botClient.SendTextMessageAsync(_message.Chat.Id,
                    "<tg-spoiler>" + article.EngWord + "</tg-spoiler>", ParseMode.Html,
                    replyMarkup: _extKbdPron);
                await _botClient.SendTextMessageAsync(_message.Chat.Id, "👆 ❓ 🧐");
            }
            else
                await _botClient.SendTextMessageAsync(_message.Chat.Id,
                    "Click \"Pronunciation\" to listen word/phrase.",
                    ParseMode.Html, replyMarkup: _extKbdPron);
        }
        else
        {
            // Draw stdKbd keyboard
            if (ifTypeWord)
            {
                await _botClient.SendTextMessageAsync(_message.Chat.Id,
                    "<tg-spoiler>" + article.EngWord + "</tg-spoiler>", ParseMode.Html,
                    replyMarkup: _stdKbd);
                await _botClient.SendTextMessageAsync(_message.Chat.Id, "👆 ❓ 🧐");
            }
            else

                await _botClient.SendTextMessageAsync(_message.Chat.Id, "Button \"Pronunciation\" is Off",
                    ParseMode.Html, replyMarkup: _stdKbd);
        }
    }

    public async Task Hard()
    {
        var hardWordList = _user.UserVocabulary
            .OrderByDescending(x => x.Weight)
            .Take(10)
            .Select(x => new { Article = x.Article, Weight = x.Weight })
            .ToList();
        var tts = new Pronunciation(_botClient, _message);
        foreach (var hardWord in hardWordList)
        {
            await _botClient.SendTextMessageAsync(_message.Chat.Id, hardWord.Article.EngWord + " - " + hardWord.Article.RusWord);
            if (_user.UserSettings.IsPronunciationOn)
                await tts.TextToSpeech(hardWord.Article);
        }
        
        await _botClient.SendTextMessageAsync(_message.Chat.Id, "<strong> 👇👇👇 Let's continue. 👇👇👇 </strong>", ParseMode.Html);
        await GetLastArticle();
        //await _botClient.SendTextMessageAsync(_message.Chat.Id, "<strong> Press /start to continue...</strong>", ParseMode.Html);
    }

    public async Task TextToSpeech()
    {
        var article = _user.LastArticle;
        var tts = new Pronunciation(_botClient, _message);
        await tts.TextToSpeech(article);
    }
    
    public async Task Example()
    {
        var article = _user.LastArticle;
        var openAi = new OpenAIAPI(new APIAuthentication(AppConfig.OpenAiToken));
        var conversation = openAi.Chat.CreateConversation();
        conversation.AppendUserInput(
            $"Give me 3 examples with \"{article.EngWord}\" for beginner level. The length of each example is no more than 20 words. Mark {article.EngWord} like <b><i>{article.EngWord}</i></b>.");

        var response = await conversation.GetResponseFromChatbotAsync();
        await _botClient.SendTextMessageAsync(_message.Chat.Id, response, ParseMode.Html);
    }

    public async Task LastActivity()
    {
        var lastActivity = _dbContext.UserList.Max(x => x.LastActivity);
        var user = _dbContext.UserList
            .FirstOrDefault(x => x.LastActivity == lastActivity);

        // convert UTC to local time zone
        var localTimeZone = TimeZoneInfo.Local;
        lastActivity = localTimeZone.IsDaylightSavingTime(DateTime.Now)
            ? lastActivity.AddHours(localTimeZone.BaseUtcOffset.Hours - 1)
            : lastActivity.AddHours(localTimeZone.BaseUtcOffset.Hours);
        
        await _botClient.SendTextMessageAsync(_message.Chat.Id,
            $"{user.TelegramFirstName} - {lastActivity.ToString("dd/MM/yyyy HH:mm:ss")}");
    }
}
