using System.IO;

public class FileSystem
{
    private readonly string comicName;
    private readonly string comicSaveFile;
    private readonly string comicsPath;
    private readonly bool minify;

    public FileSystem(string comicName, bool minify)
    {
        this.comicName = comicName;
        this.minify = minify;
        comicSaveFile = $"{comicName}.dat";
        comicsPath = CreateDirForComic(comicName);
    }    

    public void SaveComic(DateTime comicDate, string extension, byte[] image) {
        var filename = $"{comicDate.Year}{comicDate.Month.ToString("D2")}{comicDate.Day.ToString("D2")}-{comicName}{extension}";
        var filePath = Path.Combine(comicsPath, filename);
        Console.WriteLine($"Saving {filePath}");   
        File.WriteAllBytes(filePath, image);
        if (!minify) {
            return;
        }

        var processStartInfo = new ProcessStartInfo {
            FileName = "open",
            Arguments = $"-a ImageOptim {filePath}"
        };
        Process.Start(processStartInfo);
    }

    public string LoadProgress(string pathOfFirstComic)    
    {
        if (!File.Exists(comicSaveFile))
        {
            return pathOfFirstComic;
        }

        return File.ReadAllText(comicSaveFile);
    }

    public void SaveProgress(string comicPath) => File.WriteAllText(comicSaveFile, comicPath);

    private string CreateDirForComic(string comicName) {
        var path = Path.Combine("comics", comicName, "originals");
        if (!Directory.Exists(path)) {
            Directory.CreateDirectory(path);
        }

        return path;
    }
}