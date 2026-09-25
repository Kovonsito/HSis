using FluentValidation;
using HSis.Contracts.DTOs;
using HSis.Contracts.Services;
using HSis.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace HSis.Logic.Services;

public sealed class EmpresaService(
    IDbContextFactory<HSisDbContext> dbContextFactory,
    IValidator<Empresa>? validator = null) : IEmpresaService
{
    public async Task<List<EmpresaDto>> ObtenerEmpresasAsync()
    {
        using var db = dbContextFactory.CreateDbContext();
        return await db.Empresas
            .AsNoTracking()
            .OrderBy(empresa => empresa.Nombre)
            .Select(empresa => new EmpresaDto
            {
                IdEmpresa = empresa.IdEmpresa,
                Nombre = empresa.Nombre ?? string.Empty,
                Calle = empresa.Calle,
                Numero = empresa.Numero,
                Colonia = empresa.Colonia,
                Telefono = empresa.Telefono
            })
            .ToListAsync();
    }

    public async Task<EmpresaDto> CrearEmpresaAsync(EmpresaCatalogoRequestDto request)
    {
        var entidad = new Empresa
        {
            Nombre = request.Nombre,
            Calle = request.Calle,
            Numero = request.Numero,
            Colonia = request.Colonia,
            Telefono = request.Telefono
        };
        await CatalogoValidation.ValidarAsync(validator, entidad);

        using var db = dbContextFactory.CreateDbContext();
        db.Empresas.Add(entidad);
        await db.SaveChangesAsync();
        return Convertir(entidad);
    }

    public async Task<EmpresaDto?> ActualizarEmpresaAsync(int idEmpresa, EmpresaCatalogoRequestDto request)
    {
        using var db = dbContextFactory.CreateDbContext();
        var entidad = await db.Empresas.FindAsync(idEmpresa);
        if (entidad is null)
        {
            return null;
        }

        entidad.Nombre = request.Nombre;
        entidad.Calle = request.Calle;
        entidad.Numero = request.Numero;
        entidad.Colonia = request.Colonia;
        entidad.Telefono = request.Telefono;
        await CatalogoValidation.ValidarAsync(validator, entidad);
        await db.SaveChangesAsync();
        return Convertir(entidad);
    }

    public async Task<bool> EliminarEmpresaAsync(int idEmpresa)
    {
        using var db = dbContextFactory.CreateDbContext();
        var entidad = await db.Empresas.FindAsync(idEmpresa);
        if (entidad is null)
        {
            return false;
        }

        db.Empresas.Remove(entidad);
        await db.SaveChangesAsync();
        return true;
    }

    private static EmpresaDto Convertir(Empresa entidad) => new()
    {
        IdEmpresa = entidad.IdEmpresa,
        Nombre = entidad.Nombre ?? string.Empty,
        Calle = entidad.Calle,
        Numero = entidad.Numero,
        Colonia = entidad.Colonia,
        Telefono = entidad.Telefono
    };
}
