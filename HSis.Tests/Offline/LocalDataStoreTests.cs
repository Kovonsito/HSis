using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using HSis.Contracts.DTOs;
using HSis.Desktop.Infrastructure.Offline;
using Xunit;

namespace HSis.Tests.Offline;

public sealed class LocalDataStoreTests : IDisposable
{
    private readonly string _tempDbPath;
    private readonly LocalDataStore _store;

    public LocalDataStoreTests()
    {
        _tempDbPath = Path.Combine(Path.GetTempPath(), $"hsis_test_{Guid.NewGuid():N}.db");
        _store = new LocalDataStore(_tempDbPath);
    }

    [Fact]
    public async Task EncolarYObtenerOperacionesPendientesDebeFuncionarCorrectamente()
    {
        await _store.InicializarAsync();

        var idOp = await _store.EncolarOperacionAsync(
            "CREAR_TICKET",
            "api/Tickets",
            "POST",
            "{\"Descripcion\":\"Test\"}",
            "-101");

        idOp.Should().NotBeEmpty();

        var pendientes = await _store.ObtenerOperacionesPendientesAsync();
        pendientes.Should().HaveCount(1);
        pendientes[0].IdOperacion.Should().Be(idOp);
        pendientes[0].TipoOperacion.Should().Be("CREAR_TICKET");
        pendientes[0].EntidadTempId.Should().Be("-101");

        // Marcar sincronizada
        await _store.MarcarOperacionSincronizadaAsync(idOp, idReal: 555);

        var pendientesPost = await _store.ObtenerOperacionesPendientesAsync();
        pendientesPost.Should().BeEmpty();
    }

    [Fact]
    public async Task GuardarYObtenerTicketsCacheDebePersistirDatos()
    {
        await _store.InicializarAsync();

        var ticket = new TicketDto
        {
            IdTicket = 77,
            IdUsuario = 10,
            NombreUsuario = "Usuario Test",
            Descripcion = "Problema con impresora",
            Estatus = "Abierto",
            Prioridad = "Alta",
            FechaAlta = DateTime.UtcNow
        };

        await _store.GuardarTicketIndividualCacheAsync(ticket);

        var cached = await _store.ObtenerTicketCachePorIdAsync(77);
        cached.Should().NotBeNull();
        cached!.IdTicket.Should().Be(77);
        cached.Descripcion.Should().Be("Problema con impresora");
        cached.Estatus.Should().Be("Abierto");

        // Actualizar estatus en cache
        await _store.ActualizarEstatusTicketCacheAsync(77, "Cerrado");

        var actualizado = await _store.ObtenerTicketCachePorIdAsync(77);
        actualizado.Should().NotBeNull();
        actualizado!.Estatus.Should().Be("Cerrado");
    }

    public void Dispose()
    {
        try
        {
            if (File.Exists(_tempDbPath))
            {
                File.Delete(_tempDbPath);
            }
        }
        catch
        {
        }
    }
}
