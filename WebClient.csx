#load "ComicHTML.csx"

using System.Net.Http;

public class WebClient 
{
    readonly HttpClient imageHttpClient;
    readonly HttpClient comicHttpClient;

    readonly string loginCookie;
    readonly string baseAddress;
    
    public WebClient(string addressOfFirstComic, string loginCookie)
    {
        this.loginCookie = loginCookie;
        var uriOfFirstComic = new Uri(addressOfFirstComic);
        baseAddress = $"{uriOfFirstComic.Scheme}://{uriOfFirstComic.Host}";
        PathOfFirstComic = uriOfFirstComic.PathAndQuery;

        var loggedInHandler = new HttpClientHandler { UseCookies = false };
        comicHttpClient = new HttpClient(loggedInHandler) { BaseAddress = new Uri(baseAddress) };
        imageHttpClient = new HttpClient();
    }

    public string PathOfFirstComic { get; }

    public async Task<ComicHTML> GetComicHTML(string request) {
        Console.WriteLine($"Analyzing {baseAddress}{request}");
        var message = new HttpRequestMessage(HttpMethod.Get, request);
        message.Headers.Add("Cookie", loginCookie);
        var result = await comicHttpClient.SendAsync(message);
        result.EnsureSuccessStatusCode();
        var html = await result.Content.ReadAsStringAsync();
        return new ComicHTML(html);
    }

    public async Task<byte[]> DownloadComic(string comicURL) 
    {
        Console.WriteLine($"Downloading {comicURL}..."); 
        return await imageHttpClient.GetByteArrayAsync(comicURL); 
    }
}