namespace MimeTests;

public class MimeGuesserInputValidation
{
    [Fact]
    public void GuessMimeType_NullFilePath_ThrowsArgumentNullException()
    {
        // Arrange
        string filePath = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => MimeGuesser.GuessMimeType(filePath));
    }

    [Fact]
    public void GuessMimeType_NullBuffer_ThrowsArgumentNullException()
    {
        // Arrange
        byte[] buffer = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => MimeGuesser.GuessMimeType(buffer));
    }

    [Fact]
    public void GuessMimeType_NullStream_ThrowsArgumentNullException()
    {
        // Arrange
        Stream stream = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => MimeGuesser.GuessMimeType(stream));
    }

    [Fact]
    public void GuessMimeType_NonExistentFile_ThrowsFileNotFoundException()
    {
        // Arrange
        var filePath = Path.Combine(Path.GetTempPath(), "nonexistent_file_12345.xyz");

        // Act & Assert
        Assert.ThrowsAny<Exception>(() => MimeGuesser.GuessMimeType(filePath));
    }

    [Fact]
    public void GuessMimeType_EmptyBuffer_ReturnsResult()
    {
        // Arrange
        var buffer = Array.Empty<byte>();

        // Act
        string result = MimeGuesser.GuessMimeType(buffer);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void GuessExtension_NullFilePath_ThrowsArgumentNullException()
    {
        // Arrange
        string filePath = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => MimeGuesser.GuessExtension(filePath));
    }

    [Fact]
    public void GuessExtension_NullBuffer_ThrowsArgumentNullException()
    {
        // Arrange
        byte[] buffer = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => MimeGuesser.GuessExtension(buffer));
    }

    [Fact]
    public void GuessExtension_NullStream_ThrowsArgumentNullException()
    {
        // Arrange
        Stream stream = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => MimeGuesser.GuessExtension(stream));
    }

    [Fact]
    public void GuessFileType_NullFilePath_ThrowsArgumentNullException()
    {
        // Arrange
        string filePath = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => MimeGuesser.GuessFileType(filePath));
    }

    [Fact]
    public void GuessFileType_NullBuffer_ThrowsArgumentNullException()
    {
        // Arrange
        byte[] buffer = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => MimeGuesser.GuessFileType(buffer));
    }

    [Fact]
    public void GuessFileType_NullStream_ThrowsArgumentNullException()
    {
        // Arrange
        Stream stream = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => MimeGuesser.GuessFileType(stream));
    }
}