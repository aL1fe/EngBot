namespace TelegaEngBot.Models;

public class User
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public List<UserVocabularyItem>? UserVocabulary { get; set; }
    public int TotalArticlesWeight { get; set; }
    public UserSettings UserSettings { get; set; }
}
