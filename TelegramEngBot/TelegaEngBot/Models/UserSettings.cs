namespace TelegaEngBot.Models;

public class UserSettings
{
    public Guid Id { get; set; }
    public bool IsPronunciationOn { get; set; }
    public bool IsSmileOn { get; set; }
    public Level? DifficultyLevel { get; set; }
}