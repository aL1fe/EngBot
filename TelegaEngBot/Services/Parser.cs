using HtmlAgilityPack;

namespace TelegaEngBot.Services;

internal static class Parser
{
    internal static string ParsHtml(string engWord)
    {
        string? resultUri = null;
        var uri = @"https://dictionary.cambridge.org/dictionary/english/" + engWord;
        var web = new HtmlWeb();
        var htmlDoc = web.Load(uri);
        const string str = "//audio[contains(@class, 'hdn')]//source";
        
        foreach (var node in htmlDoc.DocumentNode.SelectNodes(str))
        {
            var res = node.GetAttributeValue("src", null);
            if (res.EndsWith(".mp3") && res.Contains("us_pron"))
            {
                resultUri = "https://dictionary.cambridge.org" + res;
                break;
            }
        }
        return resultUri;
    }
}