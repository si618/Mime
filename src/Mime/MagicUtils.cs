namespace HeyRed.Mime;

internal static class MagicUtils
{
    private const string MagicDbName = "magic.mgc";

    private static string GetCurrentRid() =>
        OperatingSystem.IsWindows() ? "win" :
        OperatingSystem.IsLinux() ? "linux" :
        OperatingSystem.IsMacOS() ? "osx" :
        throw new PlatformNotSupportedException();

    public static string? GetDefaultMagicPath()
    {
        string assemblyLocation = typeof(MagicUtils).Assembly.Location;
        string currentPath = Path.GetDirectoryName(assemblyLocation) ?? "";

        string magicDbPath = Path.Combine(currentPath, MagicDbName);

        // Find inside current directory
        if (File.Exists(magicDbPath))
        {
            return magicDbPath;
        }

        var architecture = RuntimeInformation.ProcessArchitecture.ToString().ToLowerInvariant();

        magicDbPath = Path.Combine(currentPath, $"runtimes/{GetCurrentRid()}-{architecture}/native/", MagicDbName);

        // Find inside runtimes directory
        if (File.Exists(magicDbPath))
        {
            return magicDbPath;
        }

        return null;
    }
}