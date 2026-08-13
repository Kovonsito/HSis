#nullable enable
using System;
using System.Collections.Generic;
using HSis.Logic.Constants;
using HSis.Logic.DTOs;
using HSis.UI.Controls;

namespace HSis.UI.Helpers
{
    public static class ConfiguracionFiltrosTickets
    {
        public static List<FiltroCampo> ObtenerCamposAdmin()
        {
            return [
                new() { NombrePropiedad = "Estatus", Etiqueta = "Estatus:", Tipo = TipoFiltroControl.ComboSeleccion, ValoresCombo = ["Todos", "Nuevos", "Urgentes", "Abierto", "En Proceso", "Cerrado", "Reabierto"], Ancho = 130 },
                new() { NombrePropiedad = "Prioridad", Etiqueta = "Prioridad:", Tipo = TipoFiltroControl.ComboSeleccion, ValoresCombo = ["Todos", ConstantesPrioridad.ALTA, ConstantesPrioridad.MEDIA, ConstantesPrioridad.BAJA, ConstantesPrioridad.URGENTE], Ancho = 130 },
                new() { NombrePropiedad = "Tecnico", Etiqueta = "Técnico:", Tipo = TipoFiltroControl.ComboSeleccion, Ancho = 160 },
                new() { NombrePropiedad = "Usuario", Etiqueta = "Usuario Emisor:", Tipo = TipoFiltroControl.Texto, Ancho = 160 },
                new() { NombrePropiedad = "Temporal", Etiqueta = "Vista Temporal:", Tipo = TipoFiltroControl.ComboSeleccion, ValoresCombo = ["Todos", "Día", "Semana", "Mes", "Año"], Ancho = 130 },
                new() { NombrePropiedad = "FechaInicio", Etiqueta = "Desde:", Tipo = TipoFiltroControl.Fecha, Ancho = 130, ValorDefecto = DateTime.Today.AddDays(-30) },
                new() { NombrePropiedad = "FechaFin", Etiqueta = "Hasta:", Tipo = TipoFiltroControl.Fecha, Ancho = 130, ValorDefecto = DateTime.Today.AddDays(1).AddTicks(-1) }
            ];
        }

        public static List<FiltroCampo> ObtenerCamposTecnico()
        {
            return [
                new() { NombrePropiedad = "Estatus", Etiqueta = "Estatus:", Tipo = TipoFiltroControl.ComboSeleccion, ValoresCombo = ["Todos", "Abierto", "En Proceso", "Cerrado", "Reabierto"], Ancho = 130 },
                new() { NombrePropiedad = "Prioridad", Etiqueta = "Prioridad:", Tipo = TipoFiltroControl.ComboSeleccion, ValoresCombo = ["Todos", ConstantesPrioridad.ALTA, ConstantesPrioridad.MEDIA, ConstantesPrioridad.BAJA, ConstantesPrioridad.URGENTE], Ancho = 130 },
                new() { NombrePropiedad = "Usuario", Etiqueta = "Usuario Emisor:", Tipo = TipoFiltroControl.Texto, Ancho = 160 },
                new() { NombrePropiedad = "FechaInicio", Etiqueta = "Desde:", Tipo = TipoFiltroControl.Fecha, Ancho = 130, ValorDefecto = DateTime.Today.AddDays(-30) },
                new() { NombrePropiedad = "FechaFin", Etiqueta = "Hasta:", Tipo = TipoFiltroControl.Fecha, Ancho = 130, ValorDefecto = DateTime.Today.AddDays(1).AddTicks(-1) }
            ];
        }

        public static Dictionary<string, object?> ObtenerValoresDefecto()
        {
            return new Dictionary<string, object?>
            {
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

            DateTime fi = DateTime.Today.AddDays(-30);
            if (vals.TryGetValue("FechaInicio", out var fiVal) && fiVal is DateTime dtInicio)
            {
                fi = dtInicio;
            }
            filtros.FechaAltaInicio = fi.Date;

            DateTime ff = DateTime.Today.AddDays(1).AddTicks(-1);
            if (vals.TryGetValue("FechaFin", out var ffVal) && ffVal is DateTime dtFin)
            {
                ff = dtFin;
            }
            filtros.FechaAltaFin = ff.Date.AddDays(1).AddTicks(-1);

            return filtros;
        }
    }
}

