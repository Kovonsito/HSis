using HSis.Contracts.DTOs;
using HSis.Contracts.Services;
using HSis.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace HSis.Logic.Services;

public sealed class NotificacionService(IDbContextFactory<HSisDbContext> dbContextFactory) : INotificacionService
{
    private const int MaximoHistorial = 50;

    public async Task<NotificacionesResumenDto> ObtenerResumenAsync(
        int usuarioId,
        CancellationToken cancellationToken = default)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var query = db.Notificaciones
            .AsNoTracking()
            .Where(notificacion => notificacion.UsuarioDestinoId == usuarioId);

        var noLeidas = await query.CountAsync(notificacion => !notificacion.Leido, cancellationToken);
        var notificaciones = await query
            .OrderByDescending(notificacion => notificacion.FechaCreacion)
            .Take(MaximoHistorial)
            .ToListAsync(cancellationToken);

        return new NotificacionesResumenDto(
            notificaciones.Select(Mapear).ToList(),
            noLeidas);
    }

    public async Task<IReadOnlyList<NotificacionDto>> ObtenerNuevasAsync(
        int usuarioId,
        int desdeId,
        CancellationToken cancellationToken = default)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var notificaciones = await db.Notificaciones
            .AsNoTracking()
            .Where(notificacion =>
                notificacion.UsuarioDestinoId == usuarioId &&
                notificacion.IdNotificacion > desdeId)
            .OrderBy(notificacion => notificacion.IdNotificacion)
            .Take(MaximoHistorial)
            .ToListAsync(cancellationToken);

        return notificaciones.Select(Mapear).ToList();
    }

    public async Task<bool> MarcarComoLeidaAsync(
        int usuarioId,
        int notificacionId,
        CancellationToken cancellationToken = default)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var notificacion = await db.Notificaciones
            .SingleOrDefaultAsync(
                item => item.IdNotificacion == notificacionId && item.UsuarioDestinoId == usuarioId,
                cancellationToken);

        if (notificacion is null)
        {
            return false;
        }

        if (!notificacion.Leido)
        {
            notificacion.Leido = true;
            await db.SaveChangesAsync(cancellationToken);
        }

        return true;
    }

    public async Task<int> MarcarTodasComoLeidasAsync(
        int usuarioId,
        CancellationToken cancellationToken = default)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var notificaciones = await db.Notificaciones
            .Where(item => item.UsuarioDestinoId == usuarioId && !item.Leido)
            .ToListAsync(cancellationToken);

        if (notificaciones.Count == 0)
        {
            return 0;
        }

        foreach (var notificacion in notificaciones)
        {
            notificacion.Leido = true;
        }

        await db.SaveChangesAsync(cancellationToken);
        return notificaciones.Count;
    }

    public async Task<int> LimpiarTodasAsync(
        int usuarioId,
        CancellationToken cancellationToken = default)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var notificaciones = await db.Notificaciones
            .Where(item => item.UsuarioDestinoId == usuarioId)
            .ToListAsync(cancellationToken);

        if (notificaciones.Count == 0)
        {
            return 0;
        }

        db.Notificaciones.RemoveRange(notificaciones);
        await db.SaveChangesAsync(cancellationToken);
        return notificaciones.Count;
    }

    private static NotificacionDto Mapear(Notificacion notificacion)
        => new(
            notificacion.IdNotificacion,
            notificacion.TicketId,
            notificacion.Tipo,
            notificacion.Mensaje,
            new DateTimeOffset(notificacion.FechaCreacion),
            notificacion.Leido,
            notificacion.MaterialId);
}
