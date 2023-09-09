using TelegaEngBot.AppConfigurations;
using TelegaEngBot.DataAccessLayer;
using TelegaEngBot.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bots.Http;

#pragma warning disable CS8602

namespace TelegaEngBot.Services;

public class UserService
{
    private AppDbContext _context;
    private ITelegramBotClient _botClient;
    private Message _message;
    
    public UserService(AppDbContext context, ITelegramBotClient botClient, Message message)
    {
        _context = context;
        _botClient = botClient;
        _message = message;
    }

    public async Task CreateUser()
    {
        var userSettings = new UserSettings
        {
            Id = new Guid(), 
            IsSmileOn = false, 
            IsPronunciationOn = true,
            DifficultyLevel = null
        };

        var user = new AppUser()
        {
            TelegramUserId = _message.From.Id,
            TelegramUserName = _message.From.Username,
            TelegramFirstName = _message.From.FirstName,
            TelegramLastName = _message.From.LastName,
            UserVocabulary = new List<UserVocabularyItem>(),
            // UserVocabulary = _context.CommonVocabulary
            //     .Select(article => new UserVocabularyItem {Article = article, Weight = 10})
            //     .ToList(),
            UserSettings = userSettings,
            LastActivity = DateTime.Now
        };
        
        _context.UserList.Add(user);
        await _context.SaveChangesAsync();

        await ChooseLanguageLevel(user);
    }

    public async Task ChooseLanguageLevel(AppUser user)
    {
        // Redraw keyboard
        ReplyKeyboardMarkup keyboard = new(new[]
        {
            // new KeyboardButton[] {Level.BeginnerA1.ToString(), Level.ElementaryA2.ToString(), Level.IntermediateB1.ToString()},
            // new KeyboardButton[] {Level.UpperIntermediateB2.ToString(), Level.AdvancedC1.ToString(), Level.ProficiencyC2.ToString()},
            // new KeyboardButton[] {Level.Mnemonics1.ToString(), Level.Mnemonics2.ToString(), Level.Mnemonics3.ToString()},
            
            new KeyboardButton[] {Level.BeginnerA1.ToString(), Level.Mnemonics2.ToString()}
        })
        {
            ResizeKeyboard = true
        };
        await _botClient.SendTextMessageAsync(_message.Chat.Id, "Which level would you like to study?", replyMarkup: keyboard);

        // If this is old user we should make null UserSettings.DifficultyLevel to assign it later 
        if (user.UserSettings.DifficultyLevel != null)
        {
            user.UserSettings.DifficultyLevel = null;
            await _context.SaveChangesAsync();
        }
        
        // Delete UserVocabulary to fill then new words
        if (user.UserVocabulary.Any())
        {
            user.UserVocabulary.Clear();
            await _context.SaveChangesAsync();
        }
    }

    public async Task FillUserVocabularyAndShowNewArticle(AppUser user)
    {
        IQueryable<Article> newArticleToLearn;
        if (user.UserVocabulary.Any())
        {
            // Add only new article
            var existingUserVocabularyWords = 
                user.UserVocabulary.Select(uv => uv.Article.EngWord).ToList();

            newArticleToLearn =
                _context.CommonVocabulary
                    .Where(x =>
                        x.DifficultyLevel == user.UserSettings.DifficultyLevel &&
                        !existingUserVocabularyWords.Contains(x.EngWord))
                    .Take(10);
        }
        else
        {
            newArticleToLearn =
                _context.CommonVocabulary
                    .Where(x =>
                        x.DifficultyLevel == user.UserSettings.DifficultyLevel)
                    .Take(10);
        }

        if (!newArticleToLearn.Any())
        {
            Console.WriteLine("CommonVocab for this level is empty");
            return;
        }
        
        // user.UserVocabulary = newArticleToLearn
        //     .Select(x => new UserVocabularyItem {Article = x, Weight = 10})
        //     .ToList();
        
        var newItemsToAdd = newArticleToLearn
            .Select(x => new UserVocabularyItem { Article = x, Weight = AppConfig.Start })
            .ToList();

        user.UserVocabulary.AddRange(newItemsToAdd);
        await _context.SaveChangesAsync();
        
        await _botClient.SendTextMessageAsync(_message.Chat.Id, "<strong> ❤️❤️❤️ It's time to learn new words. ❤️❤️❤️ </strong>", ParseMode.Html);
        var tts = new Pronunciation(_botClient, _message);
        foreach (var article in newArticleToLearn)
        {
            await _botClient.SendTextMessageAsync(_message.Chat.Id, $"{article.EngWord}  - {article.RusWord}");
            if (user.UserSettings.IsPronunciationOn)
                await tts.TextToSpeech(article);
        }
        await _botClient.SendTextMessageAsync(_message.Chat.Id, "<strong> 👇👇👇 Let's begin to learn. 👇👇👇 </strong>", ParseMode.Html);
    }

    //todo
    public void ChangeUserLanguageVocabulary(int userId)
    {
    }
}