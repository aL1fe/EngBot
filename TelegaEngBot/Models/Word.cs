namespace TelegaEngBot.Models;

public class Word
{
    public Guid Id { get; set; }
    public string EngWord { get; set; } = null!;
    public string RusWord { get; set; } = null!;
    public string? UrlLink { get; set; }
}
