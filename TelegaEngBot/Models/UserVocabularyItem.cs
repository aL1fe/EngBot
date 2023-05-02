namespace TelegaEngBot.Models;

public class UserVocabularyItem
{
    public Guid Id { get; set; }
    public Article Article { get; set; } = null!;
    public int Weight { get; set; }
}