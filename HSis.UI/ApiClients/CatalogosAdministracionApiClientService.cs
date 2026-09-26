using System.Net.Http;
using System.Net;
using System.Net.Http.Json;
using HSis.Contracts.DTOs;
using HSis.Contracts.Services;

namespace HSis.UI.ApiClients;

public sealed class CatalogosAdministracionApiClientService(HttpClient httpClient) :
    IDepartamentoService,
    ISucursalService,
    IEmpresaService,
    IPuestoService,
    IRolUsuarioService
{
    public Task<List<DepartamentoDto>> ObtenerDepartamentosAsync()
        => ObtenerListaAsync<DepartamentoDto>("api/Catalogos/departamentos");

    public Task<DepartamentoDto> CrearDepartamentoAsync(DepartamentoCatalogoRequestDto request)
        => CrearAsync<DepartamentoDto>("api/Catalogos/departamentos", request, "departamento");

    public Task<DepartamentoDto?> ActualizarDepartamentoAsync(int idDepartamento, DepartamentoCatalogoRequestDto request)
        => ActualizarAsync<DepartamentoDto>($"api/Catalogos/departamentos/{idDepartamento}", request);

    public Task<bool> EliminarDepartamentoAsync(int idDepartamento)
        => EliminarAsync($"api/Catalogos/departamentos/{idDepartamento}");

    public Task<List<SucursalDto>> ObtenerSucursalesAsync()
        => ObtenerListaAsync<SucursalDto>("api/Catalogos/sucursales");

    public Task<SucursalDto> CrearSucursalAsync(SucursalCatalogoRequestDto request)
        => CrearAsync<SucursalDto>("api/Catalogos/sucursales", request, "sucursal");

    public Task<SucursalDto?> ActualizarSucursalAsync(int idSucursal, SucursalCatalogoRequestDto request)
        => ActualizarAsync<SucursalDto>($"api/Catalogos/sucursales/{idSucursal}", request);

    public Task<bool> EliminarSucursalAsync(int idSucursal)
        => EliminarAsync($"api/Catalogos/sucursales/{idSucursal}");

    public Task<List<EmpresaDto>> ObtenerEmpresasAsync()
        => ObtenerListaAsync<EmpresaDto>("api/Catalogos/empresas");

    public Task<EmpresaDto> CrearEmpresaAsync(EmpresaCatalogoRequestDto request)
        => CrearAsync<EmpresaDto>("api/Catalogos/empresas", request, "empresa");

    public Task<EmpresaDto?> ActualizarEmpresaAsync(int idEmpresa, EmpresaCatalogoRequestDto request)
        => ActualizarAsync<EmpresaDto>($"api/Catalogos/empresas/{idEmpresa}", request);

    public Task<bool> EliminarEmpresaAsync(int idEmpresa)
        => EliminarAsync($"api/Catalogos/empresas/{idEmpresa}");

    public Task<List<PuestoDto>> ObtenerPuestosAsync()
        => ObtenerListaAsync<PuestoDto>("api/Catalogos/puestos");

    public Task<PuestoDto> CrearPuestoAsync(PuestoCatalogoRequestDto request)
        => CrearAsync<PuestoDto>("api/Catalogos/puestos", request, "puesto");

    public Task<PuestoDto?> ActualizarPuestoAsync(int idPuesto, PuestoCatalogoRequestDto request)
        => ActualizarAsync<PuestoDto>($"api/Catalogos/puestos/{idPuesto}", request);

    public Task<bool> EliminarPuestoAsync(int idPuesto)
        => EliminarAsync($"api/Catalogos/puestos/{idPuesto}");

    public Task<List<RolUsuarioDto>> ObtenerRolesUsuarioAsync()
        => ObtenerListaAsync<RolUsuarioDto>("api/Catalogos/roles");

    public Task<RolUsuarioDto> CrearRolUsuarioAsync(RolUsuarioCatalogoRequestDto request)
        => CrearAsync<RolUsuarioDto>("api/Catalogos/roles", request, "rol");

    public Task<RolUsuarioDto?> ActualizarRolUsuarioAsync(int idRol, RolUsuarioCatalogoRequestDto request)
        => ActualizarAsync<RolUsuarioDto>($"api/Catalogos/roles/{idRol}", request);

    public Task<bool> EliminarRolUsuarioAsync(int idRol)
        => EliminarAsync($"api/Catalogos/roles/{idRol}");

    private async Task<List<T>> ObtenerListaAsync<T>(string route)
        => await httpClient.GetFromJsonWithDetailsAsync<List<T>>(route) ?? [];

    private async Task<T> CrearAsync<T>(string route, object request, string nombre)
    {
        using var response = await httpClient.PostAsJsonAsync(route, request);
        return await response.ReadRequiredJsonWithDetailsAsync<T>($"{nombre} creado");
    }

    private async Task<T?> ActualizarAsync<T>(string route, object request)
    {
        using var response = await httpClient.PutAsJsonAsync(route, request);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return default;
        }

        return await response.ReadFromJsonWithDetailsAsync<T>();
    }

    private async Task<bool> EliminarAsync(string route)
    {
        using var response = await httpClient.DeleteAsync(route);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }

        await response.EnsureSuccessStatusCodeWithDetailsAsync();
        return true;
    }
}
