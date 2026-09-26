using System.Runtime.Versioning;

namespace HSis.Desktop.Infrastructure;

[SupportedOSPlatform("windows")]
public sealed class InstanciaAplicacion : IDisposable
{
    private readonly Mutex _mutex;
    private int _disposed;

    public InstanciaAplicacion(string nombreMutex)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nombreMutex);
        _mutex = new Mutex(initiallyOwned: true, nombreMutex, out var creada);
        EsPrimeraInstancia = creada;
    }

    public bool EsPrimeraInstancia { get; }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
        {
            return;
        }

        if (EsPrimeraInstancia)
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
}
