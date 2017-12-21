#r "nuget: System.IO.Compression, 4.3.0"
#r "nuget: Polly, 5.6.1"

using System.IO;
using System.IO.Compression;
using Polly;

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

        // To handle intermittent exceptions of type: System.ComponentModel.Win32Exception (0x80004005): Resource temporarily unavailable
        Policy
            .Handle<System.ComponentModel.Win32Exception>()
            .WaitAndRetry(3, retryAttempt => TimeSpan.FromSeconds(1))
            .Execute(() => OptimizeImage(filePath));
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
        if (File.Exists(path)) {
            File.Delete(path);
        }
        
        ZipFile.CreateFromDirectory(comicsPath, path);
    }

    public void CopyToDropLocation(string dropLocationPath) {
        var comicFolder = Path.Combine(comicsPath, "..");
        var destinationComicFolder = Path.Combine(dropLocationPath, comicName);
        Console.WriteLine($"Copying {comicFolder} to {destinationComicFolder}");
        DirectoryCopy(comicFolder, destinationComicFolder);
        var destinationSaveFile = Path.Combine(destinationComicFolder, "..", Path.GetFileName(comicSaveFile));
        Console.WriteLine($"Copying {comicSaveFile} to {destinationSaveFile}");
        File.Copy(comicSaveFile, destinationSaveFile);
    }
    
    private string CreateDirForComic(string comicName) {
        var path = Path.Combine("comics", comicName, "originals");
        if (!Directory.Exists(path)) {
            Console.WriteLine($"Creating {path}"); 
            Directory.CreateDirectory(path);
        }

        return path;
    }

    private void OptimizeImage(string filePath) {
        Console.WriteLine($"Optimizing image");
        var processStartInfo = new ProcessStartInfo {
            FileName = "/Applications/ImageOptim.app/Contents/MacOS/ImageOptim",
            Arguments = $"\"{filePath}\"",
            UseShellExecute = false
        };
        
        var process = new Process { StartInfo = processStartInfo };
        process.Start(); 
        process.WaitForExit();
    }

    private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs = true)
    {
        // Get the subdirectories for the specified directory.
        DirectoryInfo dir = new DirectoryInfo(sourceDirName);

        if (!dir.Exists)
        {
            throw new DirectoryNotFoundException(
                "Source directory does not exist or could not be found: "
                + sourceDirName);
        }

        DirectoryInfo[] dirs = dir.GetDirectories();
        // If the destination directory doesn't exist, create it.
        if (!Directory.Exists(destDirName))
        {
            Directory.CreateDirectory(destDirName);
        }
        
        // Get the files in the directory and copy them to the new location.
        FileInfo[] files = dir.GetFiles();
        foreach (FileInfo file in files)
        {
            string temppath = Path.Combine(destDirName, file.Name);
            file.CopyTo(temppath, true);
        }

        // If copying subdirectories, copy them and their contents to new location.
        if (copySubDirs)
        {
            foreach (DirectoryInfo subdir in dirs)
            {
                string temppath = Path.Combine(destDirName, subdir.Name);
                DirectoryCopy(subdir.FullName, temppath, copySubDirs);
            }
        }
    }
}