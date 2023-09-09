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
            IsPronunciationOn = false,
            DifficultyLevel = null
        };

        var user = new AppUser()
        {
            TelegramUserId = _message.From.Id,
            TelegramUserName = _message.From.Username,
            TelegramFirstName = _message.From.FirstName,
            TelegramLastName = _message.From.LastName,
            UserVocabulary = new List<UserVocabularyItem>(),
            UserSettings = userSettings,
            LastActivity = DateTime.Now
        };
        
        _context.UserList.Add(user);
        await _context.SaveChangesAsync();

        await ChooseLanguageLevel(user);
    }

    public async Task ChooseLanguageLevel(AppUser user)
    {
        // Define number of difficulty level that consist in CommonVocabulary
        var difficultyLevelList = _context.CommonVocabulary
            .Select(c => c.DifficultyLevel) 
            .Distinct()
            .OrderBy(level => level) // sort by enum number
            .ToList();

        var colCount = 3;
        var rowCount = (int)Math.Ceiling((double)difficultyLevelList.Count / colCount);
        var index = 0; // Should be less than difficultyLevelList.Count
        
        // Matrix from button
        var buttonArray = new Level[rowCount, colCount];
        for (int i = 0; i < rowCount; i++)
        {
            for (int j = 0; j < colCount; j++)
            {
                if (index < difficultyLevelList.Count)
                {
                    buttonArray[i, j] = difficultyLevelList[index];
                    index++;
                }
                else
                {
                    break; // We use all the elements from difficultyLevelList
                }
            }
        }

        // Create a list of keyboard rows
        var keyboardRows = new List<List<KeyboardButton>>();
        index = 0;
        for (int i = 0; i < rowCount; i++)
        {
            var row = new List<KeyboardButton>();
            for (int j = 0; j < colCount; j++)
            {
                if (index < difficultyLevelList.Count)
                {
                    var level = buttonArray[i, j];
                    row.Add(new KeyboardButton(level.ToString()));
                    index++;
                }
                else
                {
                    break; // We use all the elements from difficultyLevelList
                }
            }
            keyboardRows.Add(row);
        }

        // Create the reply keyboard markup
        var keyboard = new ReplyKeyboardMarkup(keyboardRows) {ResizeKeyboard = true};

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