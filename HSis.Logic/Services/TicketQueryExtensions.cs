using System;
using System.Linq;
using HSis.Data.Models;
using HSis.Logic.DTOs;

namespace HSis.Logic.Services
{
    public static class TicketQueryExtensions
    {
        public static IQueryable<Ticket> ApplyFilters(this IQueryable<Ticket> query, TicketFilterDto filtros)
        {
            if (filtros == null) return query;

            // 1. Filtros de Texto / Identificadores
            if (!string.IsNullOrWhiteSpace(filtros.UsuarioEmisor))
            {
                query = query.Where(t => t.IdUsuarioNavigation.Nombre != null && t.IdUsuarioNavigation.Nombre.Contains(filtros.UsuarioEmisor));
            }
            if (!string.IsNullOrWhiteSpace(filtros.Estatus))
            {
                query = query.Where(t => t.Status == filtros.Estatus);
            }
            if (filtros.IdTecnico.HasValue)
            {
                query = query.Where(t => t.IdTecnico == filtros.IdTecnico.Value);
            }
            if (!string.IsNullOrWhiteSpace(filtros.Prioridad))
            {
                query = query.Where(t => t.Prioridad == filtros.Prioridad);
            }

            // 2. Rangos de Fechas Explícitos
            if (filtros.FechaAltaInicio.HasValue)
                query = query.Where(t => t.Alta >= filtros.FechaAltaInicio.Value);
            if (filtros.FechaAltaFin.HasValue)
                query = query.Where(t => t.Alta <= filtros.FechaAltaFin.Value);

            // 3. Vistas Temporales Rápidas (Día, Semana, Mes, Año)
            if (filtros.RangoTemporal.HasValue)
            {
                var hoy = DateTime.Today;
                DateTime inicio = hoy;
                DateTime fin = hoy.AddDays(1).AddTicks(-1);

                switch (filtros.RangoTemporal.Value)
                {
                    case VistaTemporal.Dia:
                        inicio = hoy;
                        break;
                    case VistaTemporal.Semana:
                        int diasAlLunes = (int)hoy.DayOfWeek - (int)DayOfWeek.Monday;
                        if (diasAlLunes < 0) diasAlLunes += 7;
                        inicio = hoy.AddDays(-diasAlLunes);
                        break;
                    case VistaTemporal.Mes:
                        inicio = new DateTime(hoy.Year, hoy.Month, 1);
                        break;
                    case VistaTemporal.Ano:
                        inicio = new DateTime(hoy.Year, 1, 1);
                        break;
                }

                if (filtros.RangoTemporal.Value != VistaTemporal.Todos)
                {
                    query = query.Where(t => t.Alta >= inicio && t.Alta <= fin);
                }
            }

            return query;
        }

        public static IQueryable<Ticket> ApplySlaFilter(this IQueryable<Ticket> query, bool esUrgente, DateTime fechaLimite)
        {
            return esUrgente
                ? query.Where(t => t.Status == ConstantesEstatus.ABIERTO && t.Alta < fechaLimite)
                : query.Where(t => t.Status == ConstantesEstatus.ABIERTO && t.Alta >= fechaLimite);
        }
    }
}
