using HSis.Contracts.DTOs;

namespace HSis.Contracts.Services;

public interface IEmpresaService
{
    Task<List<EmpresaDto>> ObtenerEmpresasAsync();
    Task<EmpresaDto> CrearEmpresaAsync(EmpresaCatalogoRequestDto request);
    Task<EmpresaDto?> ActualizarEmpresaAsync(int idEmpresa, EmpresaCatalogoRequestDto request);
    Task<bool> EliminarEmpresaAsync(int idEmpresa);
}
