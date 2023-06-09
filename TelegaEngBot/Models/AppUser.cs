namespace TelegaEngBot.Models;

public class AppUser
{
    public Guid Id { get; set; }
    public long TelegramUserId { get; set; }
    public string TelegramUserName { get; set; }
    public string TelegramFirstName { get; set; }
    public string TelegramLastName { get; set; }
    public virtual Article LastArticle { get; set; }
    public virtual List<UserVocabularyItem> UserVocabulary { get; set; }
    public virtual UserSettings UserSettings { get; set; }
    public bool IsSynced { get; set; }
    public DateTime LastActivity { get; set; }
}