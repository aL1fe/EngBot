namespace TelegaEngBot.Services;

internal static class Validator
{
    internal static string ValidateAndTransform(string engWord)
    {
        if (engWord.StartsWith("to ")) // delete "to " from verbs
        {
            engWord = engWord.Remove(0, 3);
        }

        if (engWord.StartsWith(' ')) // delete space in the begin of the word
        {
            engWord = engWord.Remove(0, 1);
        }

        if (engWord.EndsWith(' ')) // delete space in the end of the word TODO check this algorithm
        {
            engWord = engWord.Remove(engWord.Length - 1, 1);
        }

        if (engWord.Contains(' ')) // check if it is not single word
        {
            return "error";
        }

        return engWord;
    }
}