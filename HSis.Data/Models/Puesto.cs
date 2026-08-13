namespace HSis.Data.Models
{
    public partial class Puesto
    {
        public int IdPuesto { get; set; }

        public string? Nombre { get; set; }

        public string? Descripcion { get; set; }

        public virtual ICollection<Usuario> Usuarios { get; set; } = [];
    }
}

