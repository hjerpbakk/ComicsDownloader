#! "netcoreapp2.0"
#load "AppConfig.csx"
#load "FileSystem.csx"
#load "WebClient.csx"
#r "nuget: Newtonsoft.Json, 10.0.3"

using Newtonsoft.Json;
var minify = Args.Contains("m");
var createCBZ = Args.Contains("a");
var config = JsonConvert.DeserializeObject<AppConfig>(File.ReadAllText("config.json"));
var fileSystem = new FileSystem(config.ComicName, minify);
var webClient = new WebClient(config.AddressOfFirstComic, config.LoginCookie);

// TODO: Unify same comic from multiple sources, detect duplicates
// TOOD: Do login as part of script

var progress = fileSystem.LoadProgress(webClient.PathOfFirstComic);
var comicPath = progress.PathOfFirstComic;
var n = progress.Progress;
while (true) {
    n++;
    var start = DateTime.UtcNow;
    var comicHTML = await webClient.GetComicHTML(comicPath);    
    var imageAddress = comicHTML.GetComicPath();
    
    var comicDate = comicHTML.GetComicDate();
    var extension = Path.GetExtension(imageAddress);
    var image = await webClient.DownloadComic(imageAddress);    
    fileSystem.SaveComic(comicDate, extension, image, n);

    fileSystem.SaveProgress(comicPath, n);

    var nextComic = comicHTML.GetNextComic();
    if (!nextComic.Exists) {
        break;
    }

    comicPath = nextComic.Path;
    var end = DateTime.UtcNow;
    var waitTime = 1000 - (end - start).Milliseconds;
    if (waitTime > 0) {
        Console.WriteLine($"Sleeping {waitTime}ms");
        await Task.Delay(waitTime);
    }
}

if (createCBZ) {
    fileSystem.CreateCBZ();
}

Console.WriteLine("Fin");