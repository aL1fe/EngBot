namespace TelegaEngBot.Models;

public class UserVocabularyItem
{
    public Guid Id { get; set; }
    public virtual Article Article { get; set; } = null!;
    public int Weight { get; set; }
}