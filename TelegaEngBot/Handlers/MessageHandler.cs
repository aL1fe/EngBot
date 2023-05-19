using NLog;
using TelegaEngBot.DataAccessLayer;
using TelegaEngBot.Models;
using TelegaEngBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegaEngBot.Handlers;

public class MessageHandler
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

    public MessageHandler(ITelegramBotClient botClient,
        Message message,
        AppDbContext dbContext,
        AppUser user)
    {
        _botClient = botClient;
        _message = message;
        _dbContext = dbContext;
        _user = user;
    }
    
    private void InitiateKeyboard()
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

    internal async Task Start()
    {
        InitiateKeyboard();
        await GetNewWord();
    }

    internal async Task Know()
    {
        var article = _user.LastArticle;
        if (article != null)
        {
            try
            {
                var userArticle = _user.UserVocabulary.FirstOrDefault(x => x.Article == article);

                if (userArticle.Weight > 1)
                {
                    userArticle.Weight--;
                    // user.TotalArticlesWeight = user.UserVocabulary.Sum(x => x.Weight);
                    await _dbContext.SaveChangesAsync();
                }

                if (_user.UserSettings.IsSmileOn) //Happy smile https://apps.timwhitlock.info/emoji/tables/unicode
                    await _botClient.SendTextMessageAsync(_message.Chat.Id, char.ConvertFromUtf32(0x1F642));

                _logger.Trace("UserId: " + _message.Chat.Id + ", EngWord: " + article.EngWord + ", RusWord: " +
                              article.RusWord);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                _logger.Error(e);
            }
        }

        await GetNewWord();
    }

    internal async Task NotKnow()
    {
        var article = _user.LastArticle;
        if (article != null)
        {
            try
            {
                var userArticle = _user.UserVocabulary.FirstOrDefault(x => x.Article == article);

                userArticle.Weight++;
                // user.TotalArticlesWeight = user.UserVocabulary.Sum(x => x.Weight);
                await _dbContext.SaveChangesAsync();

                if (_user.UserSettings.IsSmileOn) //Sad smile
                    await _botClient.SendTextMessageAsync(_message.Chat.Id, char.ConvertFromUtf32(0x1F622));

                _logger.Trace("UserId: " + _message.Chat.Id + ", EngWord: " + article.EngWord + ", RusWord: " +
                                  article.RusWord);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                _logger.Error(e);
            }
        }

        await GetNewWord();
    }

    internal async Task Pron()
    {
        var article = _user.LastArticle;
        if (article == null) return;
        await Pronunciation.PronUs(_botClient, _message, article);
        await _botClient.SendTextMessageAsync(_message.Chat.Id, "Click play to listen.", ParseMode.Html,
            replyMarkup: _stdKbd);
    }

    private async Task GetNewWord()
    {
        try
        {
            //if SynchroniseVocabularies todo
            var article = WeightedRandomSelector.SelectArticle(_user.UserVocabulary).Article;
            _user.LastArticle = article;
            await _dbContext.SaveChangesAsync();
            await _botClient.SendTextMessageAsync(_message.Chat.Id, article.RusWord);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            // add log todo
        }

        // Checking if the keyboard is initialized
        if (_stdKbd == null || _extKbdPron == null) InitiateKeyboard();

        await RedrawKeyboard(true);
    }

    internal async Task RedrawKeyboard(bool ifTypeWord)
    {
        var article = _user.LastArticle;
        if (article == null) return;
        
        if (Validator.Normalize(article.EngWord) != "error" && _user.UserSettings.IsPronunciationOn)
        {
            if (ifTypeWord)
                await _botClient.SendTextMessageAsync(_message.Chat.Id,
                    "<tg-spoiler>" + article.EngWord + "</tg-spoiler>",
                    ParseMode.Html, replyMarkup: _extKbdPron);
            else
                await _botClient.SendTextMessageAsync(_message.Chat.Id,
                    "Click \"Pronunciation\" to listen word.",
                    ParseMode.Html, replyMarkup: _extKbdPron);
        }
        else
        {
            if (ifTypeWord)
                await _botClient.SendTextMessageAsync(_message.Chat.Id,
                    "<tg-spoiler>" + article.EngWord + "</tg-spoiler>",
                    ParseMode.Html, replyMarkup: _stdKbd);
            else
                await _botClient.SendTextMessageAsync(_message.Chat.Id,
                    "Button \"Pronunciation\" is " + (_user.UserSettings.IsPronunciationOn ? "On" : "Off"),
                    ParseMode.Html, replyMarkup: _stdKbd);
        }
    }

    internal async Task Hard()
    {
        var hardWordList = _user.UserVocabulary
            .OrderByDescending(x => x.Weight)
            .Take(20)
            .Select(x => new { Article = x.Article, Weight = x.Weight })
            .ToList();
        foreach (var hardWord in hardWordList)
        {
            await _botClient.SendTextMessageAsync(_message.Chat.Id, hardWord.Article.EngWord + " - " + hardWord.Article.RusWord);
        }
        await _botClient.SendTextMessageAsync(_message.Chat.Id, "<strong> Press /start to continue...</strong>", ParseMode.Html);
    }
}
