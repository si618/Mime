namespace HeyRed.Mime;

internal static partial class MagicNative
{
    private const string MagicLibPath = "libmagic-1";

    [LibraryImport(MagicLibPath, EntryPoint = "magic_open")]
    public static partial IntPtr MagicOpen(MagicOpenFlags flags);

    [LibraryImport(MagicLibPath, EntryPoint = "magic_load", StringMarshalling = StringMarshalling.Utf8)]
    public static partial int MagicLoad(IntPtr magic_cookie, string? dbPath);

    [LibraryImport(MagicLibPath, EntryPoint = "magic_close")]
    public static partial void MagicClose(IntPtr magic_cookie);

    [LibraryImport(MagicLibPath, EntryPoint = "magic_file", StringMarshalling = StringMarshalling.Utf8)]
    public static partial IntPtr MagicFile(IntPtr magic_cookie, string? dbPath);

    [LibraryImport(MagicLibPath, EntryPoint = "magic_buffer")]
    public static partial IntPtr MagicBuffer(IntPtr magic_cookie, byte[] buffer, int length);

    [LibraryImport(MagicLibPath, EntryPoint = "magic_error")]
    public static partial IntPtr MagicError(IntPtr magic_cookie);

    [LibraryImport(MagicLibPath, EntryPoint = "magic_getflags")]
    public static partial MagicOpenFlags MagicGetFlags(IntPtr magic_cookie);

    [LibraryImport(MagicLibPath, EntryPoint = "magic_setflags")]
    public static partial int MagicSetFlags(IntPtr magic_cookie, MagicOpenFlags flags);

    [LibraryImport(MagicLibPath, EntryPoint = "magic_check", StringMarshalling = StringMarshalling.Utf8)]
    public static partial int MagicCheck(IntPtr magic_cookie, string? dbPath);

    [LibraryImport(MagicLibPath, EntryPoint = "magic_compile", StringMarshalling = StringMarshalling.Utf8)]
    public static partial int MagicCompile(IntPtr magic_cookie, string? dbPath);

    [LibraryImport(MagicLibPath, EntryPoint = "magic_getparam")]
    public static partial int MagicGetParam(IntPtr magic_cookie, MagicParams param, out int value);

    [LibraryImport(MagicLibPath, EntryPoint = "magic_setparam")]
    public static partial int MagicSetParam(IntPtr magic_cookie, MagicParams param, ref int value);

    [LibraryImport(MagicLibPath, EntryPoint = "magic_list", StringMarshalling = StringMarshalling.Utf8)]
    public static partial int MagicList(IntPtr magic_cookie, string? dbPath);

    [LibraryImport(MagicLibPath, EntryPoint = "magic_version")]
    public static partial int MagicVersion();
}