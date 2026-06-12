namespace HSis.Data.Models;

public partial class Notificacion
{
    public int IdNotificacion { get; set; }

    public int UsuarioDestinoId { get; set; }

    public string Mensaje { get; set; } = null!;

    public string Tipo { get; set; } = null!;

    public bool Leido { get; set; }

    public DateTime FechaCreacion { get; set; }

    public virtual Usuario UsuarioDestinoNavigation { get; set; } = null!;
}
