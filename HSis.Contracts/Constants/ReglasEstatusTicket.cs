namespace HSis.Logic.Constants
{
    public static class ReglasEstatusTicket
    {
        public static List<string> ObtenerEstatusPermitidos(int idRolUsuario, string estatusActual)
        {
            var permitidos = new List<string>();

            if (idRolUsuario == (int)RolUsuarioEnum.Administrador)
            {
                permitidos.AddRange([ConstantesEstatus.ABIERTO, ConstantesEstatus.EN_PROCESO, ConstantesEstatus.CERRADO, ConstantesEstatus.REABIERTO]);
            }
            else if (idRolUsuario == (int)RolUsuarioEnum.Tecnico)
            {
                permitidos.Add(estatusActual);

                if (estatusActual == ConstantesEstatus.ABIERTO)
                {
                    permitidos.Add(ConstantesEstatus.EN_PROCESO);
                }
                else if (estatusActual == ConstantesEstatus.EN_PROCESO)
                {
                    permitidos.Add(ConstantesEstatus.CERRADO);
                }
                else if (estatusActual == ConstantesEstatus.REABIERTO)
                {
                    permitidos.Add(ConstantesEstatus.EN_PROCESO);
                }
            }
            else if (idRolUsuario == (int)RolUsuarioEnum.Cliente)
            {
                permitidos.Add(estatusActual);
                if (estatusActual == ConstantesEstatus.CERRADO)
                {
                    permitidos.Add(ConstantesEstatus.REABIERTO);
                }
            }

            return permitidos.Distinct().ToList();
        }
    }
}
