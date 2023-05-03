namespace TelegaEngBot.Models;

public class AppUser
{
    public Guid Id { get; set; }
    public long TelegramUserId { get; set; }
    public string? TelegramUserName { get; set; }
    public string? TelegramFirstName { get; set; }
    public string? TelegramLastName { get; set; }
    public virtual List<UserVocabularyItem>? UserVocabulary { get; set; }
    public virtual UserSettings UserSettings { get; set; } = null!;
}