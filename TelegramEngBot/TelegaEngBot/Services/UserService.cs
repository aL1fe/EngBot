﻿using TelegaEngBot.DataAccessLayer;
using TelegaEngBot.Models;
using Telegram.Bot.Types;
#pragma warning disable CS8602

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
                .Select(article => new UserVocabularyItem {Article = article, Weight = 10})
                .ToList(),
            UserSettings = userSettings
        };
        
        Context.UserList.Add(user);
        Context.SaveChanges();
        return user;
    }

    //todo
    public void ChangeUserVocabulary(int userId)
    {
    }

    //todo
    public void ResetWeightUserVocabulary(int userId)
    {
    }
}