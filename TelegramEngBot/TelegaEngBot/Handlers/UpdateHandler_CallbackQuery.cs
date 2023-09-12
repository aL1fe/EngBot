using NLog;
using TelegaEngBot.AppConfigurations;
using TelegaEngBot.DataAccessLayer;
using TelegaEngBot.Identity;
using TelegaEngBot.Models;
using TelegaEngBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

public class UpdateHandler_CallbackQuery
{
    
    private ITelegramBotClient _botClient;
    private CallbackQuery _callbackQuery;
    private static AppDbContext _dbContext;
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    
    public UpdateHandler_CallbackQuery(ITelegramBotClient botClient, CallbackQuery callbackQuery, AppDbContext dbContext)
    {
        _botClient = botClient;
        _callbackQuery = callbackQuery;
        _dbContext = dbContext;
    }

    public async Task HandleCallbackQuery()
    {
    }
}