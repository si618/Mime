namespace HeyRed.Mime;

/// <summary>
/// Static "facade" for <see cref="Magic"/>
/// </summary>
public static class MimeGuesser
{
    /// <summary>
    /// Path to libmagic database file.
    /// </summary>
    public static string? MagicFilePath { get; set; }

    /// <summary>
    /// Libmagic open flags for getting file type
    /// </summary>
    private static readonly MagicOpenFlags _magicMimeFlags =
        MagicOpenFlags.MAGIC_ERROR |
        MagicOpenFlags.MAGIC_MIME_TYPE |
        MagicOpenFlags.MAGIC_NO_CHECK_COMPRESS |
        MagicOpenFlags.MAGIC_NO_CHECK_ELF |
        MagicOpenFlags.MAGIC_NO_CHECK_APPTYPE;

    /// <summary>
    /// Get mime type from file.
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns>Mime type as string</returns>
    public static string GuessMimeType(string filePath)
    {
        ArgumentNullException.ThrowIfNull(filePath);

        using var magic = new Magic(_magicMimeFlags, MagicFilePath);
        return magic.Read(filePath);
    }

    /// <summary>
    /// Get mime type from bytes buffer.
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns>Mime type as string</returns>
    public static string GuessMimeType(byte[] buffer)
    {
        ArgumentNullException.ThrowIfNull(buffer);

        using var magic = new Magic(_magicMimeFlags, MagicFilePath);
        return magic.Read(buffer, buffer.Length);
    }

    /// <summary>
    /// Get mime type from stream.
    /// </summary>
    /// <param name="stream"></param>
    /// <returns>Mime type as string</returns>
    public static string GuessMimeType(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        using var magic = new Magic(_magicMimeFlags, MagicFilePath);
        return magic.Read(stream, 1048576);
    }

    /// <summary>
    /// Get file extension from path.
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns>Extension as string</returns>
    public static string GuessExtension(string filePath) => MimeTypesMap.GetExtension(GuessMimeType(filePath));

    /// <summary>
    /// Get file extension from bytes buffer.
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns>Extension as string</returns>
    public static string GuessExtension(byte[] buffer) => MimeTypesMap.GetExtension(GuessMimeType(buffer));

    /// <summary>
    /// Get file extension from stream.
    /// </summary>
    /// <param name="stream"></param>
    /// <returns>Extension as string</returns>
    public static string GuessExtension(Stream stream) => MimeTypesMap.GetExtension(GuessMimeType(stream));

    /// <summary>
    /// Get file type from path.
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns>FileType</returns>
    public static FileType GuessFileType(string filePath)
    {
        var mime = GuessMimeType(filePath);
        var ext = MimeTypesMap.GetExtension(mime);

        return new FileType(mime, ext);
    }

    /// <summary>
    /// Get file type from bytes buffer.
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns>FileType</returns>
    public static FileType GuessFileType(byte[] buffer)
    {
        var mime = GuessMimeType(buffer);
        var ext = MimeTypesMap.GetExtension(mime);

        return new FileType(mime, ext);
    }

    /// <summary>
    /// Get file type from stream.
    /// </summary>
    /// <param name="stream"></param>
    /// <returns>FileType</returns>
    public static FileType GuessFileType(Stream stream)
    {
        var mime = GuessMimeType(stream);
        var ext = MimeTypesMap.GetExtension(mime);

        return new FileType(mime, ext);
    }

    /// <param name="fi">File to inspect.</param>
    extension(FileInfo fi)
    {
        /// <summary>
        /// <see cref="GuessMimeType(string)"/>
        /// </summary>
        /// <returns>Mime type as string</returns>
        public string GuessMimeType() => MimeGuesser.GuessMimeType(fi.FullName);

        /// <summary>
        /// <see cref="GuessExtension(string)"/>
        /// </summary>
        /// <returns>Extension as string</returns>
        public string GuessExtension() => MimeGuesser.GuessExtension(fi.FullName);

        /// <summary>
        /// <see cref="GuessFileType(string)"/>
        /// </summary>
        /// <returns>FileType</returns>
        public FileType GuessFileType() => MimeGuesser.GuessFileType(fi.FullName);
    }
}
