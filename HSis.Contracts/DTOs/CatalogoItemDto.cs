namespace HSis.Logic.DTOs
{
    public class CatalogoItemDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public int? IdPadre { get; set; }
        public string? NombrePadre { get; set; }
    }

    public class DepartamentoDto
    {
        public int IdDepartamento { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int? IdSucursal { get; set; }
        public string? SucursalNombre { get; set; }
    }

    public class PuestoDto
    {
        public int IdPuesto { get; set; }
        public string Nombre { get; set; } = string.Empty;
    }

    public class SucursalDto
    {
        public int IdSucursal { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int? IdEmpresa { get; set; }
        public string? EmpresaNombre { get; set; }
    }

    public class EmpresaDto
    {
        public int IdEmpresa { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Telefono { get; set; }
    }

    public class RolUsuarioDto
    {
        public int IdRol { get; set; }
        public string Descripcion { get; set; } = string.Empty;
    }
}
