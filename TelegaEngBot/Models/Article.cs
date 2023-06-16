namespace TelegaEngBot.Models;

public class Article
{
    public Guid Id { get; set; }
    public string EngWord { get; set; }
    public string RusWord { get; set; }
    public string UrlLink { get; set; }
}

// public class BaseEntity
// {
//     public Guid Id { get; set; }
// }
//
// public class LexicalPair: BaseEntity
// {
//     public string Lexeme { get; set; }
//     public string Translation { get; set; }
//     public Level Level { get; set; }
// }
//
// public enum Level
// {
//     BeginnerA1,
//     ElementaryA2,
//     PreIntermediate,
//     IntermediateB1,
//     UpperIntermediateB2,
//     AdvancedC1,
//     ProficiencyC2,
//     Mnemonics1,
//     Mnemonics2,
//     Mnemonics3,
//     SetExpressions,
//     PhrasalVerbs
// }