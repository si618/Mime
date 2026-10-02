# AGENTS.md

.NET P/Invoke wrapper for libmagic, published to nuget.org as `MimeMagic`. Fork of [hey-red/Mime](https://github.com/hey-red/Mime): keep the assembly name `Mime` and the namespace `HeyRed.Mime`.

## Layout

- `src/Mime`: library. `MagicNative.cs` holds the P/Invoke declarations.
- `src/Mime/runtimes/<rid>/native`: prebuilt libmagic binaries. `src/Mime/content/magic.mgc`: compiled magic database.
- `test/MimeTests`: xunit.v3 on Microsoft.Testing.Platform. Test files are in `TestData`.
- `samples/MimeExample`: console sample.
- `.github/workflows`: `ci.yml` (build and test), `build-libmagic.yml` (native binaries), `pack.yml` (release).

## Build and test

```sh
dotnet build Mime.slnx -c Release -warnaserror
dotnet test test/MimeTests/MimeTests.csproj
dotnet test test/MimeTests/MimeTests.csproj -p:Platform=ARM64   # or x86
```

- SDK comes from `global.json`. Target frameworks are in `Directory.Build.props`. Follow the README "Requirements" rule when adding or dropping one.
- Package versions are only in `Directory.Packages.props` (central package management).
- Output goes to `artifacts/` (`UseArtifactsOutput`), e.g. `artifacts/bin/MimeTests/debug_net10.0`, with `_arm64` or `_x86` appended for platform builds.
- Warnings are errors: `latest-recommended` analyzers, Meziantou.Analyzer, nullable.

## Tests

- Name tests `UnitOfWork_Scenario_ExpectedBehaviour`.
- Structure each test with `// Arrange`, `// Act` and `// Assert` comments.
- Fix a failing test by updating the test to match intended behaviour, not by changing library code to match an old test.

## Native binaries

- Don't build or commit binaries by hand. Dispatch `build-libmagic.yml` with `file_version` (a `file/file` tag such as `FILE5_48`). It builds all nine runtimes, tests all but win-arm64, and opens an `update-libmagic-<version>` PR.
- linux-x64 and linux-arm64 build in `manylinux_2_28` containers. All Linux builds use `LINUX_CONFIGURE_FLAGS`, which keeps zlib and disables every other compression library.
- glibc baseline: the oldest glibc of any distro on the supported-OS list of the oldest targeted .NET, currently 2.28 (RHEL 8). It's set in `.github/scripts/check-linux-deps.sh`, which also lists the allowed `NEEDED` libraries per runtime. It runs in `build-libmagic.yml` and on every PR.
- The Linux test jobs run on `ubuntu-22.04` and `ubuntu-22.04-arm` on purpose, and `renovate.json` keeps those runners from being updated.
- The Windows DLL set follows libmagic's imports (`.github/scripts/collect-mingw-dlls.sh`). Replace the whole set, don't merge into it.

## Releasing

- Push an annotated `v*` tag on `master`. The version comes from the tag. `pack.yml` tests, packs and publishes via nuget.org Trusted Publishing, then creates a GitHub release with generated notes.
- MinVer sets the version from the tag, so there's no `<Version>` in the csproj. Untagged commits build as `<next patch>-alpha.0.<height>`. `pack.yml` needs `fetch-depth: 0` to see the tags.
- nuget.org versions are immutable. Fix a bad release with a new version.
- Dropping a target framework or breaking the public API needs a major version.

## Conventions

- Branches have no prefix. PRs are squash-merged.
- Keep PR titles and descriptions short: list the changes and how they were tested.
- Format Markdown with `npx prettier --prose-wrap preserve --write <file>`. `.editorconfig` sets `indent_size = 4`, so prettier re-indents nested README lists; revert hunks outside the section you edited.
- Renovate keeps dependencies up to date. Pin GitHub Actions by commit SHA and container images by digest.
