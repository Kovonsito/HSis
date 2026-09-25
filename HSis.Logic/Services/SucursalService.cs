using FluentValidation;
using HSis.Contracts.DTOs;
using HSis.Contracts.Services;
using HSis.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace HSis.Logic.Services;

public sealed class SucursalService(
    IDbContextFactory<HSisDbContext> dbContextFactory,
    IValidator<Sucursal>? validator = null) : ISucursalService
{
    public async Task<List<SucursalDto>> ObtenerSucursalesAsync()
    {
        using var db = dbContextFactory.CreateDbContext();
        return await db.Sucursales
            .AsNoTracking()
            .Include(sucursal => sucursal.Empresa)
            .OrderBy(sucursal => sucursal.Nombre)
            .Select(sucursal => new SucursalDto
            {
                IdSucursal = sucursal.IdSucursal,
                Nombre = sucursal.Nombre ?? string.Empty,
                Calle = sucursal.Calle,
                Numero = sucursal.Numero,
                Colonia = sucursal.Colonia,
                Telefono = sucursal.Telefono,
                IdEmpresa = sucursal.IdEmpresa,
                EmpresaNombre = sucursal.Empresa == null ? null : sucursal.Empresa.Nombre
            })
            .ToListAsync();
    }

    public async Task<SucursalDto> CrearSucursalAsync(SucursalCatalogoRequestDto request)
    {
        var entidad = new Sucursal
        {
            Nombre = request.Nombre,
            Calle = request.Calle,
            Numero = request.Numero,
            Colonia = request.Colonia,
            Telefono = request.Telefono,
            IdEmpresa = request.IdEmpresa
        };
        await CatalogoValidation.ValidarAsync(validator, entidad);

        using var db = dbContextFactory.CreateDbContext();
        db.Sucursales.Add(entidad);
        await db.SaveChangesAsync();
        await db.Entry(entidad).Reference(sucursal => sucursal.Empresa).LoadAsync();
        return Convertir(entidad);
    }

    public async Task<SucursalDto?> ActualizarSucursalAsync(int idSucursal, SucursalCatalogoRequestDto request)
    {
        using var db = dbContextFactory.CreateDbContext();
        var entidad = await db.Sucursales.FindAsync(idSucursal);
        if (entidad is null)
        {
            return null;
        }

        entidad.Nombre = request.Nombre;
        entidad.Calle = request.Calle;
        entidad.Numero = request.Numero;
        entidad.Colonia = request.Colonia;
        entidad.Telefono = request.Telefono;
        entidad.IdEmpresa = request.IdEmpresa;
        await CatalogoValidation.ValidarAsync(validator, entidad);
        await db.SaveChangesAsync();
        await db.Entry(entidad).Reference(sucursal => sucursal.Empresa).LoadAsync();
        return Convertir(entidad);
    }

    public async Task<bool> EliminarSucursalAsync(int idSucursal)
    {
        using var db = dbContextFactory.CreateDbContext();
        var entidad = await db.Sucursales.FindAsync(idSucursal);
        if (entidad is null)
        {
            return false;
        }

        db.Sucursales.Remove(entidad);
        await db.SaveChangesAsync();
        return true;
    }

    private static SucursalDto Convertir(Sucursal entidad) => new()
    {
        IdSucursal = entidad.IdSucursal,
        Nombre = entidad.Nombre ?? string.Empty,
        Calle = entidad.Calle,
        Numero = entidad.Numero,
        Colonia = entidad.Colonia,
        Telefono = entidad.Telefono,
        IdEmpresa = entidad.IdEmpresa,
        EmpresaNombre = entidad.Empresa?.Nombre
    };
}
