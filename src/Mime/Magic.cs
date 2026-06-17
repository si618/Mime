using System.Buffers;

namespace HeyRed.Mime;

/// <summary>
/// Provides access to some libmagic methods.
/// </summary>
public sealed class Magic : IDisposable
{
    private static readonly object _magicLock = new();

    private readonly IntPtr _magic;

    /// <summary>
    /// Contains the version number of this library which is compiled
    /// into the shared library using the constant.
    /// </summary>
    public static int Version => MagicNative.MagicVersion();

    private string LastError
    {
        get
        {
            var err = Marshal.PtrToStringAnsi(MagicNative.MagicError(_magic));
            return err != null ?
                char.ToUpper(err[0]) + err[1..] :
                string.Empty;
        }
    }

    /// <summary>
    /// Creates a magic cookie and load database from given path.
    /// </summary>
    /// <param name="flags"></param>
    /// <param name="dbPath"></param>
    public Magic(MagicOpenFlags flags, string? dbPath = null)
    {
        lock (_magicLock)
        {
            _magic = MagicNative.MagicOpen(flags);
            if (_magic == IntPtr.Zero)
            {
                throw new MagicException(LastError, "Cannot create magic cookie.");
            }

            dbPath ??= MagicUtils.GetDefaultMagicPath();

            if (MagicNative.MagicLoad(_magic, dbPath) != 0)
            {
                var error = LastError;
                MagicNative.MagicClose(_magic);
                _magic = IntPtr.Zero;
                throw new MagicException(error, "Unable to load magic database file.");
            }
        }
    }

    /// <summary>
    /// Reads file from given path.
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns>returns a textual description of the contents of file</returns>
    public string Read(string filePath)
    {
        ThrowIfDisposed();

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && HasNonAsciiChars(filePath))
        {
            byte[] buffer = File.ReadAllBytes(filePath);
            return Read(buffer, buffer.Length);
        }

        return Marshal.PtrToStringAnsi(MagicNative.MagicFile(_magic, filePath))
            ?? throw new MagicException(LastError);
    }

    private static bool HasNonAsciiChars(string value) => !System.Text.Ascii.IsValid(value);

    /// <summary>
    /// Reads contents from buffer.
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="bufferSize"></param>
    /// <returns>returns a textual description of the contents of the buffer</returns>
    public string Read(byte[] buffer, int bufferSize)
    {
        ThrowIfDisposed();

        ArgumentNullException.ThrowIfNull(buffer);
        ArgumentOutOfRangeException.ThrowIfNegative(bufferSize);

        var length = buffer.Length < bufferSize ? buffer.Length : bufferSize;

        return Marshal.PtrToStringAnsi(MagicNative.MagicBuffer(_magic, buffer, length))
            ?? throw new MagicException(LastError);
    }

    /// <summary>
    /// Reads contents from stream with buffer size limit.
    /// </summary>
    /// <remarks>
    /// This method rewinds the stream if it's possible.
    /// </remarks>
    /// <param name="stream"></param>
    /// <param name="bufferSize">in bytes</param>
    /// <returns>returns a textual description of the contents of the stream</returns>
    public string Read(Stream stream, int bufferSize)
    {
        ThrowIfDisposed();

        ArgumentNullException.ThrowIfNull(stream);

        byte[] rented = ArrayPool<byte>.Shared.Rent(bufferSize);
        try
        {
            int totalRead = 0;
            int read;
            while (totalRead < bufferSize &&
                   (read = stream.Read(rented, totalRead, bufferSize - totalRead)) > 0)
            {
                totalRead += read;
            }

            if (stream.CanSeek) stream.Position = 0;

            return Read(rented, totalRead);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(rented);
        }
    }

    /// <summary>
    /// Returns a value representing current <see cref="MagicOpenFlags"/> set.
    /// </summary>
    /// <returns></returns>
    public MagicOpenFlags GetFlags()
    {
        ThrowIfDisposed();

        return MagicNative.MagicGetFlags(_magic);
    }

    /// <summary>
    /// Sets the flags <see cref="MagicOpenFlags"/>
    /// Note that using both MIME flags together can also return extra information on the charset.
    /// </summary>
    /// <param name="flags"></param>
    public void SetFlags(MagicOpenFlags flags)
    {
        ThrowIfDisposed();

        if (MagicNative.MagicSetFlags(_magic, flags) < 0)
        {
            throw new MagicException("Utime/Utimes not supported.");
        }
    }

    /// <summary>
    /// Gets various limits related to the magic library.
    /// <see cref="MagicParams"/>
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    public int GetParam(MagicParams param)
    {
        ThrowIfDisposed();

        if (MagicNative.MagicGetParam(_magic, param, out int value) < 0)
        {
            throw new MagicException($"Invalid param \"{param}\".");
        }

        return value;
    }

    /// <summary>
    /// Sets various limits related to the magic library.
    /// <see cref="MagicParams"/>
    /// </summary>
    /// <param name="param"></param>
    /// <param name="value"></param>
    public void SetParam(MagicParams param, int value)
    {
        ThrowIfDisposed();

        if (MagicNative.MagicSetParam(_magic, param, ref value) < 0)
        {
            throw new MagicException($"Invalid param \"{param}\".");
        }
    }

    /// <summary>
    /// Can be used to check the validity of entries
    /// in the colon separated database files.
    /// </summary>
    /// <param name="dbPath"></param>
    public void CheckDatabase(string? dbPath = null)
    {
        ThrowIfDisposed();

        dbPath ??= MagicUtils.GetDefaultMagicPath();

        int result = MagicNative.MagicCheck(_magic, dbPath);
        if (result < 0)
        {
            throw new MagicException(LastError);
        }
    }

    /// <summary>
    /// Can be used to compile the colon separated list of database files.
    /// </summary>
    /// <param name="dbPath"></param>
    public void CompileDatabase(string? dbPath = null)
    {
        ThrowIfDisposed();

        if (MagicNative.MagicCompile(_magic, dbPath ?? "") < 0)
        {
            throw new MagicException(LastError);
        }
    }

    /// <summary>
    /// Lists the parsed magic database entries to stdout.
    /// </summary>
    /// <remarks>
    /// Output is written to native stdout by libmagic and cannot be
    /// captured via <see cref="Console.SetOut"/>.
    /// </remarks>
    /// <param name="dbPath"></param>
    public void ListDatabase(string? dbPath = null)
    {
        ThrowIfDisposed();

        dbPath ??= MagicUtils.GetDefaultMagicPath();

        if (MagicNative.MagicList(_magic, dbPath) < 0)
        {
            throw new MagicException(LastError);
        }
    }

    #region IDisposable support

    private bool _disposed = false;

    private void ThrowIfDisposed() => ObjectDisposedException.ThrowIf(_disposed, this);

    private void DoDispose()
    {
        if (_magic != IntPtr.Zero)
        {
            MagicNative.MagicClose(_magic);
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    ~Magic() => DoDispose();

    /// <summary>
    /// Cleanups all unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        if (_disposed) return;

        DoDispose();

        _disposed = true;

        GC.SuppressFinalize(this);
    }

    #endregion IDisposable support
}