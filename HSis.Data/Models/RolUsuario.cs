namespace HSis.Data.Models;

public partial class RolUsuario
{
    public int IdRol { get; set; }

    public string? Descripción { get; set; }

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
