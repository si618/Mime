namespace MimeTests;

public class MagicInputValidation : IDisposable
{
    private readonly Magic _magic;

    public MagicInputValidation()
    {
        _magic = new Magic(
            MagicOpenFlags.MAGIC_ERROR |
            MagicOpenFlags.MAGIC_MIME_TYPE);
    }

    public void Dispose() => _magic.Dispose();

    [Fact]
    public void ReadBuffer_NullBuffer_ThrowsArgumentNullException()
    {
        // Arrange
        byte[] buffer = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _magic.Read(buffer, 0));
    }

    [Fact]
    public void ReadBuffer_NegativeBufferSize_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var buffer = new byte[] { 0x00 };

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => _magic.Read(buffer, -1));
    }

    [Fact]
    public void ReadBuffer_ZeroBufferSize_ReturnsResult()
    {
        // Arrange
        var buffer = new byte[] { 0xFF, 0xD8, 0xFF };

        // Act
        string result = _magic.Read(buffer, 0);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void ReadBuffer_BufferSizeLargerThanBuffer_ReturnsResult()
    {
        // Arrange
        byte[] buffer = File.ReadAllBytes(ResourceUtils.GetJpegFileFixture);

        // Act
        string result = _magic.Read(buffer, buffer.Length + 100);

        // Assert
        Assert.Equal("image/jpeg", result);
    }

    [Fact]
    public void ReadStream_NullStream_ThrowsArgumentNullException()
    {
        // Arrange
        Stream stream = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _magic.Read(stream, 1024));
    }

    [Fact]
    public void ReadStream_NonSeekableStream_ReturnsMimeType()
    {
        // Arrange
        using var fileStream = File.OpenRead(ResourceUtils.GetJpegFileFixture);
        using var nonSeekable = new NonSeekableStream(fileStream);

        // Act
        string result = _magic.Read(nonSeekable, 1048576);

        // Assert
        Assert.Equal("image/jpeg", result);
    }

    [Fact]
    public void ReadStream_NonSeekableStream_DoesNotThrowOnRewind()
    {
        // Arrange
        using var fileStream = File.OpenRead(ResourceUtils.GetTextFileFixture);
        using var nonSeekable = new NonSeekableStream(fileStream);

        // Act
        string result = _magic.Read(nonSeekable, 1048576);

        // Assert
        Assert.NotNull(result);
    }

    private sealed class NonSeekableStream : Stream
    {
        private readonly Stream _inner;

        public NonSeekableStream(Stream inner) => _inner = inner;

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => throw new NotSupportedException();
        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count) =>
            _inner.Read(buffer, offset, count);

        public override void Flush() => _inner.Flush();
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    }
}