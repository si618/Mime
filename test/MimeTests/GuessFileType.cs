namespace MimeTests;

public class GuessFileType
{
    [Fact]
    public void GuessFileType_FilePath_ReturnsJpegFileType()
    {
        // Arrange
        var expected = new FileType("image/jpeg", "jpeg");

        // Act
        FileType actual = MimeGuesser.GuessFileType(ResourceUtils.GetJpegFileFixture);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void GuessFileType_Buffer_ReturnsJpegFileType()
    {
        // Arrange
        byte[] buffer = File.ReadAllBytes(ResourceUtils.GetJpegFileFixture);
        var expected = new FileType("image/jpeg", "jpeg");

        // Act
        FileType actual = MimeGuesser.GuessFileType(buffer);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void GuessFileType_Stream_ReturnsJpegFileType()
    {
        // Arrange
        using var stream = File.OpenRead(ResourceUtils.GetJpegFileFixture);
        var expected = new FileType("image/jpeg", "jpeg");

        // Act
        FileType actual = MimeGuesser.GuessFileType(stream);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void GuessFileType_FileInfo_ReturnsJpegFileType()
    {
        // Arrange
        var expected = new FileType("image/jpeg", "jpeg");
        var fi = new FileInfo(ResourceUtils.GetJpegFileFixture);

        // Act
        FileType actual = fi.GuessFileType();

        // Assert
        Assert.Equal(expected, actual);
    }
}
