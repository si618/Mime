namespace MimeTests;

public class MagicDispose
{
    [Fact]
    public void Read_AfterDispose_ThrowsObjectDisposedException()
    {
        // Arrange
        var magic = new Magic(MagicOpenFlags.MAGIC_MIME_TYPE);
        magic.Dispose();

        // Act & Assert
        Assert.Throws<ObjectDisposedException>(() => magic.Read(ResourceUtils.GetJpegFileFixture));
    }

    [Fact]
    public void ReadBuffer_AfterDispose_ThrowsObjectDisposedException()
    {
        // Arrange
        var magic = new Magic(MagicOpenFlags.MAGIC_MIME_TYPE);
        magic.Dispose();
        var buffer = new byte[] { 0xFF, 0xD8, 0xFF };

        // Act & Assert
        Assert.Throws<ObjectDisposedException>(() => magic.Read(buffer, buffer.Length));
    }

    [Fact]
    public void ReadStream_AfterDispose_ThrowsObjectDisposedException()
    {
        // Arrange
        var magic = new Magic(MagicOpenFlags.MAGIC_MIME_TYPE);
        magic.Dispose();
        using var stream = File.OpenRead(ResourceUtils.GetJpegFileFixture);

        // Act & Assert
        Assert.Throws<ObjectDisposedException>(() => magic.Read(stream, 1024));
    }

    [Fact]
    public void GetFlags_AfterDispose_ThrowsObjectDisposedException()
    {
        // Arrange
        var magic = new Magic(MagicOpenFlags.MAGIC_MIME_TYPE);
        magic.Dispose();

        // Act & Assert
        Assert.Throws<ObjectDisposedException>(() => magic.GetFlags());
    }

    [Fact]
    public void SetFlags_AfterDispose_ThrowsObjectDisposedException()
    {
        // Arrange
        var magic = new Magic(MagicOpenFlags.MAGIC_MIME_TYPE);
        magic.Dispose();

        // Act & Assert
        Assert.Throws<ObjectDisposedException>(() => magic.SetFlags(MagicOpenFlags.MAGIC_NONE));
    }

    [Fact]
    public void GetParam_AfterDispose_ThrowsObjectDisposedException()
    {
        // Arrange
        var magic = new Magic(MagicOpenFlags.MAGIC_MIME_TYPE);
        magic.Dispose();

        // Act & Assert
        Assert.Throws<ObjectDisposedException>(() => magic.GetParam(MagicParams.MAGIC_PARAM_NAME_MAX));
    }

    [Fact]
    public void SetParam_AfterDispose_ThrowsObjectDisposedException()
    {
        // Arrange
        var magic = new Magic(MagicOpenFlags.MAGIC_MIME_TYPE);
        magic.Dispose();

        // Act & Assert
        Assert.Throws<ObjectDisposedException>(() => magic.SetParam(MagicParams.MAGIC_PARAM_NAME_MAX, 50));
    }

    [Fact]
    public void CheckDatabase_AfterDispose_ThrowsObjectDisposedException()
    {
        // Arrange
        var magic = new Magic(MagicOpenFlags.MAGIC_MIME_TYPE);
        magic.Dispose();

        // Act & Assert
        Assert.Throws<ObjectDisposedException>(() => magic.CheckDatabase());
    }

    [Fact]
    public void CompileDatabase_AfterDispose_ThrowsObjectDisposedException()
    {
        // Arrange
        var magic = new Magic(MagicOpenFlags.MAGIC_MIME_TYPE);
        magic.Dispose();

        // Act & Assert
        Assert.Throws<ObjectDisposedException>(() => magic.CompileDatabase());
    }

    [Fact]
    public void ListDatabase_AfterDispose_ThrowsObjectDisposedException()
    {
        // Arrange
        var magic = new Magic(MagicOpenFlags.MAGIC_MIME_TYPE);
        magic.Dispose();

        // Act & Assert
        Assert.Throws<ObjectDisposedException>(() => magic.ListDatabase());
    }

    [Fact]
    public void Dispose_CalledTwice_DoesNotThrow()
    {
        // Arrange
        var magic = new Magic(MagicOpenFlags.MAGIC_MIME_TYPE);

        // Act & Assert
        magic.Dispose();
        magic.Dispose();
    }
}