using TelegaEngBot.Models;

namespace TelegaEngBot.Services;

public class WeightedRandomSelector
{
    public static UserVocabularyItem SelectArticle(List<UserVocabularyItem> userVocabulary, Article lastArticle)
    {
        var maxWeight = userVocabulary.Sum(x => x.Weight);
        var random = new Random();
        int randomWeight = random.Next(0, maxWeight);
        int cumulativeWeight = 0;

        foreach (var userVocabularyItem in userVocabulary)
        {
            cumulativeWeight += userVocabularyItem.Weight;
            if (randomWeight >= cumulativeWeight) continue;

            if (userVocabularyItem.Article != lastArticle) return userVocabularyItem; // Check if random choose the previous article
            random = new Random();
            var randomItem = random.Next(0, userVocabulary.Count);
            return userVocabulary[randomItem];                                          // Return random item
            // return userVocabulary.OrderByDescending(x => x.Weight).FirstOrDefault(); // Return item with max weight
        }

        return userVocabulary.LastOrDefault();
    }
}