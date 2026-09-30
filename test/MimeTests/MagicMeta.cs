namespace MimeTests;

public class MagicMeta
{
    [Fact]
    public void Version_Default_ReturnsBundledLibmagicVersion()
    {
        // Arrange
        var expected = 548;

        // Act
        int actual = Magic.Version;

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void GetFlags_OpenedWithMimeType_ReturnsMimeType()
    {
        // Arrange
        using var magic = new Magic(MagicOpenFlags.MAGIC_MIME_TYPE);

        // Act
        MagicOpenFlags actual = magic.GetFlags();

        // Assert
        Assert.Equal(MagicOpenFlags.MAGIC_MIME_TYPE, actual);
    }

    [Fact]
    public void SetFlags_CombinedMimeFlags_GetFlagsReturnsThem()
    {
        // Arrange
        var flags = MagicOpenFlags.MAGIC_MIME_TYPE | MagicOpenFlags.MAGIC_MIME_ENCODING;
        using var magic = new Magic(MagicOpenFlags.MAGIC_NONE);

        // Act
        magic.SetFlags(flags);

        // Assert
        Assert.Equal(flags, magic.GetFlags());
    }

    [Fact]
    public void CheckDatabase_DefaultDatabase_DoesNotThrow()
    {
        // Arrange
        using var magic = new Magic(MagicOpenFlags.MAGIC_NONE);

        // Act & Assert
        magic.CheckDatabase();
    }

    [Fact]
    public void ListDatabase_DoesNotThrow()
    {
        // Arrange
        using var magic = new Magic(MagicOpenFlags.MAGIC_NONE);

        // Act & Assert
        magic.ListDatabase();
    }

    [Fact]
    public void GetParam_NameMax_ReturnsDefault()
    {
        // Arrange
        using var magic = new Magic(MagicOpenFlags.MAGIC_NONE);

        // Act
        int value = magic.GetParam(MagicParams.MAGIC_PARAM_NAME_MAX);

        // Assert
        Assert.Equal(150, value);
    }

    [Fact]
    public void SetParam_NameMax_GetParamReturnsNewValue()
    {
        // Arrange
        int expected = 20;
        using var magic = new Magic(MagicOpenFlags.MAGIC_NONE);

        // Act
        magic.SetParam(MagicParams.MAGIC_PARAM_NAME_MAX, expected);
        int value = magic.GetParam(MagicParams.MAGIC_PARAM_NAME_MAX);

        // Assert
        Assert.Equal(expected, value);
    }
}
