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
    private static KeyboardButton _btnKnow;
    private static KeyboardButton _btnNotKnow;
    private static KeyboardButton _btnUsPron;
    private static ReplyKeyboardMarkup _Keyboard2btn;
    private static ReplyKeyboardMarkup _Keyboard3btn;

    private static void InitiateKeyboard()
    {
        _btnKnow = new KeyboardButton("Know");
        _btnNotKnow = new KeyboardButton("Don't know");
        _btnUsPron = new KeyboardButton("US pron");

        // Create rows
        var row1 = new[] {_btnKnow, _btnNotKnow};
        var row2 = new[] {_btnUsPron};

        // Keyboards
        _Keyboard2btn = new ReplyKeyboardMarkup(new[] {row1}) {ResizeKeyboard = true};
        _Keyboard3btn = new ReplyKeyboardMarkup(new[] {row1, row2}) {ResizeKeyboard = true};
    }

    internal static async Task Start(ITelegramBotClient botClient, Message message, AppDbContext dbContext)
    {
        InitiateKeyboard();
        await GetNewWord(botClient, message, dbContext);
    }

    internal static async Task Know(ITelegramBotClient botClient, Message message, AppDbContext dbContext)
    {
        //TODO Increase weight
        await botClient.SendTextMessageAsync(message.Chat.Id,
            char.ConvertFromUtf32(0x1F642)); //Happy smile https://apps.timwhitlock.info/emoji/tables/unicode
        await GetNewWord(botClient, message, dbContext);
    }

    internal static async Task NotKnow(ITelegramBotClient botClient, Message message, AppDbContext dbContext)
    {
        //TODO Decrease weight
        await botClient.SendTextMessageAsync(message.Chat.Id, char.ConvertFromUtf32(0x1F622)); //Sad smile
        await GetNewWord(botClient, message, dbContext);
    }

    internal static async Task Pron(ITelegramBotClient botClient, Message message)
    {
        if (_word == null) return;
        await Pronunciation.PronUs(botClient, message, _word);
        await botClient.SendTextMessageAsync(message.Chat.Id, "Click play to listen.", ParseMode.Html,
            replyMarkup: _Keyboard2btn);
    }

    private static async Task GetNewWord(ITelegramBotClient botClient, Message message, AppDbContext dbContext)
    {
        var rnd = new Random();
        var rndWord = rnd.Next(0, dbContext.Dictionary.Count());
        _word = dbContext.Dictionary.Skip(rndWord).Take(1).FirstOrDefault();
        await botClient.SendTextMessageAsync(message.Chat.Id, _word.RusWord);

        // Checking if the keyboard is initialized
        if (_Keyboard2btn == null || _Keyboard3btn == null)
        {
            InitiateKeyboard();
        }

        // Redraw keyboard
        if (Validator.ValidateAndTransform(_word.EngWord) != "error")
        {
            await botClient.SendTextMessageAsync(message.Chat.Id, "<tg-spoiler>" + _word.EngWord + "</tg-spoiler>",
                ParseMode.Html, replyMarkup: _Keyboard3btn);
        }
        else
        {
            await botClient.SendTextMessageAsync(message.Chat.Id, "<tg-spoiler>" + _word.EngWord + "</tg-spoiler>",
                ParseMode.Html, replyMarkup: _Keyboard2btn);
        }
    }
}