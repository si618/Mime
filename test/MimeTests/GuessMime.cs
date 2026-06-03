using HeyRed.Mime;

using System.IO;
using System.IO.Compression;

using Xunit;

namespace MimeTests;

public class GuessMime
{
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
    public void GuessMimeFromBuffer_ZipFile_ReturnsApplicationZip()
    {
        // Arrange
        using var ms = new MemoryStream();
        using (var archive = new ZipArchive(ms, ZipArchiveMode.Create, leaveOpen: true))
        {
            var entry = archive.CreateEntry("test.txt");
            using var writer = new StreamWriter(entry.Open());
            writer.Write("Hello, World!");
        }
        byte[] buffer = ms.ToArray();

        // Act
        string actual = MimeGuesser.GuessMimeType(buffer);

        // Assert
        Assert.Equal("application/zip", actual);
    }

    [Fact]
    public void GuessMimeFromStream_ZipFile_ReturnsApplicationZip()
    {
        // Arrange
        using var ms = new MemoryStream();
        using (var archive = new ZipArchive(ms, ZipArchiveMode.Create, leaveOpen: true))
        {
            var entry = archive.CreateEntry("test.txt");
            using var writer = new StreamWriter(entry.Open());
            writer.Write("Hello, World!");
        }
        ms.Position = 0;

        // Act
        string actual = MimeGuesser.GuessMimeType(ms);

        // Assert
        Assert.Equal("application/zip", actual);
    }

    [Fact]
    public void GuessMimeFromFilePath_ZipFile_ReturnsApplicationZip()
    {
        // Arrange
        var tempPath = Path.GetTempFileName() + ".zip";
        try
        {
            using (var fs = File.Create(tempPath))
            using (var archive = new ZipArchive(fs, ZipArchiveMode.Create))
            {
                var entry = archive.CreateEntry("test.txt");
                using var writer = new StreamWriter(entry.Open());
                writer.Write("Hello, World!");
            }

            // Act
            string actual = MimeGuesser.GuessMimeType(tempPath);

            // Assert
            Assert.Equal("application/zip", actual);
        }
        finally
        {
            File.Delete(tempPath);
        }
    }
}