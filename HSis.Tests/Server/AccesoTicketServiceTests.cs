using System.Security.Claims;
using FluentAssertions;
using HSis.Contracts.Constants;
using HSis.Contracts.DTOs;
using HSis.Contracts.Services;
using HSis.Server.Security;
using Moq;
using Xunit;

namespace HSis.Tests.Server;

public sealed class AccesoTicketServiceTests
{
    private const int IdTicket = 7;
    private const int Solicitante = 10;
    private const int TecnicoAsignado = 20;
    private const int OtroTecnico = 21;

    [Theory]
    // Administrador
    [InlineData(1, RolUsuarioEnum.Administrador, AlcanceTicket.Materiales, TecnicoAsignado, true)]
    // Tecnico asignado
    [InlineData(TecnicoAsignado, RolUsuarioEnum.Tecnico, AlcanceTicket.Materiales, TecnicoAsignado, true)]
    [InlineData(TecnicoAsignado, RolUsuarioEnum.Tecnico, AlcanceTicket.Gestion, TecnicoAsignado, true)]
    // Tecnico ajeno
    [InlineData(OtroTecnico, RolUsuarioEnum.Tecnico, AlcanceTicket.Consulta, TecnicoAsignado, false)]
    [InlineData(OtroTecnico, RolUsuarioEnum.Tecnico, AlcanceTicket.Materiales, TecnicoAsignado, false)]
    // Ticket sin asignar: el tecnico puede tomarlo pero no consumir materiales
    [InlineData(OtroTecnico, RolUsuarioEnum.Tecnico, AlcanceTicket.Gestion, null, true)]
    [InlineData(OtroTecnico, RolUsuarioEnum.Tecnico, AlcanceTicket.Materiales, null, false)]
    // Cliente
    [InlineData(Solicitante, RolUsuarioEnum.Cliente, AlcanceTicket.Consulta, TecnicoAsignado, true)]
    [InlineData(Solicitante, RolUsuarioEnum.Cliente, AlcanceTicket.Solicitante, TecnicoAsignado, true)]
    [InlineData(Solicitante, RolUsuarioEnum.Cliente, AlcanceTicket.Gestion, TecnicoAsignado, false)]
    [InlineData(Solicitante, RolUsuarioEnum.Cliente, AlcanceTicket.Materiales, TecnicoAsignado, false)]
    [InlineData(11, RolUsuarioEnum.Cliente, AlcanceTicket.Consulta, TecnicoAsignado, false)]
    public async Task PermiteAsyncDebeAplicarLaMatrizDeRolesYAsignacion(
        int idUsuario,
        RolUsuarioEnum rol,
        AlcanceTicket alcance,
        int? idTecnico,
        bool esperado)
    {
        var servicio = CrearServicio(new TicketDto { IdTicket = IdTicket, IdUsuario = Solicitante, IdTecnico = idTecnico });

        var permitido = await servicio.PermiteAsync(CrearUsuario(idUsuario, rol.ToString("D")), IdTicket, alcance);

        permitido.Should().Be(esperado);
    }

    [Theory]
    [InlineData("3", 0)]
    [InlineData(null, 10)]
    [InlineData("99", 10)]
    [InlineData("texto", 10)]
    public async Task PermiteAsyncConClaimsInvalidosDebeDenegar(string? rol, int idUsuario)
    {
        var servicio = CrearServicio(new TicketDto { IdTicket = IdTicket, IdUsuario = Solicitante });

        var permitido = await servicio.PermiteAsync(CrearUsuario(idUsuario, rol), IdTicket, AlcanceTicket.Consulta);

        permitido.Should().BeFalse();
    }

    [Fact]
    public async Task PermiteAsyncConTicketInexistenteDebeDenegarAlNoAdministrador()
    {
        var servicio = CrearServicio(null);

        var permitido = await servicio.PermiteAsync(
            CrearUsuario(TecnicoAsignado, RolUsuarioEnum.Tecnico.ToString("D")),
            IdTicket,
            AlcanceTicket.Consulta);

        permitido.Should().BeFalse();
    }

    private static AccesoTicketService CrearServicio(TicketDto? ticket)
    {
        var tickets = new Mock<ITicketService>();
        tickets.Setup(servicio => servicio.ObtenerTicketPorIdAsync(IdTicket)).ReturnsAsync(ticket);
        return new AccesoTicketService(tickets.Object);
    }

    private static ClaimsPrincipal CrearUsuario(int idUsuario, string? rol)
    {
        var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, idUsuario.ToString()) };
        if (rol is not null)
        {
            claims.Add(new Claim(ClaimTypes.Role, rol));
        }

        return new ClaimsPrincipal(new ClaimsIdentity(claims, "Prueba"));
    }
}
