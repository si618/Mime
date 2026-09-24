# Mime

.NET wrapper for libmagic

[![NuGet](https://img.shields.io/nuget/v/MimeMagic.svg)](https://www.nuget.org/packages/MimeMagic)
[![license](https://img.shields.io/github/license/si618/Mime.svg)](LICENSE)

## Install

```sh
dotnet add package MimeMagic
```

[MimeMagic](https://www.nuget.org/packages/MimeMagic) is published to nuget.org from this repository, a fork of [hey-red/Mime](https://github.com/hey-red/Mime) that keeps the bundled libmagic native binaries up to date.

Only the package name differs from upstream. The assembly is still `Mime` and the namespace is still `HeyRed.Mime`, so switching from the `Mime` package to `MimeMagic` needs no code changes. Don't reference both packages in the same project, because their assemblies and native assets conflict.

## Requirements

Supported .NET versions are the current LTS releases (net8.0 and net10.0), plus the latest STS release when it is newer than the latest LTS. Versions are dropped once they reach end of support.

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
dotnet run --project samples/MimeExample/MimeExample.csproj
dotnet pack -c Release -o nupkg
```

## Releasing

MimeMagic is published to nuget.org by the [Pack workflow](.github/workflows/pack.yml), which runs only when a `v*` tag is pushed. Pushes and pull requests to `master` only build and test (see [CI](.github/workflows/ci.yml)).

### Publishing a release

1. Make sure `master` is green in CI and contains everything intended for the release.
2. Pick the version following [SemVer](https://semver.org/). The package version comes from the tag, not from `<Version>` in `src/Mime/Mime.csproj`, which is only the default for local builds. Keep it in step with the tag anyway.
3. Tag the `master` commit and push the tag:

   ```sh
   git switch master
   git pull
   git tag -a v4.0.0 -m "MimeMagic 4.0.0"
   git push origin v4.0.0
   ```

   A pre-release tag such as `v4.1.0-beta.1` publishes a pre-release package.

4. Watch the Pack run under the repository's Actions tab. It runs the tests on net8.0 and net10.0, packs with the version taken from the tag (`v4.0.0` becomes `4.0.0`), and pushes the `.nupkg` and `.snupkg` to nuget.org. The packages are also attached to the run as an artifact.
5. The new version appears on the [MimeMagic package page](https://www.nuget.org/packages/MimeMagic) after nuget.org finishes validating and indexing it, which usually takes a few minutes.

Versions on nuget.org are immutable, so a published version can't be replaced. To fix a bad release, unlist it on nuget.org and tag a new version. Re-pushing an existing version is skipped (`--skip-duplicate`).

### Publishing setup

Publishing uses [nuget.org Trusted Publishing](https://learn.microsoft.com/nuget/nuget-org/trusted-publishing). The workflow exchanges its GitHub OIDC token for a short-lived API key, so no long-lived key is stored. This is already configured, and only needs redoing if the repository, the workflow file name or the nuget.org owner changes:

- **nuget.org Trusted Publishing policy:** owner `si-fi`, repository owner `si618`, repository `Mime`, workflow file `pack.yml`, no environment, scoped to the package `MimeMagic`.
- **GitHub repository secret `NUGET_USER`:** the nuget.org username (`si-fi`). This must be a repository secret, not an environment secret, because the Pack job doesn't declare a GitHub environment.

## Possible problems

| Exception                                             | Solution                                                                                                                                                                       |
| :---------------------------------------------------- | :----------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| DllNotFoundException                                  | Make sure your `bin` folder contains the runtimes directory. If publishing a platform-dependent app, `bin` should contain `libmagic-1` (.dll, .so, or .dylib) and `magic.mgc`. |
| BadImageFormatException                               | Try targeting `x64` or `arm64` instead of `AnyCPU`.                                                                                                                            |
| MagicException: Could not find any valid magic files! | Make sure `magic.mgc` is in one of the `/runtimes/` subdirs or alongside `libmagic-1.[dll\|lib\|dylib]`. Or set a custom path as described in [basic usage](#basic-usage).     |

## License

[MIT](LICENSE)
