namespace HSis.Data.Models
{
    public partial class Departamento
    {
        public int IdDepartamento { get; set; }

        public string? Nombre { get; set; }

        public string? Descripcion { get; set; }

        public virtual ICollection<Usuario> Usuarios { get; set; } = [];
    }
}

