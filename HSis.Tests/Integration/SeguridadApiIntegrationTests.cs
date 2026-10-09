using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using System.Text.Json;
using HSis.Contracts.Constants;
using HSis.Contracts.DTOs;
using HSis.Contracts.Errors;
using Xunit;

namespace HSis.Tests.Integration;

[Collection(NotificationServerCollection.Name)]
public sealed class SeguridadApiIntegrationTests(NotificationServerFixture fixture)
{
    [Theory]
    [InlineData("GET", "api/Usuarios/rol/1")]
    [InlineData("POST", "api/ReportExport/excel")]
    [InlineData("POST", "api/ReportExport/pdf")]
    public async Task EndpointsProtegidosSinTokenDebenResponder401(string metodo, string ruta)
    {
        using var client = fixture.CreateClient();
        using var request = new HttpRequestMessage(new HttpMethod(metodo), ruta);
        if (metodo == "POST")
        {
            request.Content = JsonContent.Create(new { });
        }

        using var response = await client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task HealthDebeResponderSinTokenYSinExponerDetalles()
    {
        using var client = fixture.CreateClient();

        using var response = await client.GetAsync("health");
        var cuerpo = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        cuerpo.Should().Be("Healthy");
    }

    [Fact]
    public async Task LoginDebeLimitarIntentosRepetidosConError429()
    {
        using var client = fixture.CreateClient();
        var credenciales = new LoginRequestDto { Username = "inexistente", Password = "incorrecta" };
        var estados = new List<HttpStatusCode>();

        JsonElement? primerProblema = null;

        for (var intento = 0; intento < 12; intento++)
        {
            using var response = await client.PostAsJsonAsync("api/Auth/login", credenciales);
            estados.Add(response.StatusCode);
            if (intento == 0)
            {
                primerProblema = await response.Content.ReadFromJsonAsync<JsonElement>();
            }
        }

        estados.Should().Contain(HttpStatusCode.TooManyRequests);
        estados.Take(10).Should().OnlyContain(estado => estado == HttpStatusCode.Unauthorized);
        primerProblema!.Value.GetProperty("code").GetString().Should().Be(ApiErrorCodes.Unauthorized);
    }

    [Fact]
    public async Task TicketInexistenteDebeResponder404ConCodigoEspecifico()
    {
        await fixture.ReiniciarDatosAsync();
        using var client = fixture.CrearClienteAutenticado(300, RolUsuarioEnum.Administrador);

        using var response = await client.GetAsync("api/Tickets/9999");
        var problema = await response.Content.ReadFromJsonAsync<JsonElement>();

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        problema.GetProperty("code").GetString().Should().Be(ApiErrorCodes.TicketNotFound);
    }
}