using FluentAssertions;
using HSis.Contracts.Constants;
using Xunit;

namespace HSis.Tests.Services
{
    public class ReglasEstatusTicketTests
    {
        [Fact]
        public void AdministradorDebeTenerTodosLosEstatusDisponibles()
        {
            var permitidos = ReglasEstatusTicket.ObtenerEstatusPermitidos((int)RolUsuarioEnum.Administrador, ConstantesEstatus.ABIERTO);

            permitidos.Should().Contain([ConstantesEstatus.ABIERTO, ConstantesEstatus.EN_PROCESO, ConstantesEstatus.CERRADO, ConstantesEstatus.REABIERTO]);
        }

        [Fact]
        public void TecnicoEnEstatusAbiertoDebePoderPasarAEnProceso()
        {
            var permitidos = ReglasEstatusTicket.ObtenerEstatusPermitidos((int)RolUsuarioEnum.Tecnico, ConstantesEstatus.ABIERTO);

            permitidos.Should().Contain(ConstantesEstatus.EN_PROCESO);
            permitidos.Should().NotContain(ConstantesEstatus.CERRADO);
        }

        [Fact]
        public void TecnicoEnEstatusEnProcesoDebePoderPasarACerrado()
        {
            var permitidos = ReglasEstatusTicket.ObtenerEstatusPermitidos((int)RolUsuarioEnum.Tecnico, ConstantesEstatus.EN_PROCESO);

            permitidos.Should().Contain(ConstantesEstatus.CERRADO);
        }

        [Fact]
        public void ClienteEnEstatusCerradoDebePoderReabrir()
        {
            var permitidos = ReglasEstatusTicket.ObtenerEstatusPermitidos((int)RolUsuarioEnum.Cliente, ConstantesEstatus.CERRADO);

            permitidos.Should().Contain(ConstantesEstatus.REABIERTO);
        }
    }
}
