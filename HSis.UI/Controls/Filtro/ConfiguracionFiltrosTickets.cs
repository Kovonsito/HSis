#nullable enable
using HSis.Contracts.Constants;
using HSis.Contracts.DTOs;

namespace HSis.UI.Helpers
{
    public static class ConfiguracionFiltrosTickets
    {
        public static List<FiltroCampo> ObtenerCamposAdmin()
        {
            return [
                new() { NombrePropiedad = "Estatus", Etiqueta = "Estatus:", Tipo = TipoFiltroControl.ComboSeleccion, ValoresCombo = ["Todos", "Nuevos", "Urgentes", "Abierto", "En Proceso", "Cerrado", "Reabierto"], Ancho = 115 },
                new() { NombrePropiedad = "Prioridad", Etiqueta = "Prioridad:", Tipo = TipoFiltroControl.ComboSeleccion, ValoresCombo = ["Todos", ConstantesPrioridad.ALTA, ConstantesPrioridad.MEDIA, ConstantesPrioridad.BAJA, ConstantesPrioridad.URGENTE], Ancho = 110 },
                new() { NombrePropiedad = "Tecnico", Etiqueta = "Técnico:", Tipo = TipoFiltroControl.ComboSeleccion, Ancho = 130 },
                new() { NombrePropiedad = "Usuario", Etiqueta = "Solicitante:", Tipo = TipoFiltroControl.Texto, Ancho = 120 },
                new() { NombrePropiedad = "FechaInicio", Etiqueta = "Desde:", Tipo = TipoFiltroControl.Fecha, Ancho = 110, ValorDefecto = DateTime.Today.AddDays(-30) },
                new() { NombrePropiedad = "FechaFin", Etiqueta = "Hasta:", Tipo = TipoFiltroControl.Fecha, Ancho = 110, ValorDefecto = DateTime.Today.AddDays(1).AddTicks(-1) }
            ];
        }

        public static List<FiltroCampo> ObtenerCamposTecnico()
        {
            return [
                new() { NombrePropiedad = "Texto", Etiqueta = "Buscar (Folio/Asunto):", Tipo = TipoFiltroControl.Texto, Ancho = 160 },
                new() { NombrePropiedad = "Prioridad", Etiqueta = "Prioridad:", Tipo = TipoFiltroControl.ComboSeleccion, ValoresCombo = ["Todos", ConstantesPrioridad.ALTA, ConstantesPrioridad.MEDIA, ConstantesPrioridad.BAJA, ConstantesPrioridad.URGENTE], Ancho = 110 },
                new() { NombrePropiedad = "Usuario", Etiqueta = "Solicitante:", Tipo = TipoFiltroControl.Texto, Ancho = 130 },
                new() { NombrePropiedad = "FechaInicio", Etiqueta = "Desde:", Tipo = TipoFiltroControl.Fecha, Ancho = 110, ValorDefecto = DateTime.Today.AddDays(-30) },
                new() { NombrePropiedad = "FechaFin", Etiqueta = "Hasta:", Tipo = TipoFiltroControl.Fecha, Ancho = 110, ValorDefecto = DateTime.Today.AddDays(1).AddTicks(-1) }
            ];
        }

        public static List<FiltroCampo> ObtenerCamposCliente()
        {
            return [
                new() { NombrePropiedad = "Texto", Etiqueta = "Buscar (Folio/Problema):", Tipo = TipoFiltroControl.Texto, Ancho = 230 },
                new() { NombrePropiedad = "FechaInicio", Etiqueta = "Desde:", Tipo = TipoFiltroControl.Fecha, Ancho = 135, ValorDefecto = DateTime.Today.AddDays(-60) },
                new() { NombrePropiedad = "FechaFin", Etiqueta = "Hasta:", Tipo = TipoFiltroControl.Fecha, Ancho = 135, ValorDefecto = DateTime.Today.AddDays(1).AddTicks(-1) }
            ];
        }

        public static Dictionary<string, object?> ObtenerValoresDefecto()
        {
            return new Dictionary<string, object?>
            {
                { "Texto", string.Empty },
                { "Estatus", "Todos" },
                { "Prioridad", "Todos" },
                { "Usuario", string.Empty },
                { "FechaInicio", DateTime.Today.AddDays(-30) },
                { "FechaFin", DateTime.Today.AddDays(1).AddTicks(-1) }
            };
        }


        public static TicketFilterDto MapearFiltrosAdmin(Dictionary<string, object?> vals)
        {
            var filtros = new TicketFilterDto();

            if (vals.TryGetValue("Estatus", out var estVal) && estVal != null)
            {
                var est = estVal.ToString();
                if (est != null && est != "Todos") filtros.Estatus = est;
            }

            if (vals.TryGetValue("Prioridad", out var priVal) && priVal != null)
            {
                var pri = priVal.ToString();
                if (pri != null && pri != "Todos") filtros.Prioridad = pri;
            }

            if (vals.TryGetValue("Tecnico", out var tecVal) && tecVal != null)
            {
                if (int.TryParse(tecVal.ToString(), out int idTecnico) && idTecnico > 0)
                {
                    filtros.IdTecnico = idTecnico;
                }
            }

            if (vals.TryGetValue("Usuario", out var usrVal) && usrVal != null)
            {
                string emisor = usrVal.ToString()!;
                if (!string.IsNullOrWhiteSpace(emisor)) filtros.UsuarioEmisor = emisor;
            }

            if (vals.TryGetValue("Temporal", out var tempVal) && tempVal != null)
            {
                string tempSel = tempVal.ToString()!;
                filtros.RangoTemporal = tempSel switch
                {
                    "Día" => VistaTemporal.Dia,
                    "Semana" => VistaTemporal.Semana,
                    "Mes" => VistaTemporal.Mes,
                    "Año" => VistaTemporal.Ano,
                    _ => VistaTemporal.Todos
                };
            }
            else
            {
                filtros.RangoTemporal = VistaTemporal.Todos;
            }

            var (_, fechaInicio, fechaFin, _, _) = vals.ExtraerFiltrosComunes(DateTime.Today.AddDays(-30), DateTime.Today.AddDays(1).AddTicks(-1));
            filtros.FechaAltaInicio = fechaInicio ?? DateTime.Today.AddDays(-30);
            filtros.FechaAltaFin = fechaFin ?? DateTime.Today.AddDays(1).AddTicks(-1);

            return filtros;
        }

        public static (string? Texto, DateTime? FechaInicio, DateTime? FechaFin, string? Prioridad, string? Usuario) ExtraerFiltrosComunes(
            this Dictionary<string, object?> vals,
            DateTime? fechaInicioDefecto = null,
            DateTime? fechaFinDefecto = null)
        {
            string? texto = null;
            if (vals.TryGetValue("Texto", out var txtVal) && txtVal != null)
            {
                var txt = txtVal.ToString()?.Trim();
                if (!string.IsNullOrWhiteSpace(txt)) texto = txt.ToLowerInvariant();
            }

            string? prioridad = null;
            if (vals.TryGetValue("Prioridad", out var priVal) && priVal != null)
            {
                var priStr = priVal.ToString()?.Trim();
                if (!string.IsNullOrEmpty(priStr) && priStr != "Todos") prioridad = priStr;
            }

            string? usuario = null;
            if (vals.TryGetValue("Usuario", out var usrVal) && usrVal != null)
            {
                var usrStr = usrVal.ToString()?.Trim();
                if (!string.IsNullOrWhiteSpace(usrStr)) usuario = usrStr.ToLowerInvariant();
            }

            DateTime? fi = fechaInicioDefecto?.Date;
            if (vals.TryGetValue("FechaInicio", out var fiVal) && fiVal is DateTime dtInicio)
            {
                fi = dtInicio.Date;
            }

            DateTime? ff = fechaFinDefecto.HasValue ? fechaFinDefecto.Value.Date.AddDays(1).AddTicks(-1) : null;
            if (vals.TryGetValue("FechaFin", out var ffVal) && ffVal is DateTime dtFin)
            {
                ff = dtFin.Date.AddDays(1).AddTicks(-1);
            }

            return (texto, fi, ff, prioridad, usuario);
        }
    }
}

