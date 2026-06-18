// -----------------------------------------------------------------------
// 1. MimeGuesser — simple facade (new Magic instance per call, thread-safe)
// -----------------------------------------------------------------------

Console.WriteLine("=== MimeGuesser (facade) ===");
Console.WriteLine();

// From a file path
string sampleJpeg = Path.Combine(AppContext.BaseDirectory, "test.jpeg");
Console.WriteLine($"File path   → mime:      {MimeGuesser.GuessMimeType(sampleJpeg)}");
Console.WriteLine($"File path   → extension: {MimeGuesser.GuessExtension(sampleJpeg)}");

FileType fileType = MimeGuesser.GuessFileType(sampleJpeg);
Console.WriteLine($"File path   → FileType:  mime={fileType.MimeType}, ext={fileType.Extension}");

// Deconstruct FileType
var (mime, ext) = MimeGuesser.GuessFileType(sampleJpeg);
Console.WriteLine($"Deconstruct → mime={mime}, ext={ext}");

Console.WriteLine();

// From a byte buffer — minimal PNG (1×1 pixel)
byte[] pngBytes =
[
    0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A,  // PNG signature
    0x00, 0x00, 0x00, 0x0D, 0x49, 0x48, 0x44, 0x52,  // IHDR chunk length + type
    0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01,  // 1×1 px
    0x08, 0x02, 0x00, 0x00, 0x00, 0x90, 0x77, 0x53,  // bit depth, colour type, ...
    0xDE, 0x00, 0x00, 0x00, 0x0C, 0x49, 0x44, 0x41,  // IDAT chunk
    0x54, 0x08, 0xD7, 0x63, 0xF8, 0xCF, 0xC0, 0x00,
    0x00, 0x00, 0x02, 0x00, 0x01, 0xE2, 0x21, 0xBC,
    0x33, 0x00, 0x00, 0x00, 0x00, 0x49, 0x45, 0x4E,  // IEND chunk
    0x44, 0xAE, 0x42, 0x60, 0x82,
];
Console.WriteLine($"Byte buffer → mime:      {MimeGuesser.GuessMimeType(pngBytes)}");
Console.WriteLine($"Byte buffer → extension: {MimeGuesser.GuessExtension(pngBytes)}");

Console.WriteLine();

// From a stream — reuse the PNG bytes
using (MemoryStream ms = new(pngBytes))
{
    Console.WriteLine($"Stream      → mime:      {MimeGuesser.GuessMimeType(ms)}");
    Console.WriteLine($"Stream      → extension: {MimeGuesser.GuessExtension(ms)}");
}

Console.WriteLine();

// FileInfo extension method
FileInfo fi = new(sampleJpeg);
Console.WriteLine($"FileInfo    → mime:      {fi.GuessMimeType()}");
Console.WriteLine($"FileInfo    → extension: {fi.GuessExtension()}");

Console.WriteLine();
Console.WriteLine();

// -----------------------------------------------------------------------
// 2. Magic — low-level class for custom flag combinations
// -----------------------------------------------------------------------

Console.WriteLine("=== Magic (low-level) ===");
Console.WriteLine();

// Full human-readable description (MAGIC_NONE)
using (var magic = new Magic(MagicOpenFlags.MAGIC_NONE))
{
    Console.WriteLine($"Description → {magic.Read(sampleJpeg)}");
}

// Encoding of a text file created on the fly
string tempTxt = Path.GetTempFileName();
try
{
    await File.WriteAllTextAsync(tempTxt, "Hello, world! こんにちは", System.Text.Encoding.UTF8);

    using var magic = new Magic(MagicOpenFlags.MAGIC_MIME_ENCODING);
    Console.WriteLine($"Encoding    → {magic.Read(tempTxt)}");
}
finally
{
    File.Delete(tempTxt);
}

// MIME type + encoding together
using (var magic = new Magic(MagicOpenFlags.MAGIC_MIME_TYPE | MagicOpenFlags.MAGIC_MIME_ENCODING))
{
    Console.WriteLine($"MIME+enc    → {magic.Read(sampleJpeg)}");
}

Console.WriteLine();

// Library version
Console.WriteLine($"libmagic version: {Magic.Version}");
