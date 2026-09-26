using System.Runtime.Versioning;
using System.Text.Json;

namespace HSis.Desktop.Infrastructure;

[SupportedOSPlatform("windows")]
public sealed class PresenciaAplicacion : IDisposable
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly string _presenceFilePath;
    private readonly string _mutexName;
    private readonly object _leaseLock = new();
    private Lease? _applicationLease;
    private int _disposed;

    public PresenciaAplicacion(
        string? presenceFileName = null,
        string? mutexName = null)
    {
        var fileName = string.IsNullOrWhiteSpace(presenceFileName)
            ? ConfiguracionEscritorio.ArchivoPresenciaUi
            : presenceFileName;

        _presenceFilePath = ConfiguracionEscritorio.ObtenerRutaDatos(fileName);
        _mutexName = string.IsNullOrWhiteSpace(mutexName)
            ? ConfiguracionEscritorio.MutexPresenciaUi
            : mutexName;
    }

    public void Activar()
    {
        ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed) != 0, this);

        lock (_leaseLock)
        {
            _applicationLease ??= new Lease(_presenceFilePath, _mutexName);
        }
    }

    public void Desactivar()
    {
        lock (_leaseLock)
        {
            _applicationLease?.Dispose();
            _applicationLease = null;
        }
    }

    public IDisposable CrearLease()
    {
        ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed) != 0, this);
        return new Lease(_presenceFilePath, _mutexName);
    }

    public bool EstaActiva(TimeSpan timeout)
    {
        if (timeout < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(timeout));
        }

        try
        {
            if (Mutex.TryOpenExisting(_mutexName, out var mutex))
            {
                mutex.Dispose();
                return true;
            }
        }
        catch (UnauthorizedAccessException)
        {
            return false;
        }
        catch (WaitHandleCannotBeOpenedException)
        {
            return false;
        }

        try
        {
            if (!File.Exists(_presenceFilePath))
            {
                return false;
            }

            var presence = JsonSerializer.Deserialize<PresenceState>(
                File.ReadAllText(_presenceFilePath),
                JsonOptions);

            return presence is not null &&
                   DateTimeOffset.UtcNow - presence.LastSeenUtc <= timeout;
        }
        catch (IOException)
        {
            return false;
        }
        catch (UnauthorizedAccessException)
        {
            return false;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
        {
            return;
        }

        Desactivar();
    }

    private sealed record PresenceState(
        DateTimeOffset LastSeenUtc,
        int ProcessId,
        string? LeaseId = null);

    private sealed class Lease : IDisposable
    {
        private readonly string _filePath;
        private readonly Mutex _mutex;
        private readonly bool _ownsMutex;
        private readonly string _leaseId = Guid.NewGuid().ToString("N");
        private readonly Timer _timer;
        private int _disposed;

        public Lease(string filePath, string mutexName)
        {
            _filePath = filePath;
            _mutex = new Mutex(initiallyOwned: false, mutexName);
            try
            {
                _ownsMutex = _mutex.WaitOne(0);
            }
            catch (AbandonedMutexException)
            {
                _ownsMutex = true;
            }

            Actualizar();
            _timer = new Timer(
                static state => ((Lease)state!).Actualizar(),
                this,
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(5));
        }

        private void Actualizar()
        {
            if (Volatile.Read(ref _disposed) != 0)
            {
                return;
            }

            try
            {
                var directory = Path.GetDirectoryName(_filePath);
                if (!string.IsNullOrWhiteSpace(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var state = new PresenceState(
                    DateTimeOffset.UtcNow,
                    Environment.ProcessId,
                    _leaseId);
                var temporaryPath = $"{_filePath}.{Environment.ProcessId}.{_leaseId}.tmp";
                File.WriteAllText(temporaryPath, JsonSerializer.Serialize(state, JsonOptions));
                File.Move(temporaryPath, _filePath, overwrite: true);
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) != 0)
            {
                return;
            }

            _timer.Dispose();
            EliminarPresenciaSiCorresponde();

            if (_ownsMutex)
            {
                try
                {
                    _mutex.ReleaseMutex();
                }
                catch (SynchronizationLockException)
                {
                }
            }

            _mutex.Dispose();
        }

        private void EliminarPresenciaSiCorresponde()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    return;
                }

                var presence = JsonSerializer.Deserialize<PresenceState>(
                    File.ReadAllText(_filePath),
                    JsonOptions);
                if (presence?.ProcessId == Environment.ProcessId &&
                    (string.IsNullOrEmpty(presence.LeaseId) || presence.LeaseId == _leaseId))
                {
                    File.Delete(_filePath);
                }
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
            catch (JsonException)
            {
            }
        }
    }
}
