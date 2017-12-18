#! "netcoreapp2.0"
#load "AppConfig.csx"
#load "FileSystem.csx"
#load "WebClient.csx"
#r "nuget: Newtonsoft.Json, 10.0.3"

using Newtonsoft.Json;
var config = JsonConvert.DeserializeObject<AppConfig>(File.ReadAllText("config.json"));
var fileSystem = new FileSystem(config.ComicName);
var webClient = new WebClient(config.AddressOfFirstComic, config.LoginCookie);

// TODO: Minify, if user wants
// TODO: Combine into a comic archive, if user wants
// TODO: Unify same comic from multiple sources, detect duplicates
// TOOD: Do login as part of script

var comicPath = fileSystem.LoadProgress(webClient.PathOfFirstComic);
while (true) {
    var comicHTML = await webClient.GetComicHTML(comicPath);    
    var imageAddress = comicHTML.GetComicPath();
    
    var comicDate = comicHTML.GetComicDate();
    var extension = Path.GetExtension(imageAddress);
    var image = await webClient.DownloadComic(imageAddress);    
    fileSystem.SaveComic(comicDate, extension, image);

    fileSystem.SaveProgress(comicPath);

    var nextComic = comicHTML.GetNextComic();
    if (!nextComic.Exists) {
        break;
    }

    comicPath = nextComic.Path;
    await Task.Delay(1000);
}

Console.WriteLine("Fin");