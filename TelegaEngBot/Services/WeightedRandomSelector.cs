using TelegaEngBot.Models;

namespace TelegaEngBot.Services;

public class WeightedRandomSelector
{
    internal static UserVocabularyItem? SelectArticle(int maxWeight, List<UserVocabularyItem> userVocabulary)
    {
        var random = new Random();
        int randomWeight = random.Next(0, maxWeight);
        int cumulativeWeight = 0;
        
        foreach (var userVocabularyItem in userVocabulary)
        {
            cumulativeWeight += userVocabularyItem.Weight;
            if (randomWeight < cumulativeWeight) return userVocabularyItem;
        }

        return userVocabulary.LastOrDefault();
    }
}