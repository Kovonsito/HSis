using FluentValidation;
using HSis.Contracts.DTOs;
using HSis.Contracts.Services;
using HSis.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace HSis.Logic.Services;

public sealed class PuestoService(
    IDbContextFactory<HSisDbContext> dbContextFactory,
    IValidator<Puesto>? validator = null) : IPuestoService
{
    public async Task<List<PuestoDto>> ObtenerPuestosAsync()
    {
        using var db = dbContextFactory.CreateDbContext();
        return await db.Puestos
            .AsNoTracking()
            .OrderBy(puesto => puesto.Nombre)
            .Select(puesto => new PuestoDto
            {
                IdPuesto = puesto.IdPuesto,
                Nombre = puesto.Nombre ?? string.Empty,
                Descripcion = puesto.Descripcion
            })
            .ToListAsync();
    }

    public async Task<PuestoDto> CrearPuestoAsync(PuestoCatalogoRequestDto request)
    {
        var entidad = new Puesto
        {
            Nombre = request.Nombre,
            Descripcion = request.Descripcion
        };
        await CatalogoValidation.ValidarAsync(validator, entidad);

        using var db = dbContextFactory.CreateDbContext();
        db.Puestos.Add(entidad);
        await db.SaveChangesAsync();
        return Convertir(entidad);
    }

    public async Task<PuestoDto?> ActualizarPuestoAsync(int idPuesto, PuestoCatalogoRequestDto request)
    {
        using var db = dbContextFactory.CreateDbContext();
        var entidad = await db.Puestos.FindAsync(idPuesto);
        if (entidad is null)
        {
            return null;
        }

        entidad.Nombre = request.Nombre;
        entidad.Descripcion = request.Descripcion;
        await CatalogoValidation.ValidarAsync(validator, entidad);
        await db.SaveChangesAsync();
        return Convertir(entidad);
    }

    public async Task<bool> EliminarPuestoAsync(int idPuesto)
    {
        using var db = dbContextFactory.CreateDbContext();
        var entidad = await db.Puestos.FindAsync(idPuesto);
        if (entidad is null)
        {
            return false;
        }

        db.Puestos.Remove(entidad);
        await db.SaveChangesAsync();
        return true;
    }

    private static PuestoDto Convertir(Puesto entidad) => new()
    {
        IdPuesto = entidad.IdPuesto,
        Nombre = entidad.Nombre ?? string.Empty,
        Descripcion = entidad.Descripcion
    };
}
