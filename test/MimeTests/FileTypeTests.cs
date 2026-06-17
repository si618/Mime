namespace MimeTests;

public class FileTypeTests
{
    [Fact]
    public void Constructor_SetsProperties()
    {
        // Arrange & Act
        var fileType = new FileType("image/jpeg", "jpeg");

        // Assert
        Assert.Equal("image/jpeg", fileType.MimeType);
        Assert.Equal("jpeg", fileType.Extension);
    }

    [Fact]
    public void Deconstruct_ReturnsComponents()
    {
        // Arrange
        var fileType = new FileType("text/plain", "txt");

        // Act
        var (mimeType, extension) = fileType;

        // Assert
        Assert.Equal("text/plain", mimeType);
        Assert.Equal("txt", extension);
    }

    [Fact]
    public void Equals_SameValues_ReturnsTrue()
    {
        // Arrange
        var a = new FileType("image/png", "png");
        var b = new FileType("image/png", "png");

        // Act & Assert
        Assert.Equal(a, b);
    }

    [Fact]
    public void Equals_DifferentMimeType_ReturnsFalse()
    {
        // Arrange
        var a = new FileType("image/png", "png");
        var b = new FileType("image/jpeg", "png");

        // Act & Assert
        Assert.NotEqual(a, b);
    }

    [Fact]
    public void Equals_DifferentExtension_ReturnsFalse()
    {
        // Arrange
        var a = new FileType("image/png", "png");
        var b = new FileType("image/png", "jpg");

        // Act & Assert
        Assert.NotEqual(a, b);
    }
}