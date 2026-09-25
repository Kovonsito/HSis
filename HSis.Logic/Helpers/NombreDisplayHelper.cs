namespace HSis.Logic.Helpers
{
    /// <summary>
    /// Normaliza nombres procedentes de la base de datos sin alterar su capitalización.
    /// </summary>
    public static class NombreDisplayHelper
    {
        public static string Normalizar(string? nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                return string.Empty;
            }

            return string.Join(' ', nombre.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
        }
    }
}
