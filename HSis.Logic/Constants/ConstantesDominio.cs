namespace HSis.Logic.Constants
{
    public static class ConstantesEstatus
    {
        public const string ABIERTO = "Abierto";
        public const string EN_PROCESO = "En proceso";
        public const string CERRADO = "Cerrado";
        public const string REABIERTO = "Reabierto";
    }

    public static class ConstantesPrioridad
    {
        public const string ALTA = "Alta";
        public const string MEDIA = "Media";
        public const string BAJA = "Baja";
        public const string URGENTE = "Urgente";
    }

    /// <summary>
    /// Enumeración fuertemente tipada para los Roles de Usuario en el sistema HSis.
    /// </summary>
    public enum RolUsuarioEnum
    {
        Administrador = 1,
        Tecnico = 2,
        Cliente = 3
    }

    public enum VistaTemporal
    {
        Dia,
        Semana,
        Mes,
        Ano,
        Todos
    }
}
