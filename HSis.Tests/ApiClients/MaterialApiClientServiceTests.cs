using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using HSis.Contracts.DTOs;
using HSis.UI.ApiClients;
using Xunit;

namespace HSis.Tests.ApiClients;

public sealed class MaterialApiClientServiceTests
{
    // El tecnico no tiene acceso a api/Catalogos/materiales (solo administradores); el kardex debe usar api/Materiales.
    [Fact]
    public async Task ObtenerMaterialesAsyncDebeUsarLaRutaDisponibleParaPersonalDeAtencion()
    {
        var manejador = new ManejadorRegistrador(() => JsonContent.Create(new[] { new MaterialDto { IdMaterial = 1, Nombre = "Cable" } }));
        using var cliente = new HttpClient(manejador) { BaseAddress = new Uri("https://localhost/") };
        var servicio = new MaterialApiClientService(cliente);

        var materiales = await servicio.ObtenerMaterialesAsync();

        manejador.Rutas.Should().ContainSingle().Which.Should().Be("/api/Materiales");
        materiales.Should().ContainSingle().Which.Nombre.Should().Be("Cable");
    }

    private sealed class ManejadorRegistrador(Func<HttpContent> crearContenido) : HttpMessageHandler
    {
        public List<string> Rutas { get; } = [];

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Rutas.Add(request.RequestUri!.AbsolutePath);
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = crearContenido() });
        }
    }
}