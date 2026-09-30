namespace MimeTests;

public class GuessExtension
{
    [Fact]
    public void GuessExtension_FilePath_ReturnsJpeg()
    {
        // Arrange
        var expected = "jpeg";

        // Act
        string actual = MimeGuesser.GuessExtension(ResourceUtils.GetJpegFileFixture);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void GuessExtension_Buffer_ReturnsJpeg()
    {
        // Arrange
        byte[] buffer = File.ReadAllBytes(ResourceUtils.GetJpegFileFixture);
        var expected = "jpeg";

        // Act
        string actual = MimeGuesser.GuessExtension(buffer);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void GuessExtension_Stream_ReturnsJpeg()
    {
        // Arrange
        using var stream = File.OpenRead(ResourceUtils.GetJpegFileFixture);
        var expected = "jpeg";

        // Act
        string actual = MimeGuesser.GuessExtension(stream);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void GuessExtension_FileInfo_ReturnsJpeg()
    {
        // Arrange
        var expected = "jpeg";
        var fi = new FileInfo(ResourceUtils.GetJpegFileFixture);

        // Act
        string actual = fi.GuessExtension();

        // Assert
        Assert.Equal(expected, actual);
    }
}
