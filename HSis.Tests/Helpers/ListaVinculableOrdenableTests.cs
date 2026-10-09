using FluentAssertions;
using HSis.Contracts.DTOs;
using HSis.UI.Helpers;
using Xunit;

namespace HSis.Tests.Helpers;

public sealed class ListaVinculableOrdenableTests
{
    // Con AllowNew el DataGridView agrega una fila vacia de alta; abrirla pedia el ticket 0 y daba Forbidden al tecnico.
    [Fact]
    public void ListaVacia_NoDebePermitirAltas()
    {
        new ListaVinculableOrdenable<TicketDto>().AllowNew.Should().BeFalse();
    }

    [Fact]
    public void ListaConElementos_NoDebePermitirAltas()
    {
        new ListaVinculableOrdenable<TicketDto>([new TicketDto { IdTicket = 1025 }]).AllowNew.Should().BeFalse();
    }
}