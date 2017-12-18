#r "nuget: HtmlAgilityPack, 1.6.8"

using HtmlAgilityPack;

public struct ComicHTML 
{ 
    readonly HtmlDocument htmlDoc;

    public ComicHTML(string html)
    {
        htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(html);
    }

    public string GetComicPath() => htmlDoc.DocumentNode.Descendants("img")
                                    .Select(e => e.GetAttributeValue("src", null))
                                    .Single(s => !String.IsNullOrEmpty(s));

    public DateTime GetComicDate() 
    {
        var comicDate = htmlDoc.DocumentNode.Descendants("span")
                                .SingleOrDefault(e => e.GetAttributeValue("class", null) == "pub_date");
        return DateTime.Parse(comicDate.InnerText);
    }

    public (string Path, bool Exists) GetNextComic() {
        var nextComicLink = htmlDoc.DocumentNode.Descendants("link")
                                .SingleOrDefault(e => e.GetAttributeValue("rel", null) == "next");
        if (nextComicLink == null) {
            return ("", false);
        }

        return (nextComicLink.GetAttributeValue("href", null), true);
    } 
}