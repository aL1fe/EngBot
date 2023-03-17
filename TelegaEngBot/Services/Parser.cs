using HtmlAgilityPack;

namespace TelegaEngBot.Services;

internal static class Parser
{
    internal static string? ParsHtml(string engWord)
    {
        var listUris = new List<string?>(10);

        var uri = @"https://dictionary.cambridge.org/dictionary/english/" + engWord;
        var web = new HtmlWeb();
        var htmlDoc = web.Load(uri);
        const string str = "//audio[contains(@class, 'hdn')]//source";
        
        //TODO сделать проверку что найдено только 2 значения, значит нужного значения нет
        foreach (var node in htmlDoc.DocumentNode.SelectNodes(str))
        {
            var res = node.GetAttributeValue("src", null);
            if (res.EndsWith(".mp3") && res.Contains("us_pron"))
                listUris.Add("https://dictionary.cambridge.org" + res);
        }

        if (listUris.Capacity < 2 ) //if only 2 values are found, therefor the required word is missing
        {
            return null;
        }
        return listUris[0];
    }
}