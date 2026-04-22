using HSis.Data.Models;
using HSis.Logic.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace HSis.Logic.Interceptors
{
    public class TicketAuditInterceptor : SaveChangesInterceptor
    {
        private readonly ICurrentUserService _currentUserService;

        public TicketAuditInterceptor(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            RegistrarCambiosAuditoria(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            RegistrarCambiosAuditoria(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void RegistrarCambiosAuditoria(DbContext? context)
        {
            if (context == null) return;

            // Filtramos las entidades Ticket que están siendo modificadas
            var entradasModificadas = context.ChangeTracker.Entries<Ticket>()
                .Where(e => e.State == EntityState.Modified)
                .ToList();

            if (!entradasModificadas.Any()) return;

            int currentUserId = _currentUserService.GetCurrentUserId();
            var auditorias = new List<HistorialCambiosTicket>();

            foreach (var entrada in entradasModificadas)
            {
                var propiedadesA_Auditar = new[] { "Status", "IdTecnico", "Solución" };

                foreach (var propName in propiedadesA_Auditar)
                {
                    var property = entrada.Property(propName);

                    if (property.IsModified)
                    {
                        var originalValue = property.OriginalValue?.ToString() ?? "";
                        var currentValue = property.CurrentValue?.ToString() ?? "";

                        if (originalValue != currentValue)
                        {
                            auditorias.Add(new HistorialCambiosTicket
                            {
                                IdTicket = entrada.Entity.IdTicket,
                                CampoModificado = propName,
                                ValorAnterior = originalValue == "" ? "null" : originalValue,
                                ValorNuevo = currentValue == "" ? "null" : currentValue,
                                IdUsuarioCambio = currentUserId,
                                FechaMovimiento = DateTime.Now
                            });
                        }
                    }
                }
            }

            if (auditorias.Any())
            {
                context.Set<HistorialCambiosTicket>().AddRange(auditorias);
            }
        }
    }
}
