using HSis.Contracts.DTOs;

namespace HSis.Contracts.Services;

public interface IDepartamentoService
{
    Task<List<DepartamentoDto>> ObtenerDepartamentosAsync();
    Task<DepartamentoDto> CrearDepartamentoAsync(DepartamentoCatalogoRequestDto request);
    Task<DepartamentoDto?> ActualizarDepartamentoAsync(int idDepartamento, DepartamentoCatalogoRequestDto request);
    Task<bool> EliminarDepartamentoAsync(int idDepartamento);
}
