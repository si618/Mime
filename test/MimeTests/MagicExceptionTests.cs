namespace MimeTests;

public class MagicExceptionTests
{
    [Fact]
    public void Constructor_Default_CreatesException()
    {
        // Arrange & Act
        var ex = new MagicException();

        // Assert
        Assert.NotNull(ex);
    }

    [Fact]
    public void Constructor_WithMessage_SetsMessage()
    {
        // Arrange & Act
        var ex = new MagicException("test error");

        // Assert
        Assert.Equal("test error", ex.Message);
    }

    [Fact]
    public void Constructor_WithMessageAndAdditionalInfo_PrefersMessage()
    {
        // Arrange & Act
        var ex = new MagicException("primary error", "fallback info");

        // Assert
        Assert.Equal("primary error", ex.Message);
    }

    [Fact]
    public void Constructor_WithNullMessageAndAdditionalInfo_UsesFallback()
    {
        // Arrange & Act
        var ex = new MagicException(null!, "fallback info");

        // Assert
        Assert.Equal("fallback info", ex.Message);
    }

    [Fact]
    public void Constructor_WithInnerException_SetsInnerException()
    {
        // Arrange
        var inner = new InvalidOperationException("inner");

        // Act
        var ex = new MagicException("outer", inner);

        // Assert
        Assert.Equal("outer", ex.Message);
        Assert.Same(inner, ex.InnerException);
    }
}