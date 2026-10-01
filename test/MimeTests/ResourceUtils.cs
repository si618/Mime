using System.IO.Compression;

namespace MimeTests;

public static class ResourceUtils
{
    public static string GetJpegFileFixture =>
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "test.jpeg");

    public static string GetTextFileFixture =>
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", ".editorconfig");

    /// <summary>
    /// Creates a ZIP archive holding one zero-filled entry, stored uncompressed so the archive size
    /// tracks <paramref name="entryBytes"/>.
    /// </summary>
    public static byte[] CreateStoredZip(int entryBytes)
    {
        using var stream = new MemoryStream();
        using (var archive = new ZipArchive(stream, ZipArchiveMode.Create, leaveOpen: true))
        {
            using var entryStream = archive.CreateEntry("entry.bin", CompressionLevel.NoCompression).Open();
            entryStream.Write(new byte[entryBytes]);
        }

        return stream.ToArray();
    }
}
