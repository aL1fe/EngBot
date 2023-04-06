using NLog;
using TelegaEngBot.DataAccessLayer;
using TelegaEngBot.Models;
using TelegaEngBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegaEngBot.Handlers;

internal static class MessageHandler
{
    private static Word? _word;
    private static KeyboardButton? _btnKnow;
    private static KeyboardButton? _btnNotKnow;
    private static KeyboardButton? _btnUsPron;
    private static ReplyKeyboardMarkup? _keyboard2Btn;
    private static ReplyKeyboardMarkup? _keyboard3Btn;
    private static Logger _logger = LogManager.GetCurrentClassLogger();
    internal static bool IsPronunciationOn = false;

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

    internal static async Task Start(ITelegramBotClient botClient, Message message, AppDbContext dbContext)
    {
        InitiateKeyboard();
        await GetNewWord(botClient, message, dbContext);
    }

    internal static async Task Know(ITelegramBotClient botClient, Message message, AppDbContext dbContext,
        bool isSmileOn)
    {
        //TODO Increase weight
        if (isSmileOn) //Happy smile https://apps.timwhitlock.info/emoji/tables/unicode
            await botClient.SendTextMessageAsync(message.Chat.Id,
                char.ConvertFromUtf32(0x1F642));
        if (_word != null)
            _logger.Trace("UserId: " + message.Chat.Id + ", EngWord: " + _word.EngWord + ", RusWord: " + _word.RusWord);
        await GetNewWord(botClient, message, dbContext);
    }

    internal static async Task NotKnow(ITelegramBotClient botClient, Message message, AppDbContext dbContext,
        bool isSmileOn)
    {
        //TODO Decrease weight
        if (isSmileOn) //Sad smile
            await botClient.SendTextMessageAsync(message.Chat.Id, char.ConvertFromUtf32(0x1F622));
        if (_word != null)
            _logger.Trace("UserId: " + message.Chat.Id + ", EngWord: " + _word.EngWord + ", RusWord: " + _word.RusWord);
        await GetNewWord(botClient, message, dbContext);
    }

    internal static async Task Pron(ITelegramBotClient botClient, Message message)
    {
        if (_word == null) return;
        await Pronunciation.PronUs(botClient, message, _word);
        await botClient.SendTextMessageAsync(message.Chat.Id, "Click play to listen.", ParseMode.Html,
            replyMarkup: _keyboard2Btn);
    }

    private static async Task GetNewWord(ITelegramBotClient botClient, Message message, AppDbContext dbContext)
    {
        var rnd = new Random();
        var rndWord = rnd.Next(0, dbContext.Dictionary.Count());
        _word = dbContext.Dictionary.Skip(rndWord).Take(1).FirstOrDefault();
        await botClient.SendTextMessageAsync(message.Chat.Id, _word.RusWord);

        // Checking if the keyboard is initialized
        if (_keyboard2Btn == null || _keyboard3Btn == null)
        {
            InitiateKeyboard();
        }

        // Redraw keyboard
        if (Validator.ValidateAndTransform(_word.EngWord) != "error" && IsPronunciationOn)
        {
            await botClient.SendTextMessageAsync(message.Chat.Id, "<tg-spoiler>" + _word.EngWord + "</tg-spoiler>",
                ParseMode.Html, replyMarkup: _keyboard3Btn);
        }
        else
        {
            await botClient.SendTextMessageAsync(message.Chat.Id, "<tg-spoiler>" + _word.EngWord + "</tg-spoiler>",
                ParseMode.Html, replyMarkup: _keyboard2Btn);
        }
    }
}