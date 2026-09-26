namespace HSis.Contracts.Constants;

public static class ConstantesTiposNotificacion
{
    public const string NuevoTicket = "NuevoTicket";
    public const string EstadoTicket = "EstadoTicket";
    public const string AsignacionTicket = "AsignacionTicket";
    public const string ReasignacionTicket = "ReasignacionTicket";
    public const string PrioridadTicket = "PrioridadTicket";
    public const string SolucionTicket = "SolucionTicket";
    public const string Calificacion = "Calificacion";
    public const string MovimientoMaterial = "MovimientoMaterial";
    public const string MaterialTicket = "MaterialTicket";
}

public static class GruposNotificaciones
{
    public const string Tecnicos = "Role_Técnico";
    public const string Administradores = "Role_Administrador";
    public const string Usuarios = "Role_Usuario";

    public static string Usuario(int userId) => $"User_{userId}";
}
