using FluentValidation;
using HSis.Contracts.DTOs;
using HSis.Contracts.Services;
using HSis.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace HSis.Logic.Services;

public sealed class DepartamentoService(
    IDbContextFactory<HSisDbContext> dbContextFactory,
    IValidator<Departamento>? validator = null) : IDepartamentoService
{
    public async Task<List<DepartamentoDto>> ObtenerDepartamentosAsync()
    {
        using var db = dbContextFactory.CreateDbContext();
        return await db.Departamentos
            .AsNoTracking()
            .OrderBy(departamento => departamento.Nombre)
            .Select(departamento => new DepartamentoDto
            {
                IdDepartamento = departamento.IdDepartamento,
                Nombre = departamento.Nombre ?? string.Empty,
                Descripcion = departamento.Descripcion
            })
            .ToListAsync();
    }

    public async Task<DepartamentoDto> CrearDepartamentoAsync(DepartamentoCatalogoRequestDto request)
    {
        var entidad = new Departamento
        {
            Nombre = request.Nombre,
            Descripcion = request.Descripcion
        };
        await CatalogoValidation.ValidarAsync(validator, entidad);

        using var db = dbContextFactory.CreateDbContext();
        db.Departamentos.Add(entidad);
        await db.SaveChangesAsync();
        return Convertir(entidad);
    }

    public async Task<DepartamentoDto?> ActualizarDepartamentoAsync(int idDepartamento, DepartamentoCatalogoRequestDto request)
    {
        using var db = dbContextFactory.CreateDbContext();
        var entidad = await db.Departamentos.FindAsync(idDepartamento);
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

    public async Task<bool> EliminarDepartamentoAsync(int idDepartamento)
    {
        using var db = dbContextFactory.CreateDbContext();
        var entidad = await db.Departamentos.FindAsync(idDepartamento);
        if (entidad is null)
        {
            return false;
        }

        db.Departamentos.Remove(entidad);
        await db.SaveChangesAsync();
        return true;
    }

    private static DepartamentoDto Convertir(Departamento entidad) => new()
    {
        IdDepartamento = entidad.IdDepartamento,
        Nombre = entidad.Nombre ?? string.Empty,
        Descripcion = entidad.Descripcion
    };
}
