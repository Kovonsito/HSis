using FluentValidation;
using HSis.Contracts.DTOs;
using HSis.Contracts.Services;
using HSis.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace HSis.Logic.Services;

public sealed class RolUsuarioService(
    IDbContextFactory<HSisDbContext> dbContextFactory,
    IValidator<RolUsuario>? validator = null) : IRolUsuarioService
{
    public async Task<List<RolUsuarioDto>> ObtenerRolesUsuarioAsync()
    {
        using var db = dbContextFactory.CreateDbContext();
        return await db.RolesUsuarios
            .AsNoTracking()
            .OrderBy(rol => rol.Descripcion)
            .Select(rol => new RolUsuarioDto
            {
                IdRol = rol.IdRol,
                Descripcion = rol.Descripcion ?? string.Empty
            })
            .ToListAsync();
    }

    public async Task<RolUsuarioDto> CrearRolUsuarioAsync(RolUsuarioCatalogoRequestDto request)
    {
        var entidad = new RolUsuario
        {
            Descripcion = request.Descripcion
        };
        await CatalogoValidation.ValidarAsync(validator, entidad);

        using var db = dbContextFactory.CreateDbContext();
        db.RolesUsuarios.Add(entidad);
        await db.SaveChangesAsync();
        return Convertir(entidad);
    }

    public async Task<RolUsuarioDto?> ActualizarRolUsuarioAsync(int idRol, RolUsuarioCatalogoRequestDto request)
    {
        using var db = dbContextFactory.CreateDbContext();
        var entidad = await db.RolesUsuarios.FindAsync(idRol);
        if (entidad is null)
        {
            return null;
        }

        entidad.Descripcion = request.Descripcion;
        await CatalogoValidation.ValidarAsync(validator, entidad);
        await db.SaveChangesAsync();
        return Convertir(entidad);
    }

    public async Task<bool> EliminarRolUsuarioAsync(int idRol)
    {
        using var db = dbContextFactory.CreateDbContext();
        var entidad = await db.RolesUsuarios.FindAsync(idRol);
        if (entidad is null)
        {
            return false;
        }

        db.RolesUsuarios.Remove(entidad);
        await db.SaveChangesAsync();
        return true;
    }

    private static RolUsuarioDto Convertir(RolUsuario entidad) => new()
    {
        IdRol = entidad.IdRol,
        Descripcion = entidad.Descripcion ?? string.Empty
    };
}
