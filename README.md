# Comics Downloader

Comics Downloader is a dotnet script that will download comics from [comics.io](https://comics.io/). It will start with a specified strip and continue until all strips are downloaded for the given comic, and optionally combine them into a .cbz-archive.

## Basic usage

This is a C# script written using [dotnet script](https://github.com/filipw/dotnet-script). It can be run from any CLI on any platform, or just as easily through [Visual Studio Code](https://code.visualstudio.com).

### Installing dotnet script

First, make sure [.Net Core](https://www.microsoft.com/net/download/core) is installed. 

Then install [dotnet script](https://github.com/filipw/dotnet-script).

### Running the script

Before the first run, create a copy of `config.default.json` and rename the copy to `config.json`. Fill inn the values as needed:

```json
{
  "ComicName": "[The name of the Comic]",
  "AddressOfFirstComic": "[comics.io address of the first strip you want]",
  "LoginCookie": "[sessionid=SESSION; csrftoken=CSRFTOKEN]"
}
```

Run the script using this command:

```shell
dotnet script comicdownloader.csx
```

As the script runs, it will create the following directories and files:

```shell
.
├── comics
│   └── [ComicName]
│       └── originals
│           └── strip-1.img
│           └── ...
│           └── strip-n.img
├── [ComicName].dat
```

The strips are prefixed with the publication date, followed by the comic name and its number in the sequence, for example, `20090106-Rocky-00683.gif`.

If the script is interrupted or you stop it manually, it will remember its progress and continue from the last downloaded comic. Otherwise, the script will download all the strips it finds of the given comic.

## Advanced usage with arguments

Arguments in dotnet scripts are passed using `--`. Comics Downloader supports the following arguments:

```shell
dotnet script comicdownloader.csx -- m a
```

### m - Optimize images (macOS only)

`m` uses [ImageOptim](https://imageoptim.com/mac) to optimise the comic strips after download. This will happen continuously as each image is downloaded. If you specify `m` without ImageOptim installed or on another os than macOS, the script will fail.

### a - Archive to CBZ

In addition to download the strips normally, `a` archives the comic strips into a [.cbz-file](https://www.howtogeek.com/291936/what-are-cbr-and-cbz-files-and-why-are-they-used-for-comics/). CBZ is a file format for comics and can be read by any modern comic reader app on any platform.

```shell
.
├── comics
│   └── [ComicName]
│       └── [ComicName].cbz
```


 
 
