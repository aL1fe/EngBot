using TelegaEngBot.DataAccessLayer;
using TelegaEngBot.Models;
using Telegram.Bot.Types;

namespace TelegaEngBot.Services;

public class UserService
{
    public AppDbContext Context;

    public UserService(AppDbContext context)
    {
        Context = context;
    }

    public AppUser CreateUser(long userId, Message message)
    {
        var userSettings = new UserSettings {Id = new Guid(), IsSmileOn = false, IsPronunciationOn = false};

        var user = new AppUser()
        {
            TelegramUserId = userId,
            TelegramUserName = message.From.Username,
            TelegramFirstName = message.From.FirstName,
            TelegramLastName = message.From.LastName,
            UserVocabulary = Context.CommonVocabulary
                .Select(article => new UserVocabularyItem {Article = article, Weight = 1})
                .ToList(),
            UserSettings = userSettings
        };
        user.TotalArticlesWeight = user.UserVocabulary.Sum(x => x.Weight);

        Context.UserList.Add(user);
        Context.SaveChanges();
        return user;
    }

    public void ChangeUserVocabulary(int userId)
    {
    }

    public void ResetWeightUserVocabulary(int userId)
    {
    }
}