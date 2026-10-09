using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using HSis.Contracts.DTOs;
using HSis.Contracts.Services;
using HSis.Desktop.Infrastructure;
using HSis.NotificationAgent.Services;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace HSis.Tests.Agent;

public sealed class NotificationAgentHostSignalRTests : IDisposable
{
    private readonly string _stateFilePath = Path.Combine(
        Path.GetTempPath(),
        $"hsis-agent-state-{Guid.NewGuid():N}.json");

    private readonly List<ConexionDoble> _conexiones = [];

    public void Dispose()
    {
        if (File.Exists(_stateFilePath))
        {
            File.Delete(_stateFilePath);
        }
    }

    [Fact]
    public async Task Inicio_ConServidorSignalRCaido_LiberaLaConexionYSigueConSondeoRest()
    {
        var api = new ApiFalsa();
        await using var host = CrearHost(api, _ => new HttpRequestException("servidor caido"));

        await host.StartAsync();
        await Esperar(() => api.SondeosRest >= 1);

        _conexiones.Should().ContainSingle();
        _conexiones[0].StartLlamadas.Should().Be(1);
        _conexiones[0].Liberada.Should().BeTrue();
        api.SondeosRest.Should().BeGreaterThanOrEqualTo(1);
    }

    [Fact]
    public async Task Inicio_ConHubException_LiberaLaConexion()
    {
        var api = new ApiFalsa();
        await using var host = CrearHost(api, _ => new HubException("rechazado"));

        await host.StartAsync();
        await Esperar(() => api.SondeosRest >= 1);

        _conexiones.Should().ContainSingle();
        _conexiones[0].Liberada.Should().BeTrue();
    }

    [Fact]
    public async Task Inicio_SinServerUrl_NoCreaConexionPeroSondeaPorRest()
    {
        var api = new ApiFalsa();
        await using var host = CrearHost(api, falloStart: null, serverUrl: null);

        await host.StartAsync();
        await Esperar(() => api.SondeosRest >= 1);

        _conexiones.Should().BeEmpty();
    }

    [Fact]
    public async Task Detener_ConConexionActiva_LlamaAStopYLibera()
    {
        var api = new ApiFalsa();
        var host = CrearHost(api, falloStart: null);

        await host.StartAsync();
        await Esperar(() => api.SondeosRest >= 1);
        await host.DisposeAsync();

        _conexiones.Should().ContainSingle();
        _conexiones[0].StopLlamadas.Should().Be(1);
        _conexiones[0].Liberada.Should().BeTrue();
    }

    // La doble conexion nunca pasa a Connected; el agente debe tratarla como caida y recrearla.
    // El ciclo de supervision espera como minimo 10 s entre sondeos, por eso esta prueba tarda.
    [Fact]
    public async Task CicloSiguiente_ConConexionCaida_LiberaLaAnteriorYCreaOtra()
    {
        var api = new ApiFalsa();
        await using var host = CrearHost(api, falloStart: null);

        await host.StartAsync();
        await Esperar(() => _conexiones.Count >= 2, TimeSpan.FromSeconds(25));

        _conexiones[0].Liberada.Should().BeTrue();
        _conexiones[1].StartLlamadas.Should().Be(1);
    }

    private NotificationAgentHost CrearHost(
        ApiFalsa api,
        Func<int, Exception?>? falloStart,
        string? serverUrl = "https://localhost/notificationHub")
    {
        var settings = new Dictionary<string, string?>
        {
            ["SignalR:ServerUrl"] = serverUrl,
            ["NotificationAgent:EnableNativeNotifications"] = "false",
            ["NotificationAgent:PollIntervalSeconds"] = "10"
        };
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(settings).Build();

        var credenciales = new Mock<IAlmacenamientoCredencialesLocal>();
        credenciales.Setup(c => c.GetCredentials()).Returns(("tecnico", "clave"));

        var cliente = new HttpClient(api) { BaseAddress = new Uri("https://localhost/") };

        return new NotificationAgentHost(
            configuration,
            cliente,
            new TokenApiClient(new HttpClient(api) { BaseAddress = cliente.BaseAddress }, NullLogger<TokenApiClient>.Instance),
            credenciales.Object,
            new PresenciaAplicacion(
                $"hsis-agent-presence-{Guid.NewGuid():N}.json",
                $"Local\\HSis.Tests.{Guid.NewGuid():N}"),
            new NativeNotificationService(NullLogger<NativeNotificationService>.Instance, configuration),
            NullLogger<NotificationAgentHost>.Instance,
            (_, _) =>
            {
                var conexion = new ConexionDoble(falloStart?.Invoke(_conexiones.Count));
                _conexiones.Add(conexion);
                return conexion;
            },
            _stateFilePath);
    }

    private static async Task Esperar(Func<bool> condicion, TimeSpan? limite = null)
    {
        var fin = DateTime.UtcNow + (limite ?? TimeSpan.FromSeconds(10));
        while (!condicion())
        {
            if (DateTime.UtcNow > fin)
            {
                throw new TimeoutException("La condicion esperada no se cumplio a tiempo.");
            }

            await Task.Delay(50);
        }
    }

    private sealed class ApiFalsa : HttpMessageHandler
    {
        private int _sondeosRest;

        public int SondeosRest => Volatile.Read(ref _sondeosRest);

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var ruta = request.RequestUri!.AbsolutePath;
            if (ruta.EndsWith("/api/Auth/login", StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = JsonContent.Create(new LoginResponseDto { Token = "token-de-prueba" })
                });
            }

            Interlocked.Increment(ref _sondeosRest);
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(Array.Empty<NotificacionDto>())
            });
        }
    }

    // HubConnection no esta sellada; se sustituye StartAsync/StopAsync/DisposeAsync para controlar los fallos.
    private sealed class ConexionDoble(Exception? falloStart) : HubConnection(
        new Mock<IConnectionFactory>().Object,
        new JsonHubProtocol(),
        new UriEndPoint(new Uri("https://localhost/notificationHub")),
        new ServiceCollection().BuildServiceProvider(),
        NullLoggerFactory.Instance)
    {
        private int _startLlamadas;
        private int _stopLlamadas;
        private int _liberada;

        public int StartLlamadas => Volatile.Read(ref _startLlamadas);

        public int StopLlamadas => Volatile.Read(ref _stopLlamadas);

        public bool Liberada => Volatile.Read(ref _liberada) != 0;

        public override Task StartAsync(CancellationToken cancellationToken = default)
        {
            Interlocked.Increment(ref _startLlamadas);
            return falloStart is null ? Task.CompletedTask : Task.FromException(falloStart);
        }

        public override Task StopAsync(CancellationToken cancellationToken = default)
        {
            Interlocked.Increment(ref _stopLlamadas);
            return Task.CompletedTask;
        }

        public override ValueTask DisposeAsync()
        {
            Interlocked.Exchange(ref _liberada, 1);
            return ValueTask.CompletedTask;
        }
    }
}
