namespace HSis.Contracts.DTOs
{
    public interface IElementoCatalogo
    {
        int Id { get; }
        string Nombre { get; }
    }

    public class CatalogoItemDto : IElementoCatalogo
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public int? IdPadre { get; set; }
        public string? NombrePadre { get; set; }
    }

    public class DepartamentoDto : IElementoCatalogo
    {
        public int IdDepartamento { get; set; }
        public int Id => IdDepartamento;
        public string Nombre { get; set; } = string.Empty;
        public int? IdSucursal { get; set; }
        public string? SucursalNombre { get; set; }
    }

    public class PuestoDto : IElementoCatalogo
    {
        public int IdPuesto { get; set; }
        public int Id => IdPuesto;
        public string Nombre { get; set; } = string.Empty;
    }

    public class SucursalDto : IElementoCatalogo
    {
        public int IdSucursal { get; set; }
        public int Id => IdSucursal;
        public string Nombre { get; set; } = string.Empty;
        public int? IdEmpresa { get; set; }
        public string? EmpresaNombre { get; set; }
    }

    public class EmpresaDto : IElementoCatalogo
    {
        public int IdEmpresa { get; set; }
        public int Id => IdEmpresa;
        public string Nombre { get; set; } = string.Empty;
        public string? Telefono { get; set; }
    }

    public class RolUsuarioDto : IElementoCatalogo
    {
        public int IdRol { get; set; }
        public int Id => IdRol;
        public string Descripcion { get; set; } = string.Empty;
        public string Nombre => Descripcion;
    }
}
