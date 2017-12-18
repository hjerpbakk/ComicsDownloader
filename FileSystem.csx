using System.IO;

public class FileSystem
{
    private readonly string comicName;
    private readonly string comicSaveFile;
    private readonly string comicPath;

    public FileSystem(string comicName)
    {
        this.comicName = comicName;
        comicSaveFile = $"{comicName}.dat";
        comicPath = CreateDirForComic(comicName);
    }    

    public void SaveComic(DateTime comicDate, string extension, byte[] image) {
        var filename = $"{comicDate.Year}{comicDate.Month.ToString("D2")}{comicDate.Day.ToString("D2")}-{comicName}{extension}";
        var filePath = Path.Combine(comicPath, filename);
        Console.WriteLine($"Saving {filePath}");   
        File.WriteAllBytes(filePath, image);
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