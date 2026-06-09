using System;
using System.IO;

using HeyRed.Mime;

using Xunit;

namespace MimeTests;

public class GuessMime : IDisposable
{
    private readonly string _unicodeTempDir;

    public GuessMime()
    {
        _unicodeTempDir = Path.Combine(Path.GetTempPath(), "MimeTests_إظهار_テスト");
        Directory.CreateDirectory(_unicodeTempDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_unicodeTempDir))
        {
            Directory.Delete(_unicodeTempDir, recursive: true);
        }
    }

    [Fact]
    public void GuessMimeFromFilePath_UnicodeCharacters_ReturnsMimeType()
    {
        // Arrange
        var unicodePath = Path.Combine(_unicodeTempDir, "إظهار لقطات الشاشة.jpeg");
        File.Copy(ResourceUtils.GetJpegFileFixture, unicodePath, overwrite: true);
        var expected = "image/jpeg";

        // Act
        string actual = MimeGuesser.GuessMimeType(unicodePath);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void GuessMimeFromFileInfo_UnicodeCharacters_ReturnsMimeType()
    {
        // Arrange
        var unicodePath = Path.Combine(_unicodeTempDir, "テスト画像.jpeg");
        File.Copy(ResourceUtils.GetJpegFileFixture, unicodePath, overwrite: true);
        var expected = "image/jpeg";

        // Act
        var fi = new FileInfo(unicodePath);
        string actual = fi.GuessMimeType();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void GuessMimeFromFilePath()
    {
        var expected = "image/jpeg";
        string actual = MimeGuesser.GuessMimeType(ResourceUtils.GetJpegFileFixture);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void GuessMimeFromBuffer()
    {
        byte[] buffer = File.ReadAllBytes(ResourceUtils.GetJpegFileFixture);
        var expected = "image/jpeg";
        string actual = MimeGuesser.GuessMimeType(buffer);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void GuessMimeFromStream()
    {
        using var stream = File.OpenRead(ResourceUtils.GetJpegFileFixture);
        var expected = "image/jpeg";
        string actual = MimeGuesser.GuessMimeType(stream);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void GuessMimeFromSmallTextStream()
    {
        using var stream = File.OpenRead(ResourceUtils.GetTextFileFixture);
        var expected = "text/plain";
        string actual = MimeGuesser.GuessMimeType(stream);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void GuessMimeFromFileInfo()
    {
        var expected = "image/jpeg";
        var fi = new FileInfo(ResourceUtils.GetJpegFileFixture);
        string actual = fi.GuessMimeType();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void GuessMimeFromStream_RewindsSeekableStream()
    {
        // Arrange
        using var stream = File.OpenRead(ResourceUtils.GetJpegFileFixture);

        // Act
        MimeGuesser.GuessMimeType(stream);

        // Assert
        Assert.Equal(0, stream.Position);
    }

    [Fact]
    public void GuessMimeFromStream_SmallBuffer_ReturnsMimeType()
    {
        // Arrange
        using var stream = File.OpenRead(ResourceUtils.GetJpegFileFixture);
        using var magic = new Magic(
            MagicOpenFlags.MAGIC_ERROR |
            MagicOpenFlags.MAGIC_MIME_TYPE |
            MagicOpenFlags.MAGIC_NO_CHECK_COMPRESS |
            MagicOpenFlags.MAGIC_NO_CHECK_ELF |
            MagicOpenFlags.MAGIC_NO_CHECK_APPTYPE);

        // Act
        string actual = magic.Read(stream, 256);

        // Assert
        Assert.Equal("image/jpeg", actual);
    }
}