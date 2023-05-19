using NLog;
using TelegaEngBot.DataAccessLayer;
using TelegaEngBot.Models;
using TelegaEngBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
#pragma warning disable CS8604
#pragma warning disable CS8602

namespace TelegaEngBot.Handlers;

internal static class MessageHandler
{
    private static Article _article;
    private static KeyboardButton _btnKnow;
    private static KeyboardButton _btnNotKnow;
    private static KeyboardButton _btnPron;
    private static ReplyKeyboardMarkup _stdKbd;
    private static ReplyKeyboardMarkup _extKbdPron;
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    private static void InitiateKeyboard()
    {
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

    internal static async Task Start(ITelegramBotClient botClient, 
        Message message, 
        AppDbContext dbContext, 
        AppUser user)
    {
        InitiateKeyboard();
        await GetNewWord(botClient, message, dbContext, user);
    }

    internal static async Task Know(ITelegramBotClient botClient,
        Message message,
        AppDbContext dbContext,
        AppUser user)
    {
        if (_article != null)
        {
            try
            {
                var userArticle = user.UserVocabulary.FirstOrDefault(x => x.Article == _article);

                if (userArticle.Weight > 1)
                {
                    userArticle.Weight--;
                    // user.TotalArticlesWeight = user.UserVocabulary.Sum(x => x.Weight);
                    await dbContext.SaveChangesAsync();
                }

                if (user.UserSettings.IsSmileOn) //Happy smile https://apps.timwhitlock.info/emoji/tables/unicode
                    await botClient.SendTextMessageAsync(message.Chat.Id, char.ConvertFromUtf32(0x1F642));

                Logger.Trace("UserId: " + message.Chat.Id + ", EngWord: " + _article.EngWord + ", RusWord: " +
                              _article.RusWord);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Logger.Error(e);
            }
        }

        await GetNewWord(botClient, message, dbContext, user);
    }

    internal static async Task NotKnow(ITelegramBotClient botClient,
        Message message,
        AppDbContext dbContext,
        AppUser user)
    {
        if (_article != null)
        {
            try
            {
                var userArticle = user.UserVocabulary.FirstOrDefault(x => x.Article == _article);

                userArticle.Weight++;
                // user.TotalArticlesWeight = user.UserVocabulary.Sum(x => x.Weight);
                await dbContext.SaveChangesAsync();

                if (user.UserSettings.IsSmileOn) //Sad smile
                    await botClient.SendTextMessageAsync(message.Chat.Id, char.ConvertFromUtf32(0x1F622));

                Logger.Trace("UserId: " + message.Chat.Id + ", EngWord: " + _article.EngWord + ", RusWord: " +
                                  _article.RusWord);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Logger.Error(e);
            }
        }

        await GetNewWord(botClient, message, dbContext, user);
    }

    internal static async Task Pron(ITelegramBotClient botClient, Message message)
    {
        if (_article == null) return;
        await Pronunciation.PronUs(botClient, message, _article);
        await botClient.SendTextMessageAsync(message.Chat.Id, "Click play to listen.", ParseMode.Html,
            replyMarkup: _stdKbd);
    }

    private static async Task GetNewWord(ITelegramBotClient botClient, 
        Message message, 
        AppDbContext dbContext,
        AppUser user)
    {
        try
        {
            //if SynchroniseVocabularies todo
            _article = WeightedRandomSelector.SelectArticle(user.UserVocabulary).Article;
            await botClient.SendTextMessageAsync(message.Chat.Id, _article.RusWord);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            // add log todo
        }

        // Checking if the keyboard is initialized
        if (_stdKbd == null || _extKbdPron == null) InitiateKeyboard();

        await RedrawKeyboard(botClient, message, true, user);
    }

    internal static async Task RedrawKeyboard(ITelegramBotClient botClient, 
        Message message, 
        bool ifTypeWord,
        AppUser user)
    {
        if (_article == null) return;
        
        if (Validator.Normalize(_article.EngWord) != "error" && user.UserSettings.IsPronunciationOn)
        {
            if (ifTypeWord)
                await botClient.SendTextMessageAsync(message.Chat.Id,
                    "<tg-spoiler>" + _article.EngWord + "</tg-spoiler>",
                    ParseMode.Html, replyMarkup: _extKbdPron);
            else
                await botClient.SendTextMessageAsync(message.Chat.Id,
                    "Click \"Pronunciation\" to listen word.",
                    ParseMode.Html, replyMarkup: _extKbdPron);
        }
        else
        {
            if (ifTypeWord)
                await botClient.SendTextMessageAsync(message.Chat.Id,
                    "<tg-spoiler>" + _article.EngWord + "</tg-spoiler>",
                    ParseMode.Html, replyMarkup: _stdKbd);
            else
                await botClient.SendTextMessageAsync(message.Chat.Id,
                    "Button \"Pronunciation\" is " + (user.UserSettings.IsPronunciationOn ? "On" : "Off"),
                    ParseMode.Html, replyMarkup: _stdKbd);
        }
    }

    internal static async Task Hard(ITelegramBotClient botClient,
        Message message,
        AppUser user)
    {
        var hardWordList = user.UserVocabulary
            .OrderByDescending(x => x.Weight)
            .Take(20)
            .Select(x => new { Article = x.Article, Weight = x.Weight })
            .ToList();
        foreach (var hardWord in hardWordList)
        {
            await botClient.SendTextMessageAsync(message.Chat.Id, hardWord.Article.EngWord + " - " + hardWord.Article.RusWord);
        }
        await botClient.SendTextMessageAsync(message.Chat.Id, "<strong> Press /start to continue...</strong>", ParseMode.Html);
    }
}