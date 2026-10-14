#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HSis.Contracts.DTOs;
using HSis.Contracts.Services;
using HSis.Desktop.Infrastructure.Offline.Models;
using Microsoft.EntityFrameworkCore;

namespace HSis.Desktop.Infrastructure.Offline;

public class LocalDataStore : ILocalDataStore
{
    private readonly string? _dbPath;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private bool _initialized;

    public LocalDataStore(string? dbPath = null)
    {
        _dbPath = dbPath;
    }

    private HSisLocalDbContext CreateContext() => new(_dbPath);

    public async Task InicializarAsync()
    {
        if (_initialized) return;

        await _lock.WaitAsync();
        try
        {
            if (_initialized) return;
            await using var db = CreateContext();
            await db.Database.EnsureCreatedAsync();
            _initialized = true;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<Guid> EncolarOperacionAsync(string tipo, string ruta, string metodo, string payloadJson, string? tempId = null)
    {
        await InicializarAsync();
        await using var db = CreateContext();

        var operacion = new OperacionOutboxLocal
        {
            IdOperacion = Guid.NewGuid(),
            TipoOperacion = tipo,
            RutaEndpoint = ruta,
            MetodoHttp = metodo,
            PayloadJson = payloadJson,
            EntidadTempId = tempId,
            FechaCreacion = DateTime.UtcNow,
            Estado = EstadoOperacionOutbox.Pendiente,
            Intentos = 0
        };

        db.OperacionesOutbox.Add(operacion);
        await db.SaveChangesAsync();
        return operacion.IdOperacion;
    }

    public async Task<List<OperacionOutboxDto>> ObtenerOperacionesPendientesAsync()
    {
        await InicializarAsync();
        await using var db = CreateContext();

        var items = await db.OperacionesOutbox
            .AsNoTracking()
            .Where(o => o.Estado == EstadoOperacionOutbox.Pendiente)
            .OrderBy(o => o.FechaCreacion)
            .ToListAsync();

        return items.Select(o => new OperacionOutboxDto
        {
            IdOperacion = o.IdOperacion,
            TipoOperacion = o.TipoOperacion,
            RutaEndpoint = o.RutaEndpoint,
            MetodoHttp = o.MetodoHttp,
            PayloadJson = o.PayloadJson,
            EntidadTempId = o.EntidadTempId,
            EntidadIdReal = o.EntidadIdReal,
            FechaCreacion = o.FechaCreacion,
            Intentos = o.Intentos,
            Estado = (int)o.Estado,
            UltimoError = o.UltimoError
        }).ToList();
    }

    public async Task MarcarOperacionSincronizadaAsync(Guid idOperacion, int? idReal = null)
    {
        await InicializarAsync();
        await using var db = CreateContext();

        var item = await db.OperacionesOutbox.FindAsync(idOperacion);
        if (item != null)
        {
            item.Estado = EstadoOperacionOutbox.Sincronizado;
            if (idReal.HasValue) item.EntidadIdReal = idReal;
            await db.SaveChangesAsync();
        }
    }

    public async Task MarcarOperacionFallidaAsync(Guid idOperacion, string error)
    {
        await InicializarAsync();
        await using var db = CreateContext();

        var item = await db.OperacionesOutbox.FindAsync(idOperacion);
        if (item != null)
        {
            item.Intentos++;
            item.UltimoError = error;
            if (item.Intentos >= 5)
            {
                item.Estado = EstadoOperacionOutbox.Fallido;
            }
            await db.SaveChangesAsync();
        }
    }

    public async Task<int> ObtenerConteoPendientesAsync()
    {
        await InicializarAsync();
        await using var db = CreateContext();

        return await db.OperacionesOutbox
            .CountAsync(o => o.Estado == EstadoOperacionOutbox.Pendiente);
    }

    public async Task GuardarTicketsCacheAsync(IEnumerable<TicketDto> tickets)
    {
        await InicializarAsync();
        await using var db = CreateContext();

        foreach (var t in tickets)
        {
            var existente = await db.TicketsCache.FindAsync(t.IdTicket);
            if (existente == null)
            {
                var nuevo = new TicketCacheLocal();
                MapearDtoEnCache(t, nuevo, false);
                db.TicketsCache.Add(nuevo);
            }
            else
            {
                MapearDtoEnCache(t, existente, false);
            }
        }

        await db.SaveChangesAsync();
    }

    public async Task GuardarTicketIndividualCacheAsync(TicketDto ticket, bool esOffline = false)
    {
        await InicializarAsync();
        await using var db = CreateContext();

        var existente = await db.TicketsCache.FindAsync(ticket.IdTicket);
        if (existente == null)
        {
            var nuevo = new TicketCacheLocal();
            MapearDtoEnCache(ticket, nuevo, esOffline);
            db.TicketsCache.Add(nuevo);
        }
        else
        {
            MapearDtoEnCache(ticket, existente, esOffline);
        }

        await db.SaveChangesAsync();
    }

    public async Task<List<TicketDto>> ObtenerTicketsCacheAsync(int? idUsuario = null, int? idTecnico = null, string? estatus = null)
    {
        await InicializarAsync();
        await using var db = CreateContext();

        var query = db.TicketsCache.AsNoTracking().AsQueryable();

        if (idUsuario.HasValue)
        {
            query = query.Where(t => t.IdUsuario == idUsuario.Value);
        }

        if (idTecnico.HasValue)
        {
            query = query.Where(t => t.IdTecnico == idTecnico.Value);
        }

        if (!string.IsNullOrWhiteSpace(estatus))
        {
            query = query.Where(t => t.Estatus == estatus);
        }

        var items = await query.OrderByDescending(t => t.FechaAlta).ToListAsync();
        return items.Select(MapearCacheEnDto).ToList();
    }

    public async Task<TicketDto?> ObtenerTicketCachePorIdAsync(int idTicket)
    {
        await InicializarAsync();
        await using var db = CreateContext();

        var t = await db.TicketsCache.AsNoTracking().FirstOrDefaultAsync(x => x.IdTicket == idTicket);
        return t == null ? null : MapearCacheEnDto(t);
    }

    public async Task ActualizarEstatusTicketCacheAsync(int idTicket, string nuevoEstatus)
    {
        await InicializarAsync();
        await using var db = CreateContext();

        var t = await db.TicketsCache.FindAsync(idTicket);
        if (t != null)
        {
            t.Estatus = nuevoEstatus;
            t.UltimaActualizacion = DateTime.UtcNow;
            await db.SaveChangesAsync();
        }
    }

    public async Task GuardarCatalogoCacheAsync(string tipoCatalogo, IEnumerable<CatalogoItemDto> items)
    {
        await InicializarAsync();
        await using var db = CreateContext();

        var anteriores = await db.CatalogosCache.Where(c => c.TipoCatalogo == tipoCatalogo).ToListAsync();
        db.CatalogosCache.RemoveRange(anteriores);

        foreach (var item in items)
        {
            db.CatalogosCache.Add(new CatalogoItemCacheLocal
            {
                TipoCatalogo = tipoCatalogo,
                IdItem = item.Id,
                Nombre = item.Nombre,
                UltimaActualizacion = DateTime.UtcNow
            });
        }

        await db.SaveChangesAsync();
    }

    public async Task<List<CatalogoItemDto>> ObtenerCatalogoCacheAsync(string tipoCatalogo)
    {
        await InicializarAsync();
        await using var db = CreateContext();

        var items = await db.CatalogosCache
            .AsNoTracking()
            .Where(c => c.TipoCatalogo == tipoCatalogo)
            .OrderBy(c => c.Nombre)
            .ToListAsync();

        return items.Select(c => new CatalogoItemDto
        {
            Id = c.IdItem,
            Nombre = c.Nombre
        }).ToList();
    }

    private static void MapearDtoEnCache(TicketDto t, TicketCacheLocal c, bool esOffline)
    {
        c.IdTicket = t.IdTicket;
        c.IdUsuario = t.IdUsuario;
        c.NombreUsuario = t.NombreUsuario ?? t.Usuario ?? string.Empty;
        c.DepartamentoUsuario = t.DepartamentoUsuario ?? string.Empty;
        c.IdTecnico = t.IdTecnico;
        c.NombreTecnico = t.NombreTecnico ?? t.TecnicoAsignado ?? string.Empty;
        c.Descripcion = t.Descripcion;
        c.Solucion = t.Solucion;
        c.Estatus = t.Estatus;
        c.Prioridad = t.Prioridad;
        c.FechaAlta = t.FechaAlta;
        c.FechaAtencion = t.FechaAtencion;
        c.FechaCierre = t.FechaCierre;
        c.Calificacion = t.Calificacion;
        c.ComentarioEvaluacion = t.ComentarioEvaluacion;
        c.FechaEvaluacion = t.FechaEvaluacion;
        c.EsOfflineLocal = esOffline;
        c.UltimaActualizacion = DateTime.UtcNow;
    }

    private static TicketDto MapearCacheEnDto(TicketCacheLocal c)
    {
        return new TicketDto
        {
            IdTicket = c.IdTicket,
            IdUsuario = c.IdUsuario,
            NombreUsuario = c.NombreUsuario,
            DepartamentoUsuario = c.DepartamentoUsuario,
            IdTecnico = c.IdTecnico,
            NombreTecnico = c.NombreTecnico,
            Descripcion = c.Descripcion,
            Solucion = c.Solucion,
            Estatus = c.Estatus,
            Prioridad = c.Prioridad,
            FechaAlta = c.FechaAlta,
            FechaAtencion = c.FechaAtencion,
            FechaCierre = c.FechaCierre,
            Calificacion = c.Calificacion,
            ComentarioEvaluacion = c.ComentarioEvaluacion,
            FechaEvaluacion = c.FechaEvaluacion
        };
    }
}
