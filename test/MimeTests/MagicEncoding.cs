using System.Text;

namespace MimeTests;

public class MagicEncoding
{
    [Fact]
    public void ReadBuffer_NonAsciiJpegComment_ReturnsCommentDecodedAsUtf8()
    {
        // Arrange
        // A minimal JFIF header followed by a COM segment; libmagic echoes the comment into its description,
        // and MAGIC_RAW stops it escaping the non-ASCII bytes.
        var comment = "テスト";
        byte[] text = Encoding.UTF8.GetBytes(comment);
        byte[] jpeg =
        [
            0xFF, 0xD8,
            0xFF, 0xE0, 0x00, 0x10, (byte)'J', (byte)'F', (byte)'I', (byte)'F', 0x00, 0x01, 0x01, 0x00, 0x00, 0x01, 0x00, 0x01, 0x00, 0x00,
            0xFF, 0xFE, 0x00, (byte)(text.Length + 2), .. text,
            0xFF, 0xD9,
        ];
        using var magic = new Magic(MagicOpenFlags.MAGIC_RAW);

        // Act
        string actual = magic.Read(jpeg, jpeg.Length);

        // Assert
        Assert.Contains($"comment: \"{comment}\"", actual, StringComparison.Ordinal);
    }
}
