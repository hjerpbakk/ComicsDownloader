#r "nuget: System.IO.Compression, 4.3.0"

using System.IO;
using System.IO.Compression;

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

    public void SaveComic(DateTime comicDate, string extension, byte[] image, int n) {
        var filename = $"{comicDate.Year}{comicDate.Month.ToString("D2")}{comicDate.Day.ToString("D2")}-{comicName}-{n.ToString("D5")}{extension}";
        var filePath = Path.Combine(comicsPath, filename);
        Console.WriteLine($"Saving {filePath}");   
        File.WriteAllBytes(filePath, image);
        if (!minify) {
            return;
        }

        Console.WriteLine($"Optimizing image");
        var processStartInfo = new ProcessStartInfo {
            FileName = "/Applications/ImageOptim.app/Contents/MacOS/ImageOptim",
            Arguments = filePath
        };
        Process.Start(processStartInfo);
    }

    public (string PathOfFirstComic, int Progress) LoadProgress(string pathOfFirstComic)    
    {
        if (!File.Exists(comicSaveFile))
        {
            return (pathOfFirstComic, 0);
        }

        var state = File.ReadAllText(comicSaveFile);
        var split = state.Split('|');
        return (split[0], int.Parse(split[1]));
    }

    public void SaveProgress(string comicPath, int n) => File.WriteAllText(comicSaveFile, $"{comicPath}|{n - 1}");

    public void CreateCBZ() {
        var path = Path.Combine(comicsPath, "..", $"{comicName}.cbz");
        Console.WriteLine($"Creating comic archive {path}"); 
        ZipFile.CreateFromDirectory(comicsPath, path);
    }

    private string CreateDirForComic(string comicName) {
        var path = Path.Combine("comics", comicName, "originals");
        if (!Directory.Exists(path)) {
            Console.WriteLine($"Creating {path}"); 
            Directory.CreateDirectory(path);
        }

        return path;
    }
}