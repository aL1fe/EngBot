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
    private static Article? _article;
    private static KeyboardButton? _btnKnow;
    private static KeyboardButton? _btnNotKnow;
    private static KeyboardButton? _btnUsPron;
    private static ReplyKeyboardMarkup? _keyboard2Btn;
    private static ReplyKeyboardMarkup? _keyboard3Btn;
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    private static void InitiateKeyboard()
    {
        _btnKnow = new KeyboardButton("Know");
        _btnNotKnow = new KeyboardButton("Don't know");
        _btnUsPron = new KeyboardButton("Pronunciation");

        // Create rows
        var row1 = new[] {_btnKnow, _btnNotKnow};
        var row2 = new[] {_btnUsPron};

        // Keyboards
        _keyboard2Btn = new ReplyKeyboardMarkup(new[] {row1}) {ResizeKeyboard = true};
        _keyboard3Btn = new ReplyKeyboardMarkup(new[] {row1, row2}) {ResizeKeyboard = true};
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
                    user.TotalArticlesWeight = user.UserVocabulary.Sum(x => x.Weight);
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
                // add log todo
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
                user.TotalArticlesWeight = user.UserVocabulary.Sum(x => x.Weight);
                await dbContext.SaveChangesAsync();

                if (user.UserSettings.IsSmileOn) //Sad smile
                    await botClient.SendTextMessageAsync(message.Chat.Id, char.ConvertFromUtf32(0x1F622));

                Logger.Trace("UserId: " + message.Chat.Id + ", EngWord: " + _article.EngWord + ", RusWord: " +
                                  _article.RusWord);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                // add log todo
            }
        }

        await GetNewWord(botClient, message, dbContext, user);
    }

    internal static async Task Pron(ITelegramBotClient botClient, Message message)
    {
        if (_article == null) return;
        await Pronunciation.PronUs(botClient, message, _article);
        await botClient.SendTextMessageAsync(message.Chat.Id, "Click play to listen.", ParseMode.Html,
            replyMarkup: _keyboard2Btn);
    }

    private static async Task GetNewWord(ITelegramBotClient botClient, 
        Message message, 
        AppDbContext dbContext,
        AppUser user)
    {
        try
        {
            //if SynchroniseVocabularies todo
            _article = WeightedRandomSelector.SelectArticle(user.TotalArticlesWeight, user.UserVocabulary).Article;
            await botClient.SendTextMessageAsync(message.Chat.Id, _article.RusWord);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            // add log todo
        }

        // Checking if the keyboard is initialized
        if (_keyboard2Btn == null || _keyboard3Btn == null) InitiateKeyboard();

        await RedrawKeyboard(botClient, message, true, user);
    }

    internal static async Task RedrawKeyboard(ITelegramBotClient botClient, 
        Message message, 
        bool ifTypeWord,
        AppUser user)
    {
        if (_article == null) return;
        
        if (Validator.ValidateAndTransform(_article.EngWord) != "error" && user.UserSettings.IsPronunciationOn)
        {
            if (ifTypeWord)
                await botClient.SendTextMessageAsync(message.Chat.Id,
                    "<tg-spoiler>" + _article.EngWord + "</tg-spoiler>",
                    ParseMode.Html, replyMarkup: _keyboard3Btn);
            else
                await botClient.SendTextMessageAsync(message.Chat.Id,
                    "Click \"Pronunciation\" to listen word.",
                    ParseMode.Html, replyMarkup: _keyboard3Btn);
        }
        else
        {
            if (ifTypeWord)
                await botClient.SendTextMessageAsync(message.Chat.Id,
                    "<tg-spoiler>" + _article.EngWord + "</tg-spoiler>",
                    ParseMode.Html, replyMarkup: _keyboard2Btn);
            else
                await botClient.SendTextMessageAsync(message.Chat.Id,
                    "Button \"Pronunciation\" is " + (user.UserSettings.IsPronunciationOn ? "On" : "Off"),
                    ParseMode.Html, replyMarkup: _keyboard2Btn);
        }
    }
}