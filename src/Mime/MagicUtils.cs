namespace HeyRed.Mime;

internal static class MagicUtils
{
    private const string MAGIC_DB_NAME = "magic.mgc";

    private static string GetCurrentRid() =>
        RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "win" :
        RuntimeInformation.IsOSPlatform(OSPlatform.Linux)   ? "linux" :
        RuntimeInformation.IsOSPlatform(OSPlatform.OSX)     ? "osx" :
        throw new PlatformNotSupportedException();

    public static string? GetDefaultMagicPath()
    {
        string assemblyLocation = typeof(MagicUtils).Assembly.Location;
        string currentPath = Path.GetDirectoryName(assemblyLocation) ?? "";

        string magicDbPath = Path.Combine(currentPath, MAGIC_DB_NAME);

        // Find inside current directory
        if (File.Exists(magicDbPath))
        {
            return magicDbPath;
        }

        var architecture = RuntimeInformation.ProcessArchitecture.ToString().ToLowerInvariant();

        magicDbPath = Path.Combine(currentPath, $"runtimes/{GetCurrentRid()}-{architecture}/native/", MAGIC_DB_NAME);

        // Find inside runtimes directory
        if (File.Exists(magicDbPath))
        {
            return magicDbPath;
        }

        return null;
    }
}