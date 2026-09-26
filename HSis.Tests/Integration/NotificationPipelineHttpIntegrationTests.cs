using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using HSis.Contracts.Constants;
using HSis.Contracts.DTOs;
using Xunit;

namespace HSis.Tests.Integration;

[Collection(NotificationServerCollection.Name)]
public sealed class NotificationPipelineHttpIntegrationTests(NotificationServerFixture fixture)
{
    [Fact]
    public async Task ObtenerResumenSinTokenDebeResponder401()
    {
        await fixture.ReiniciarDatosAsync();
        using var client = fixture.CreateClient();

        using var response = await client.GetAsync("api/Notificaciones");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ObtenerResumenDebeAislarLasNotificacionesPorUsuario()
    {
        await fixture.ReiniciarDatosAsync();
        using var client = fixture.CrearClienteAutenticado(100, RolUsuarioEnum.Cliente);

        using var response = await client.GetAsync("api/Notificaciones");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var resumen = await response.Content.ReadFromJsonAsync<NotificacionesResumenDto>();

        resumen.Should().NotBeNull();
        resumen!.NoLeidas.Should().Be(1);
        resumen.Notificaciones.Should().HaveCount(2);
        resumen.Notificaciones.Select(item => item.IdNotificacion)
            .Should().BeEquivalentTo([1001, 1002]);
        resumen.Notificaciones.Should().OnlyContain(item => item.TicketId == 42);
    }

    [Fact]
    public async Task ObtenerNuevasDebeRespetarDesdeIdYAislamiento()
    {
        await fixture.ReiniciarDatosAsync();
        using var client = fixture.CrearClienteAutenticado(100, RolUsuarioEnum.Cliente);

        using var response = await client.GetAsync("api/Notificaciones/nuevas?desdeId=1001");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var notificaciones = await response.Content.ReadFromJsonAsync<List<NotificacionDto>>();
        notificaciones.Should().NotBeNull();
        notificaciones!.Select(item => item.IdNotificacion).Should().Equal(1002);
    }

    [Fact]
    public async Task ObtenerNuevasConDesdeIdNegativoDebeResponder400()
    {
        await fixture.ReiniciarDatosAsync();
        using var client = fixture.CrearClienteAutenticado(100, RolUsuarioEnum.Cliente);

        using var response = await client.GetAsync("api/Notificaciones/nuevas?desdeId=-1");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task MutacionesDebenRespetarElPropietarioDeLaNotificacion()
    {
        await fixture.ReiniciarDatosAsync();
        using var cliente = fixture.CrearClienteAutenticado(100, RolUsuarioEnum.Cliente);
        using var tecnico = fixture.CrearClienteAutenticado(200, RolUsuarioEnum.Tecnico);

        using (var respuestaAjena = await cliente.PutAsync("api/Notificaciones/2001/leida", null))
        {
            respuestaAjena.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        using (var resumenTecnico = await tecnico.GetAsync("api/Notificaciones"))
        {
            resumenTecnico.StatusCode.Should().Be(HttpStatusCode.OK);
            var contenido = await resumenTecnico.Content.ReadFromJsonAsync<NotificacionesResumenDto>();
            contenido.Should().NotBeNull();
            contenido!.NoLeidas.Should().Be(1);
            contenido.Notificaciones.Should().ContainSingle(item => item.IdNotificacion == 2001);
        }

        using (var marcarPropias = await cliente.PutAsync("api/Notificaciones/1001/leida", null))
        {
            marcarPropias.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        using (var eliminarPropias = await cliente.DeleteAsync("api/Notificaciones"))
        {
            eliminarPropias.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        using var resumenTecnicoFinal = await tecnico.GetAsync("api/Notificaciones");
        resumenTecnicoFinal.StatusCode.Should().Be(HttpStatusCode.OK);
        var contenidoFinal = await resumenTecnicoFinal.Content.ReadFromJsonAsync<NotificacionesResumenDto>();
        contenidoFinal.Should().NotBeNull();
        contenidoFinal!.Notificaciones.Should().ContainSingle(item => item.IdNotificacion == 2001);
        contenidoFinal.NoLeidas.Should().Be(1);
    }
}
