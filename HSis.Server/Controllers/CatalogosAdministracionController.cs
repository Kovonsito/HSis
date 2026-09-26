using HSis.Contracts.DTOs;
using HSis.Contracts.Errors;
using HSis.Contracts.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HSis.Server.Controllers;

[ApiController]
[Authorize]
[Route("api/Catalogos")]
public sealed class CatalogosAdministracionController(
    IMaterialService materialService,
    IUsuarioService usuarioService,
    IDepartamentoService departamentoService,
    ISucursalService sucursalService,
    IEmpresaService empresaService,
    IPuestoService puestoService,
    IRolUsuarioService rolUsuarioService,
    ILogger<CatalogosAdministracionController> logger) : ControllerBase
{
    [HttpGet("materiales")]
    public async Task<ActionResult<List<MaterialDto>>> ObtenerMateriales()
        => Ok(await materialService.ObtenerMaterialesAsync());

    [HttpPost("materiales")]
    public async Task<ActionResult<MaterialDto>> CrearMaterial(MaterialCatalogoRequestDto request)
    {
        var material = await materialService.CrearMaterialAsync(request);
        return Created($"api/Catalogos/materiales/{material.IdMaterial}", material);
    }

    [HttpPut("materiales/{idMaterial:int}")]
    public async Task<ActionResult<MaterialDto>> ActualizarMaterial(int idMaterial, MaterialCatalogoRequestDto request)
    {
        var material = await materialService.ActualizarMaterialAsync(idMaterial, request);
        return material is null ? RecursoNoEncontrado("material", idMaterial) : Ok(material);
    }

    [HttpDelete("materiales/{idMaterial:int}")]
    public async Task<IActionResult> EliminarMaterial(int idMaterial)
        => await materialService.EliminarMaterialAsync(idMaterial) ? NoContent() : RecursoNoEncontrado("material", idMaterial);

    [HttpGet("usuarios")]
    public async Task<ActionResult<List<UsuarioDto>>> ObtenerUsuarios()
        => Ok(await usuarioService.ObtenerUsuariosAsync());

    [HttpPost("usuarios")]
    public async Task<ActionResult<UsuarioDto>> CrearUsuario(UsuarioCatalogoRequestDto request)
    {
        var usuario = await usuarioService.CrearUsuarioAsync(request);
        return Created($"api/Catalogos/usuarios/{usuario.IdUsuario}", usuario);
    }

    [HttpPut("usuarios/{idUsuario:int}")]
    public async Task<ActionResult<UsuarioDto>> ActualizarUsuario(int idUsuario, UsuarioCatalogoRequestDto request)
    {
        var usuario = await usuarioService.ActualizarUsuarioAsync(idUsuario, request);
        return usuario is null ? RecursoNoEncontrado("usuario", idUsuario) : Ok(usuario);
    }

    [HttpDelete("usuarios/{idUsuario:int}")]
    public async Task<IActionResult> EliminarUsuario(int idUsuario)
        => await usuarioService.EliminarUsuarioAsync(idUsuario) ? NoContent() : RecursoNoEncontrado("usuario", idUsuario);

    [HttpGet("departamentos")]
    public async Task<ActionResult<List<DepartamentoDto>>> ObtenerDepartamentos()
        => Ok(await departamentoService.ObtenerDepartamentosAsync());

    [HttpPost("departamentos")]
    public async Task<ActionResult<DepartamentoDto>> CrearDepartamento(DepartamentoCatalogoRequestDto request)
    {
        var departamento = await departamentoService.CrearDepartamentoAsync(request);
        return Created($"api/Catalogos/departamentos/{departamento.IdDepartamento}", departamento);
    }

    [HttpPut("departamentos/{idDepartamento:int}")]
    public async Task<ActionResult<DepartamentoDto>> ActualizarDepartamento(int idDepartamento, DepartamentoCatalogoRequestDto request)
    {
        var departamento = await departamentoService.ActualizarDepartamentoAsync(idDepartamento, request);
        return departamento is null ? RecursoNoEncontrado("departamento", idDepartamento) : Ok(departamento);
    }

    [HttpDelete("departamentos/{idDepartamento:int}")]
    public async Task<IActionResult> EliminarDepartamento(int idDepartamento)
        => await departamentoService.EliminarDepartamentoAsync(idDepartamento) ? NoContent() : RecursoNoEncontrado("departamento", idDepartamento);

    [HttpGet("sucursales")]
    public async Task<ActionResult<List<SucursalDto>>> ObtenerSucursales()
        => Ok(await sucursalService.ObtenerSucursalesAsync());

    [HttpPost("sucursales")]
    public async Task<ActionResult<SucursalDto>> CrearSucursal(SucursalCatalogoRequestDto request)
    {
        var sucursal = await sucursalService.CrearSucursalAsync(request);
        return Created($"api/Catalogos/sucursales/{sucursal.IdSucursal}", sucursal);
    }

    [HttpPut("sucursales/{idSucursal:int}")]
    public async Task<ActionResult<SucursalDto>> ActualizarSucursal(int idSucursal, SucursalCatalogoRequestDto request)
    {
        var sucursal = await sucursalService.ActualizarSucursalAsync(idSucursal, request);
        return sucursal is null ? RecursoNoEncontrado("sucursal", idSucursal) : Ok(sucursal);
    }

    [HttpDelete("sucursales/{idSucursal:int}")]
    public async Task<IActionResult> EliminarSucursal(int idSucursal)
        => await sucursalService.EliminarSucursalAsync(idSucursal) ? NoContent() : RecursoNoEncontrado("sucursal", idSucursal);

    [HttpGet("empresas")]
    public async Task<ActionResult<List<EmpresaDto>>> ObtenerEmpresas()
        => Ok(await empresaService.ObtenerEmpresasAsync());

    [HttpPost("empresas")]
    public async Task<ActionResult<EmpresaDto>> CrearEmpresa(EmpresaCatalogoRequestDto request)
    {
        var empresa = await empresaService.CrearEmpresaAsync(request);
        return Created($"api/Catalogos/empresas/{empresa.IdEmpresa}", empresa);
    }

    [HttpPut("empresas/{idEmpresa:int}")]
    public async Task<ActionResult<EmpresaDto>> ActualizarEmpresa(int idEmpresa, EmpresaCatalogoRequestDto request)
    {
        var empresa = await empresaService.ActualizarEmpresaAsync(idEmpresa, request);
        return empresa is null ? RecursoNoEncontrado("empresa", idEmpresa) : Ok(empresa);
    }

    [HttpDelete("empresas/{idEmpresa:int}")]
    public async Task<IActionResult> EliminarEmpresa(int idEmpresa)
        => await empresaService.EliminarEmpresaAsync(idEmpresa) ? NoContent() : RecursoNoEncontrado("empresa", idEmpresa);

    [HttpGet("puestos")]
    public async Task<ActionResult<List<PuestoDto>>> ObtenerPuestos()
        => Ok(await puestoService.ObtenerPuestosAsync());

    [HttpPost("puestos")]
    public async Task<ActionResult<PuestoDto>> CrearPuesto(PuestoCatalogoRequestDto request)
    {
        var puesto = await puestoService.CrearPuestoAsync(request);
        return Created($"api/Catalogos/puestos/{puesto.IdPuesto}", puesto);
    }

    [HttpPut("puestos/{idPuesto:int}")]
    public async Task<ActionResult<PuestoDto>> ActualizarPuesto(int idPuesto, PuestoCatalogoRequestDto request)
    {
        var puesto = await puestoService.ActualizarPuestoAsync(idPuesto, request);
        return puesto is null ? RecursoNoEncontrado("puesto", idPuesto) : Ok(puesto);
    }

    [HttpDelete("puestos/{idPuesto:int}")]
    public async Task<IActionResult> EliminarPuesto(int idPuesto)
        => await puestoService.EliminarPuestoAsync(idPuesto) ? NoContent() : RecursoNoEncontrado("puesto", idPuesto);

    [HttpGet("roles")]
    public async Task<ActionResult<List<RolUsuarioDto>>> ObtenerRoles()
        => Ok(await rolUsuarioService.ObtenerRolesUsuarioAsync());

    [HttpPost("roles")]
    public async Task<ActionResult<RolUsuarioDto>> CrearRol(RolUsuarioCatalogoRequestDto request)
    {
        var rol = await rolUsuarioService.CrearRolUsuarioAsync(request);
        return Created($"api/Catalogos/roles/{rol.IdRol}", rol);
    }

    [HttpPut("roles/{idRol:int}")]
    public async Task<ActionResult<RolUsuarioDto>> ActualizarRol(int idRol, RolUsuarioCatalogoRequestDto request)
    {
        var rol = await rolUsuarioService.ActualizarRolUsuarioAsync(idRol, request);
        return rol is null ? RecursoNoEncontrado("rol de usuario", idRol) : Ok(rol);
    }

    [HttpDelete("roles/{idRol:int}")]
    public async Task<IActionResult> EliminarRol(int idRol)
        => await rolUsuarioService.EliminarRolUsuarioAsync(idRol) ? NoContent() : RecursoNoEncontrado("rol de usuario", idRol);

    private ObjectResult RecursoNoEncontrado(string recurso, int id)
    {
        logger.LogWarning(
            "No se encontró el recurso de catálogo {Recurso} con identificador {Id}. TraceId: {TraceId}",
            recurso,
            id,
            HttpContext.TraceIdentifier);

        return Problem(
            statusCode: StatusCodes.Status404NotFound,
            title: "Registro no encontrado",
            detail: $"No se encontró el {recurso} con identificador {id}. Compruebe el dato e inténtelo de nuevo.",
            extensions: new Dictionary<string, object?>
            {
                ["code"] = ApiErrorCodes.ResourceNotFound,
                ["traceId"] = HttpContext.TraceIdentifier
            });
    }
}
