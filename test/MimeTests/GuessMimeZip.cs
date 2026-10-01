namespace MimeTests;

/// <summary>
/// Regression tests for hey-red/Mime#62, where ZIP archives were reported as application/octet-stream:
/// from a buffer or stream at any size, and from a file path once the file was larger than libmagic's
/// default 7 MiB scan limit (MAGIC_PARAM_BYTES_MAX).
/// </summary>
public sealed class GuessMimeZip : IDisposable
{
    private const string ZipMimeType = "application/zip";
    private const int SmallEntryBytes = 1;

    // Smallest size that reliably reproduced the file path failure, with 1 MiB of margin over the
    // 7 MiB scan limit. The entry is stored uncompressed so the archive is as large as the entry.
    private const int EntryBytesBeyondScanLimit = 8 * 1024 * 1024;

    private readonly string _tempDir;

    public GuessMimeZip()
    {
        // Unique per instance, as each target framework's test process runs concurrently under MTP
        _tempDir = Path.Combine(Path.GetTempPath(), $"MimeTests_Zip_{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
        {
            Directory.Delete(_tempDir, recursive: true);
        }
    }

    [Theory]
    [InlineData(SmallEntryBytes)]
    [InlineData(EntryBytesBeyondScanLimit)]
    public void GuessMimeFromFilePath_ZipFile_ReturnsZip(int entryBytes)
    {
        // Arrange
        var path = Path.Combine(_tempDir, "test.zip");
        File.WriteAllBytes(path, ResourceUtils.CreateStoredZip(entryBytes));

        // Act
        string actual = MimeGuesser.GuessMimeType(path);

        // Assert
        Assert.Equal(ZipMimeType, actual);
    }

    [Theory]
    [InlineData(SmallEntryBytes)]
    [InlineData(EntryBytesBeyondScanLimit)]
    public void GuessMimeFromBuffer_ZipBytes_ReturnsZip(int entryBytes)
    {
        // Arrange
        byte[] buffer = ResourceUtils.CreateStoredZip(entryBytes);

        // Act
        string actual = MimeGuesser.GuessMimeType(buffer);

        // Assert
        Assert.Equal(ZipMimeType, actual);
    }

    [Theory]
    [InlineData(SmallEntryBytes)]
    [InlineData(EntryBytesBeyondScanLimit)]
    public void GuessMimeFromStream_ZipStream_ReturnsZip(int entryBytes)
    {
        // Arrange
        using var stream = new MemoryStream(ResourceUtils.CreateStoredZip(entryBytes));

        // Act
        string actual = MimeGuesser.GuessMimeType(stream);

        // Assert
        Assert.Equal(ZipMimeType, actual);
    }
}
