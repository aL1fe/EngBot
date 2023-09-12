using NLog;
using TelegaEngBot.DataAccessLayer;
using Telegram.Bot;
using Telegram.Bot.Types;

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