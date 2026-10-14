using System;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using HSis.Contracts.Constants;
using HSis.Contracts.DTOs;
using Xunit;

namespace HSis.Tests.Integration;

[Collection(NotificationServerCollection.Name)]
public sealed class IdempotencyIntegrationTests(NotificationServerFixture fixture)
{
    [Fact]
    public async Task PeticionConMismaClaveDebeRetornarRespuestaPreviaYNoDuplicarTicket()
    {
        await fixture.ReiniciarDatosAsync();
        using var client = fixture.CrearClienteAutenticado(100, RolUsuarioEnum.Cliente);

        var idempotencyKey = Guid.NewGuid().ToString();
        var ticketCreate = new TicketCreateDto
        {
            IdUsuario = 100,
            Descripcion = "Ticket idempotente de prueba",
            Prioridad = null
        };

        // 1. Primera petición
        using var request1 = new HttpRequestMessage(HttpMethod.Post, "api/Tickets")
        {
            Content = JsonContent.Create(ticketCreate)
        };
        request1.Headers.Add(ConstantesSincronizacion.HeaderIdempotencia, idempotencyKey);
        using var response1 = await client.SendAsync(request1);

        response1.StatusCode.Should().Be(HttpStatusCode.Created);
        response1.Headers.Contains(ConstantesSincronizacion.HeaderIdempotenciaReplay).Should().BeFalse();
        var creado1 = await response1.Content.ReadFromJsonAsync<TicketDto>();
        creado1.Should().NotBeNull();

        // 2. Segunda petición con la misma clave de idempotencia
        using var request2 = new HttpRequestMessage(HttpMethod.Post, "api/Tickets")
        {
            Content = JsonContent.Create(ticketCreate)
        };
        request2.Headers.Add(ConstantesSincronizacion.HeaderIdempotencia, idempotencyKey);
        using var response2 = await client.SendAsync(request2);

        response2.StatusCode.Should().Be(HttpStatusCode.Created);
        response2.Headers.Contains(ConstantesSincronizacion.HeaderIdempotenciaReplay).Should().BeTrue();
        var creado2 = await response2.Content.ReadFromJsonAsync<TicketDto>();
        creado2.Should().NotBeNull();
        creado2!.IdTicket.Should().Be(creado1!.IdTicket);
    }
}
