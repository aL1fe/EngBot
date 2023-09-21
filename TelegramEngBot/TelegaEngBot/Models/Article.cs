namespace TelegaEngBot.Models;

public enum Level
{
    Mnemonics1 = 0,
    Mnemonics2 = 1,
    Mnemonics3 = 2,
    BeginnerA1 = 3,
    ElementaryA2 = 4,
    PreIntermediate = 5,
    IntermediateB1 = 6,
    UpperIntermediateB2 = 7,
    AdvancedC1 = 8,
    ProficiencyC2 = 9,
    SetExpressions = 10,
    PhrasalVerbs = 11,
    StreamsShiza = 12
}

public class Article
{
    public Guid Id { get; set; }
    public string EngWord { get; set; }
    public string RusWord { get; set; }
    public string UrlLink { get; set; }
    public Level DifficultyLevel { get; set; }
}