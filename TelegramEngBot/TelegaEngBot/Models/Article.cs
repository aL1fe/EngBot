namespace TelegaEngBot.Models;

public enum Level
{
    Mnemonics1,
    Mnemonics2,
    Mnemonics3,
    BeginnerA1,
    ElementaryA2,
    PreIntermediate,
    IntermediateB1,
    UpperIntermediateB2,
    AdvancedC1,
    ProficiencyC2,
    SetExpressions,
    PhrasalVerbs
}

public class Article
{
    public Guid Id { get; set; }
    public string EngWord { get; set; }
    public string RusWord { get; set; }
    public string UrlLink { get; set; }
    public Level DifficultyLevel { get; set; }
}
