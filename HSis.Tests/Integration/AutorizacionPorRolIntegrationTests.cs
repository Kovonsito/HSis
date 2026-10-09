using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using HSis.Contracts.Constants;
using HSis.Contracts.DTOs;
using Xunit;

namespace HSis.Tests.Integration;

// Datos sembrados: ticket 42 solicitado por el cliente 100 y asignado al tecnico 200; administrador 300.
[Collection(NotificationServerCollection.Name)]
public sealed class AutorizacionPorRolIntegrationTests(NotificationServerFixture fixture)
{
    [Theory]
    [InlineData(100, RolUsuarioEnum.Cliente, HttpStatusCode.OK)]
    [InlineData(101, RolUsuarioEnum.Cliente, HttpStatusCode.Forbidden)]
    [InlineData(200, RolUsuarioEnum.Tecnico, HttpStatusCode.OK)]
    [InlineData(201, RolUsuarioEnum.Tecnico, HttpStatusCode.Forbidden)]
    [InlineData(300, RolUsuarioEnum.Administrador, HttpStatusCode.OK)]
    public async Task ObtenerTicketPorIdDebeRespetarPropiedadYAsignacion(
        int idUsuario,
        RolUsuarioEnum rol,
        HttpStatusCode esperado)
    {
        await fixture.ReiniciarDatosAsync();
        using var client = fixture.CrearClienteAutenticado(idUsuario, rol);

        using var response = await client.GetAsync("api/Tickets/42");

        response.StatusCode.Should().Be(esperado);
    }

    [Theory]
    [InlineData(100, RolUsuarioEnum.Cliente, "GET", "api/Tickets")]
    [InlineData(200, RolUsuarioEnum.Tecnico, "GET", "api/Tickets")]
    [InlineData(100, RolUsuarioEnum.Cliente, "GET", "api/Tickets/42/historial")]
    [InlineData(201, RolUsuarioEnum.Tecnico, "GET", "api/Tickets/42/historial")]
    [InlineData(100, RolUsuarioEnum.Cliente, "GET", "api/Tickets/disponibles")]
    [InlineData(200, RolUsuarioEnum.Tecnico, "GET", "api/Tickets/kpis?inicio=2026-01-01&fin=2026-12-31")]
    [InlineData(200, RolUsuarioEnum.Tecnico, "GET", "api/Tickets/tecnico/201/asignados")]
    [InlineData(100, RolUsuarioEnum.Cliente, "GET", "api/Tickets/tecnico/200/indicadores")]
    [InlineData(100, RolUsuarioEnum.Cliente, "GET", "api/Tickets/usuario/101")]
    [InlineData(100, RolUsuarioEnum.Cliente, "GET", "api/Tickets/usuario/101/resumen")]
    [InlineData(100, RolUsuarioEnum.Cliente, "POST", "api/Tickets/filtrar/todos")]
    [InlineData(200, RolUsuarioEnum.Tecnico, "POST", "api/Tickets/filtrar")]
    [InlineData(100, RolUsuarioEnum.Cliente, "GET", "api/Usuarios/rol/2")]
    [InlineData(100, RolUsuarioEnum.Cliente, "GET", "api/Materiales")]
    [InlineData(200, RolUsuarioEnum.Tecnico, "PUT", "api/Materiales/1/costo")]
    [InlineData(100, RolUsuarioEnum.Cliente, "POST", "api/Materiales/movimientos")]
    [InlineData(100, RolUsuarioEnum.Cliente, "GET", "api/TicketDetalle/ticket/42")]
    [InlineData(201, RolUsuarioEnum.Tecnico, "GET", "api/TicketDetalle/ticket/42")]
    [InlineData(100, RolUsuarioEnum.Cliente, "POST", "api/Tickets/42/calificar/otro")]
    public async Task EndpointsFueraDelAlcanceDelRolDebenResponder403O404(
        int idUsuario,
        RolUsuarioEnum rol,
        string metodo,
        string ruta)
    {
        await fixture.ReiniciarDatosAsync();
        using var client = fixture.CrearClienteAutenticado(idUsuario, rol);
        using var request = new HttpRequestMessage(new HttpMethod(metodo), ruta);
        if (metodo is "POST" or "PUT")
        {
            request.Content = JsonContent.Create(new { });
        }

        using var response = await client.SendAsync(request);

        response.StatusCode.Should().BeOneOf(HttpStatusCode.Forbidden, HttpStatusCode.NotFound);
        if (!ruta.EndsWith("/otro", StringComparison.Ordinal))
        {
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }
    }

    [Fact]
    public async Task TecnicoNoAsignadoNoDebePoderModificarMaterialesDelTicket()
    {
        await fixture.ReiniciarDatosAsync();
        using var client = fixture.CrearClienteAutenticado(201, RolUsuarioEnum.Tecnico);

        using var response = await client.PostAsJsonAsync(
            "api/TicketDetalle",
            new TicketDetalleDto { IdTicket = 42, IdMaterial = 1, Cantidad = 1 });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task ClienteNoDebeCrearTicketsParaOtroUsuarioNiAsignarTecnico()
    {
        await fixture.ReiniciarDatosAsync();
        using var client = fixture.CrearClienteAutenticado(100, RolUsuarioEnum.Cliente);

        using var paraOtro = await client.PostAsJsonAsync(
            "api/Tickets",
            new TicketCreateDto { IdUsuario = 101, Descripcion = "Ticket ajeno" });
        using var conTecnico = await client.PostAsJsonAsync(
            "api/Tickets",
            new TicketCreateDto { IdUsuario = 100, Descripcion = "Con tecnico", IdTecnico = 200 });

        paraOtro.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        conTecnico.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task ClienteNoDebeActualizarTickets()
    {
        await fixture.ReiniciarDatosAsync();
        using var client = fixture.CrearClienteAutenticado(100, RolUsuarioEnum.Cliente);

        using var response = await client.PutAsJsonAsync(
            "api/Tickets/42",
            new TicketUpdateDto { IdTicket = 42, Estatus = ConstantesEstatus.CERRADO });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task TecnicoDebeVerSoloIdentidadDeUsuariosYAdministradorElDetalle()
    {
        await fixture.ReiniciarDatosAsync();
        using var tecnico = fixture.CrearClienteAutenticado(200, RolUsuarioEnum.Tecnico);
        using var administrador = fixture.CrearClienteAutenticado(300, RolUsuarioEnum.Administrador);

        var porTecnico = await tecnico.GetFromJsonAsync<List<UsuarioDto>>("api/Usuarios/rol/3");
        var porAdministrador = await administrador.GetFromJsonAsync<List<UsuarioDto>>("api/Usuarios/rol/3");

        porTecnico.Should().ContainSingle().Which.Nombre.Should().Be("Cliente de integración");
        porTecnico![0].IdDepartamento.Should().BeNull();
        porTecnico[0].RolNombre.Should().BeNull();
        porAdministrador.Should().ContainSingle().Which.RolNombre.Should().NotBeNull();
    }

    [Fact]
    public async Task SoloElSolicitanteDebePoderCalificarSuTicket()
    {
        await fixture.ReiniciarDatosAsync();
        using var tecnico = fixture.CrearClienteAutenticado(200, RolUsuarioEnum.Tecnico);
        using var otroCliente = fixture.CrearClienteAutenticado(101, RolUsuarioEnum.Cliente);

        using var porTecnico = await tecnico.PostAsJsonAsync("api/Tickets/42/calificar", new { Calificacion = 5 });
        using var porOtro = await otroCliente.PostAsJsonAsync("api/Tickets/42/calificar", new { Calificacion = 5 });

        porTecnico.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        porOtro.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
