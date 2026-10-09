using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using HSis.Contracts.DTOs;
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

        for (var intento = 0; intento < 12; intento++)
        {
            using var response = await client.PostAsJsonAsync("api/Auth/login", credenciales);
            estados.Add(response.StatusCode);
        }

        estados.Should().Contain(HttpStatusCode.TooManyRequests);
        estados.Take(10).Should().NotContain(HttpStatusCode.TooManyRequests);
    }
}
