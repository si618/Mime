using HeyRed.Mime;

using Xunit;

namespace MimeTests;

public class MagicMeta
{
    [Fact]
    public void CheckVersion() => Assert.Equal(547, Magic.Version);

    [Fact]
    public void GetFlags()
    {
        using var magic = new Magic(MagicOpenFlags.MAGIC_MIME_TYPE);

        Assert.Equal(MagicOpenFlags.MAGIC_MIME_TYPE, magic.GetFlags());
    }

    [Fact]
    public void SetFlags()
    {
        var flags = MagicOpenFlags.MAGIC_MIME_TYPE | MagicOpenFlags.MAGIC_MIME_ENCODING;

        using var magic = new Magic(MagicOpenFlags.MAGIC_NONE);
        magic.SetFlags(flags);

        Assert.Equal(flags, magic.GetFlags());
    }

    [Fact]
    public void ValidateDatabase()
    {
        using var magic = new Magic(MagicOpenFlags.MAGIC_NONE);

        magic.CheckDatabase();
    }

    [Fact]
    public void GetParams()
    {
        using var magic = new Magic(MagicOpenFlags.MAGIC_NONE);
        nint value = magic.GetParam(MagicParams.MAGIC_PARAM_NAME_MAX);

        Assert.Equal(150, value);
    }

    [Fact]
    public void SetParams()
    {
        nint expected = 20;

        using var magic = new Magic(MagicOpenFlags.MAGIC_NONE);

        magic.SetParam(MagicParams.MAGIC_PARAM_NAME_MAX, expected);
        nint value = magic.GetParam(MagicParams.MAGIC_PARAM_NAME_MAX);

        Assert.Equal(expected, value);
    }
}