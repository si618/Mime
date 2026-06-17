# Mime

.NET wrapper for libmagic

<!-- TODO: update badges — NuGet points to upstream, license link points to hey-red/Markdown -->
<!-- [![NuGet](https://img.shields.io/nuget/v/Mime.svg)](https://www.nuget.org/packages/Mime) -->
<!-- [![license](https://img.shields.io/github/license/mashape/apistatus.svg)](https://github.com/hey-red/Markdown/blob/master/LICENSE) -->

## Install

The [NuGet package on nuget.org](https://www.nuget.org/packages/Mime) is published by the upstream [hey-red/Mime](https://github.com/hey-red/Mime) repository.

This fork does not currently publish a package outside of the repository — build artifacts are available as downloads from GitHub Actions workflow runs.

## Requirements

Only current LTS versions of .NET are supported (net8.0 and net10.0).

Supported runtimes:

- linux-arm64
- linux-musl-arm64
- linux-musl-x64
- linux-x64
- osx-arm64
- osx-x64
- win-arm64 (not currently tested due to lack of GitHub runner)
- win-x64
- win-x86

## Basic usage

```C#
using HeyRed.Mime;

// (Optionally) You can set path to magic database file manually.
MimeGuesser.MagicFilePath = "/path/to/magic.mgc";

// Guess mime type of file(overloaded method takes byte array or stream as arg.)
MimeGuesser.GuessMimeType("path/to/file"); //=> image/jpeg

// Get extension of file(overloaded method takes byte array or stream as arg.)
MimeGuesser.GuessExtension("path/to/file"); //=> jpeg

// Get mime type and extension of file(overloaded method takes byte array or stream as arg.)
MimeGuesser.GuessFileType("path/to/file"); //=> FileType
```

## Advanced

Want more than just the mime type? Use the Magic class:

```C#
string calc = @"C:\Windows\System32\calc.exe";
using var magic = new Magic(MagicOpenFlags.MAGIC_NONE);
magic.Read(calc); //=> PE32+ executable (GUI) x86-64, for MS Windows

// Check encoding:
string textFile = @"F:\Temp\file.txt";
using var magic = new Magic(MagicOpenFlags.MAGIC_MIME_ENCODING);
magic.Read(textFile); //=> Output: utf-8
```

Also, we can combine flags with "|" operator.
See all [flags](src/Mime/MagicOpenFlags.cs) for more info.

## Remarks

- The Magic class is not thread safe, but if you use different instances on different threads it seems to work fine.
- The MimeGuesser is thread safe, since it generates a new instance of Magic class on each use.

## Building

```sh
git clone https://github.com/si618/Mime.git
cd Mime
dotnet build
dotnet test --no-build
dotnet pack -c Release -o nupkg
```

## Possible problems

| Exception                                              | Solution                                                                                                                                                                                                                         |
| :----------------------------------------------------- | :------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| DllNotFoundException                                   | Make sure that your `bin` folder contains runtimes directory. If you publishing platform dependent app, then `bin` should be contains `libmagic-1`(.dll, .so or .dylib) and `magic.mgc` files.                                   |
| BadImageFormatException                                | Try targeting `x64` or `arm64` instead of `AnyCPU`.                                                                                                                                                                             |
| MagicException: Could not find any valid magic files!  | Make sure your magic.mgc file contains in one of /runtimes/ subdirs or along with libmagic-1.[dll\|lib\|dylib]. Or set path to custom database as described in [basic usage](#basic-usage).       |

## License

[MIT](LICENSE)
