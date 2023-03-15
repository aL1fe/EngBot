namespace TelegaEngBot.Models;

public class WordWithWeight
{
    public Guid Id { get; set; }
    public Word Word { get; set; } = null!;
    public int Weight { get; set; }
}