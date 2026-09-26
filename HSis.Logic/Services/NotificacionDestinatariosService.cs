using HSis.Contracts.Constants;
using HSis.Contracts.Services;
using HSis.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace HSis.Logic.Services;

public sealed class NotificacionDestinatariosService(
    IDbContextFactory<HSisDbContext> dbContextFactory) : INotificacionDestinatariosService
{
    public async Task<IReadOnlyList<int>> ObtenerDestinatariosNuevoTicketAsync(
        CancellationToken cancellationToken = default)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        return await db.Usuarios
            .AsNoTracking()
            .Where(usuario =>
                usuario.IdRol == (int)RolUsuarioEnum.Tecnico ||
                usuario.IdRol == (int)RolUsuarioEnum.Administrador)
            .Select(usuario => usuario.IdUsuario)
            .Distinct()
            .OrderBy(idUsuario => idUsuario)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<int>> ObtenerDestinatariosCalificacionAsync(
        int? tecnicoId,
        CancellationToken cancellationToken = default)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var administradores = await db.Usuarios
            .AsNoTracking()
            .Where(usuario => usuario.IdRol == (int)RolUsuarioEnum.Administrador)
            .Select(usuario => usuario.IdUsuario)
            .ToListAsync(cancellationToken);

        if (tecnicoId is > 0)
        {
            administradores.Add(tecnicoId.Value);
        }

        return administradores
            .Distinct()
            .OrderBy(idUsuario => idUsuario)
            .ToList();
    }

    public async Task<IReadOnlyList<int>> ObtenerDestinatariosCambioEstadoAsync(
        int usuarioPropietarioId,
        int? tecnicoId,
        CancellationToken cancellationToken = default)
    {
        var destinatarios = new HashSet<int>();

        if (usuarioPropietarioId > 0)
        {
            destinatarios.Add(usuarioPropietarioId);
        }

        if (tecnicoId is > 0)
        {
            destinatarios.Add(tecnicoId.Value);
        }

        await using var db = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        var administradores = await db.Usuarios
            .AsNoTracking()
            .Where(usuario => usuario.IdRol == (int)RolUsuarioEnum.Administrador)
            .Select(usuario => usuario.IdUsuario)
            .ToListAsync(cancellationToken);

        destinatarios.UnionWith(administradores);
        return destinatarios.OrderBy(idUsuario => idUsuario).ToList();
    }

    public async Task<IReadOnlyList<int>> ObtenerDestinatariosAsignacionAsync(
        int usuarioPropietarioId,
        int? tecnicoAnteriorId,
        int? tecnicoNuevoId,
        CancellationToken cancellationToken = default)
    {
        var destinatarios = new HashSet<int>();

        if (usuarioPropietarioId > 0)
        {
            destinatarios.Add(usuarioPropietarioId);
        }

        if (tecnicoAnteriorId is > 0)
        {
            destinatarios.Add(tecnicoAnteriorId.Value);
        }

        if (tecnicoNuevoId is > 0)
        {
            destinatarios.Add(tecnicoNuevoId.Value);
        }

        await using var db = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        var administradores = await db.Usuarios
            .AsNoTracking()
            .Where(usuario => usuario.IdRol == (int)RolUsuarioEnum.Administrador)
            .Select(usuario => usuario.IdUsuario)
            .ToListAsync(cancellationToken);

        destinatarios.UnionWith(administradores);
        return destinatarios.OrderBy(idUsuario => idUsuario).ToList();
    }

    public Task<IReadOnlyList<int>> ObtenerDestinatariosCambioPrioridadAsync(
        int usuarioPropietarioId,
        int? tecnicoId,
        CancellationToken cancellationToken = default)
        => ObtenerDestinatariosTicketConAdministradoresAsync(
            usuarioPropietarioId,
            tecnicoId,
            cancellationToken);

    public Task<IReadOnlyList<int>> ObtenerDestinatariosActualizacionSolucionAsync(
        int usuarioPropietarioId,
        int? tecnicoId,
        CancellationToken cancellationToken = default)
        => ObtenerDestinatariosTicketConAdministradoresAsync(
            usuarioPropietarioId,
            tecnicoId,
            cancellationToken);

    public Task<IReadOnlyList<int>> ObtenerDestinatariosCambioMaterialTicketAsync(
        int usuarioPropietarioId,
        int? tecnicoId,
        CancellationToken cancellationToken = default)
        => ObtenerDestinatariosTicketConAdministradoresAsync(
            usuarioPropietarioId,
            tecnicoId,
            cancellationToken);

    public async Task<IReadOnlyList<int>> ObtenerDestinatariosMovimientoMaterialAsync(
        CancellationToken cancellationToken = default)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        return await db.Usuarios
            .AsNoTracking()
            .Where(usuario =>
                usuario.IdRol == (int)RolUsuarioEnum.Tecnico ||
                usuario.IdRol == (int)RolUsuarioEnum.Administrador)
            .Select(usuario => usuario.IdUsuario)
            .Distinct()
            .OrderBy(idUsuario => idUsuario)
            .ToListAsync(cancellationToken);
    }

    private async Task<IReadOnlyList<int>> ObtenerDestinatariosTicketConAdministradoresAsync(
        int usuarioPropietarioId,
        int? tecnicoId,
        CancellationToken cancellationToken)
    {
        var destinatarios = new HashSet<int>();
        if (usuarioPropietarioId > 0)
        {
            destinatarios.Add(usuarioPropietarioId);
        }

        if (tecnicoId is > 0)
        {
            destinatarios.Add(tecnicoId.Value);
        }

        await using var db = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        var administradores = await db.Usuarios
            .AsNoTracking()
            .Where(usuario => usuario.IdRol == (int)RolUsuarioEnum.Administrador)
            .Select(usuario => usuario.IdUsuario)
            .ToListAsync(cancellationToken);

        destinatarios.UnionWith(administradores);
        return destinatarios.OrderBy(idUsuario => idUsuario).ToList();
    }
}
