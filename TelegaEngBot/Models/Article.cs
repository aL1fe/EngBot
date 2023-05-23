namespace TelegaEngBot.Models;

public class Article
{
    public Guid Id { get; set; }
    public string EngWord { get; set; }
    public string RusWord { get; set; }
    public string UrlLink { get; set; }
}