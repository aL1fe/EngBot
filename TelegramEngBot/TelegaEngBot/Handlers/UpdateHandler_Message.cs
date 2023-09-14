using NLog;
using TelegaEngBot.AppConfigurations;
using TelegaEngBot.DataAccessLayer;
using TelegaEngBot.Identity;
using TelegaEngBot.Models;
using TelegaEngBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegaEngBot.Handlers;

public class UpdateHandler_Message
{
    private ITelegramBotClient _botClient;
    private Message _message;
    private static AppDbContext _dbContext;
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    
    public UpdateHandler_Message(ITelegramBotClient botClient, Message message, AppDbContext dbContext)
    {
        _botClient = botClient;
        _message = message;
        _dbContext = dbContext;
    }
    
    public async Task HandleMessage()
    {
        // Identity
        // var identity = new IdentityServer(_message.From.Id);
        // if (!identity.CheckAuth())
        // {
        //     await _botClient.SendTextMessageAsync(_message.Chat.Id, 
        //         "*Access denied.* You should request access and then restart bot using the command //start", 
        //         ParseMode.Markdown);
        //     _logger.Warn(
        //         $"New user tried to connect. User Id: {_message.From.Id}; Username: {_message.From.Username}; FirstName: {_message.From.FirstName}; LastName: {_message.From.LastName}");
        //     return;
        // }
        
        try
        {
            var user = _dbContext.UserList.FirstOrDefault(x => x.TelegramUserId == _message.From.Id);
            var userService = new UserService(_dbContext, _botClient, _message);
            
            // Check if user exist
            if (user == null)
            {
                await userService.CreateUser();
                return;
            }

            // Choose difficulty level if needed
            var messageHandler = new AppMessageHandler(_botClient, _message, _dbContext, user);
            if (user.UserSettings.DifficultyLevel == null || !user.UserVocabulary.Any())
            {
                try
                {
                    var level = (Level)Enum.Parse(typeof(Level), _message.Text);
                    user.UserSettings.DifficultyLevel = level;
                    await _dbContext.SaveChangesAsync();
                    await userService.FillUserVocabularyAndShowNewArticle(user);
                    _message.Text = "/start";
                }
                catch (ArgumentException)
                {
                    await _botClient.SendTextMessageAsync(_message.Chat.Id, "Please choose difficulty level.");
                    await userService.ChooseLanguageLevel(user);
                    return;
                }
            }

            // Add new words for user to check
            if (user.UserVocabulary.Average(x => x.Weight) < AppConfig.AverageWeight
                && user.UserVocabulary.Count != _dbContext.CommonVocabulary.Count()) // UserVocabulary has all the articles from CommonVocabulary
                await userService.FillUserVocabularyAndShowNewArticle(user);
            
            switch (_message.Text)
            {
                case "/start":
                    await messageHandler.Start();
                    break;
                case "Know":
                    await messageHandler.Know();
                    break;
                case "Don't know":
                    await messageHandler.NotKnow();
                    break;
                case "Pronunciation":
                    messageHandler.TextToSpeech();
                    break;
                case "/smile":
                    user.UserSettings.IsSmileOn = !user.UserSettings.IsSmileOn;
                    await _dbContext.SaveChangesAsync();
                    await _botClient.SendTextMessageAsync(_message.Chat.Id,
                        $"Smiles is {(user.UserSettings.IsSmileOn ? "On" : "Off")}");
                    break;
                case "/pronunciation":
                    user.UserSettings.IsPronunciationOn = !user.UserSettings.IsPronunciationOn;
                    await _dbContext.SaveChangesAsync();
                    await messageHandler.RedrawKeyboard(false);
                    break;
                case "/hard":
                    await messageHandler.Hard();
                    break;
                case "/camb":
                    await messageHandler.CambridgePron();
                    break;
                case "/ex":
                    if (_message.Chat.Id is 450056320 or 438560103 or 906180277)
                        await messageHandler.Example();
                    break;
                case "/change":
                    await userService.ConfirmAction();
                    break;
                case "Yes, I want to change":
                    await userService.ChooseLanguageLevel(user);
                    break;
                default:
                    await messageHandler.Start();
                    break;
            }
        }
        catch (Exception exception)
        {
            var mainErrorHandler = new AppErrorHandler(exception, _botClient, _message);
            await mainErrorHandler.HandleError();
        }
    }
}