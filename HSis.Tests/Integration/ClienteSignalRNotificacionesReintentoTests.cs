using FluentAssertions;
using HSis.Contracts.Constants;
using HSis.UI.Services;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Http.Connections.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace HSis.Tests.Integration;

[Collection(NotificationServerCollection.Name)]
public sealed class ClienteSignalRNotificacionesReintentoTests(NotificationServerFixture fixture)
{
    private static readonly TimeSpan Espera = TimeSpan.FromSeconds(10);

    [Fact]
    public async Task IniciarAsyncConServidorCaidoNoDebeLanzarYDebePublicarDesconexion()
    {
        var bus = new BusEventosNotificaciones();
        var estados = new List<bool>();
        bus.OnEstadoConexionCambiado += (_, e) => estados.Add(e.Conectado);
        await using var cliente = CrearCliente(bus, fallosIniciales: int.MaxValue);

        await cliente.IniciarAsync(Token());

        cliente.IsConnected.Should().BeFalse();
        estados.Should().Equal(false);
    }

    [Fact]
    public async Task DebeReintentarEnSegundoPlanoYConectarCuandoElServidorVuelve()
    {
        var bus = new BusEventosNotificaciones();
        var conectado = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var estados = new List<bool>();
        bus.OnEstadoConexionCambiado += (_, e) =>
        {
            estados.Add(e.Conectado);
            if (e.Conectado)
            {
                conectado.TrySetResult();
            }
        };
        await using var cliente = CrearCliente(bus, fallosIniciales: 3);

        await cliente.IniciarAsync(Token());
        cliente.IsConnected.Should().BeFalse();

        await conectado.Task.WaitAsync(Espera);

        cliente.IsConnected.Should().BeTrue();
        cliente.IntentosDeConexion.Should().Be(4);
        estados.Should().Equal(false, true);
    }

    [Fact]
    public async Task DetenerAsyncDebeCancelarLosReintentosPendientes()
    {
        await using var cliente = CrearCliente(new BusEventosNotificaciones(), fallosIniciales: int.MaxValue);

        await cliente.IniciarAsync(Token());
        await EsperarAsync(() => cliente.IntentosDeConexion >= 3);

        await cliente.DetenerAsync();
        var intentosAlDetener = cliente.IntentosDeConexion;
        await Task.Delay(TimeSpan.FromMilliseconds(500));

        cliente.IntentosDeConexion.Should().BeLessThanOrEqualTo(intentosAlDetener + 1);
        cliente.IsConnected.Should().BeFalse();
    }

    [Fact]
    public async Task IniciarAsyncNuevoDebeReemplazarElReintentoAnterior()
    {
        await using var cliente = CrearCliente(new BusEventosNotificaciones(), fallosIniciales: 1);

        await cliente.IniciarAsync(Token());
        await cliente.IniciarAsync(Token());
        await EsperarAsync(() => cliente.IsConnected);
        await Task.Delay(TimeSpan.FromMilliseconds(500));

        cliente.IsConnected.Should().BeTrue();
        cliente.IntentosDeConexion.Should().Be(2);
    }

    private ClienteDePrueba CrearCliente(BusEventosNotificaciones bus, int fallosIniciales)
    {
        var configuracion = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["SignalR:ServerUrl"] = new Uri(fixture.Server.BaseAddress, "notificationHub").ToString()
            })
            .Build();
        return new ClienteDePrueba(configuracion, bus, fixture, fallosIniciales);
    }

    private string Token() => fixture.CrearToken(100, RolUsuarioEnum.Cliente);

    private static async Task EsperarAsync(Func<bool> condicion)
    {
        var limite = DateTime.UtcNow + Espera;
        while (!condicion())
        {
            if (DateTime.UtcNow > limite)
            {
                throw new TimeoutException("No se cumplió la condición esperada.");
            }

            await Task.Delay(20);
        }
    }

    // Simula un servidor inaccesible durante los primeros intentos y luego usa el servidor de pruebas.
    private sealed class ClienteDePrueba(
        IConfiguration configuracion,
        BusEventosNotificaciones bus,
        NotificationServerFixture fixture,
        int fallosIniciales)
        : ClienteSignalRNotificaciones(configuracion, NullLogger<ClienteSignalRNotificaciones>.Instance, bus),
            IAsyncDisposable
    {
        private int _intentos;

        public int IntentosDeConexion => Volatile.Read(ref _intentos);

        protected override TimeSpan RetardoInicialReintento => TimeSpan.FromMilliseconds(50);

        protected override void ConfigurarTransporte(HttpConnectionOptions options)
        {
            var intento = Interlocked.Increment(ref _intentos);
            options.Transports = HttpTransportType.LongPolling;
            options.HttpMessageHandlerFactory = _ => intento <= fallosIniciales
                ? new ServidorCaidoHandler()
                : fixture.Server.CreateHandler();
        }

        public async ValueTask DisposeAsync() => await DetenerAsync();
    }

    private sealed class ServidorCaidoHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
            => throw new HttpRequestException("Servidor no disponible (simulado).");
    }
}
