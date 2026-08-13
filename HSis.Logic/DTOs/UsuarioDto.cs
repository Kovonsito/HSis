namespace HSis.Logic.DTOs
{
    public class UsuarioDto
    {
        public int IdUsuario { get; set; }
        public string? Nombre { get; set; }
        public int? IdDepartamento { get; set; }
        public int? IdPuesto { get; set; }
        public int? IdSucursal { get; set; }
        public int? IdRol { get; set; }
        public string? Contraseña { get; set; }

        public string? DepartamentoNombre { get; set; }
        public string? PuestoNombre { get; set; }
        public string? SucursalNombre { get; set; }
    }
}

