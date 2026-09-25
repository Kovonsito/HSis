namespace HSis.Contracts.DTOs;

/// <summary>Datos editables de un material del catálogo.</summary>
public sealed class MaterialCatalogoRequestDto
{
    public string Nombre { get; set; } = string.Empty;
    public int StockActual { get; set; }
    public string UnidadMedida { get; set; } = string.Empty;
    public decimal CostoUnitario { get; set; }
}

/// <summary>Datos editables de un usuario del catálogo.</summary>
public sealed class UsuarioCatalogoRequestDto
{
    public string? Nombre { get; set; }
    public int? IdDepartamento { get; set; }
    public int? IdPuesto { get; set; }
    public int? IdSucursal { get; set; }
    public int? IdRol { get; set; }
    public string? Contraseña { get; set; }
}

/// <summary>Datos editables de un departamento del catálogo.</summary>
public sealed class DepartamentoCatalogoRequestDto
{
    public string? Nombre { get; set; }
    public string? Descripcion { get; set; }
}

/// <summary>Datos editables de una sucursal del catálogo.</summary>
public sealed class SucursalCatalogoRequestDto
{
    public string? Nombre { get; set; }
    public string? Calle { get; set; }
    public string? Numero { get; set; }
    public string? Colonia { get; set; }
    public string? Telefono { get; set; }
    public int? IdEmpresa { get; set; }
}

/// <summary>Datos editables de una empresa del catálogo.</summary>
public sealed class EmpresaCatalogoRequestDto
{
    public string? Nombre { get; set; }
    public string? Calle { get; set; }
    public string? Numero { get; set; }
    public string? Colonia { get; set; }
    public string? Telefono { get; set; }
}

/// <summary>Datos editables de un puesto del catálogo.</summary>
public sealed class PuestoCatalogoRequestDto
{
    public string? Nombre { get; set; }
    public string? Descripcion { get; set; }
}

/// <summary>Datos editables de un rol de usuario del catálogo.</summary>
public sealed class RolUsuarioCatalogoRequestDto
{
    public string? Descripcion { get; set; }
}
