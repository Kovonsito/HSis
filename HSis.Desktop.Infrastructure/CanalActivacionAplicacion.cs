using System.IO.Pipes;
using System.Runtime.Versioning;
using System.Text;
using System.Text.Json;

namespace HSis.Desktop.Infrastructure;

[SupportedOSPlatform("windows")]
public sealed class CanalActivacionAplicacion : IDisposable, IAsyncDisposable
{
    private const int MaximoMensajeBytes = 4096;
    private readonly string _nombreCanal;
    private readonly CancellationTokenSource _detener = new();
    private Task? _servidor;
    private int _iniciado;
    private int _disposed;

    public CanalActivacionAplicacion(string? nombreCanal = null)
    {
        _nombreCanal = string.IsNullOrWhiteSpace(nombreCanal)
            ? ConfiguracionEscritorio.CanalActivacionUi
            : nombreCanal;
    }

    public void Iniciar(Action<IReadOnlyList<string>> alRecibir)
    {
        ArgumentNullException.ThrowIfNull(alRecibir);
        ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed) != 0, this);
        if (Interlocked.Exchange(ref _iniciado, 1) != 0)
        {
            return;
        }

        _servidor = EscucharAsync(alRecibir, _detener.Token);
    }

    public bool IntentarEnviar(
        IReadOnlyList<string> argumentos,
        int tiempoEsperaMilisegundos = 1500)
    {
        ArgumentNullException.ThrowIfNull(argumentos);
        ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed) != 0, this);
        if (argumentos.Count == 0)
        {
            return false;
        }

        var mensaje = JsonSerializer.Serialize(argumentos);
        if (Encoding.UTF8.GetByteCount(mensaje) > MaximoMensajeBytes)
        {
            throw new ArgumentException(
                "La activación excede el tamaño máximo permitido.",
                nameof(argumentos));
        }

        try
        {
            using var cliente = new NamedPipeClientStream(
                ".",
                _nombreCanal,
                PipeDirection.Out,
                PipeOptions.Asynchronous | PipeOptions.CurrentUserOnly);
            cliente.Connect(Math.Clamp(tiempoEsperaMilisegundos, 100, 5000));

            using var writer = new StreamWriter(
                cliente,
                new UTF8Encoding(encoderShouldEmitUTF8Identifier: false))
            {
                AutoFlush = true
            };
            writer.WriteLine(mensaje);
            return true;
        }
        catch (TimeoutException)
        {
            return false;
        }
        catch (IOException)
        {
            return false;
        }
        catch (UnauthorizedAccessException)
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

        _detener.Cancel();
        _detener.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
        {
            return;
        }

        _detener.Cancel();
        if (_servidor is not null)
        {
            try
            {
                await _servidor;
            }
            catch (OperationCanceledException)
            {
            }
        }

        _detener.Dispose();
    }

    private async Task EscucharAsync(
        Action<IReadOnlyList<string>> alRecibir,
        CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await using var servidor = new NamedPipeServerStream(
                _nombreCanal,
                PipeDirection.In,
                1,
                PipeTransmissionMode.Byte,
                PipeOptions.Asynchronous | PipeOptions.CurrentUserOnly);

            try
            {
                await servidor.WaitForConnectionAsync(cancellationToken);
                using var reader = new StreamReader(
                    servidor,
                    new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
                var mensaje = await reader.ReadLineAsync(cancellationToken);
                if (string.IsNullOrWhiteSpace(mensaje) ||
                    Encoding.UTF8.GetByteCount(mensaje) > MaximoMensajeBytes)
                {
                    continue;
                }

                var argumentos = JsonSerializer.Deserialize<string[]>(mensaje);
                if (argumentos is { Length: > 0 })
                {
                    alRecibir(argumentos);
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                break;
            }
            catch (IOException)
            {
            }
            catch (JsonException)
            {
            }
        }
    }
}
