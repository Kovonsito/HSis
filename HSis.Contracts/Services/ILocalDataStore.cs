using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HSis.Contracts.DTOs;

namespace HSis.Contracts.Services;

public interface ILocalDataStore
{
    Task InicializarAsync();

    // Outbox
    Task<Guid> EncolarOperacionAsync(string tipo, string ruta, string metodo, string payloadJson, string? tempId = null);
    Task<List<OperacionOutboxDto>> ObtenerOperacionesPendientesAsync();
    Task MarcarOperacionSincronizadaAsync(Guid idOperacion, int? idReal = null);
    Task MarcarOperacionFallidaAsync(Guid idOperacion, string error);
    Task<int> ObtenerConteoPendientesAsync();

    // Cache Tickets
    Task GuardarTicketsCacheAsync(IEnumerable<TicketDto> tickets);
    Task GuardarTicketIndividualCacheAsync(TicketDto ticket, bool esOffline = false);
    Task<List<TicketDto>> ObtenerTicketsCacheAsync(int? idUsuario = null, int? idTecnico = null, string? estatus = null);
    Task<TicketDto?> ObtenerTicketCachePorIdAsync(int idTicket);
    Task ActualizarEstatusTicketCacheAsync(int idTicket, string nuevoEstatus);

    // Cache Catálogos
    Task GuardarCatalogoCacheAsync(string tipoCatalogo, IEnumerable<CatalogoItemDto> items);
    Task<List<CatalogoItemDto>> ObtenerCatalogoCacheAsync(string tipoCatalogo);
}
