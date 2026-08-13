using HSis.Data.Models;
using HSis.Logic.Constants;
using HSis.Logic.DTOs;
using HSis.Logic.Services;
using HSis.UI.Helpers;

namespace HSis.UI.Presenters
{
    public class TicketDetallePresenter(
        ITicketService ticketService,
        ITicketDetalleService ticketDetalleService,
        ICatalogoService catalogoService)
    {
        private ITicketDetalleView? _view;

        public void SetView(ITicketDetalleView view)
        {
            _view = view;
        }


        public async Task CargarTicketDetallesAsync(int idTicket)
        {
            if (_view == null) return;
            try
            {
                _view.MostrarCargando(true);
                var ticket = await ticketService.ObtenerTicketPorIdAsync(idTicket);
                if (ticket == null)
                {
                    _view.MostrarError("Ticket no encontrado.");
                    _view.CerrarFormulario();
                    return;
                }

                _view.MostrarTicket(ticket);

                string estatusActual = ticket.Estatus ?? ConstantesEstatus.ABIERTO;
                var estatusPermitidos = TicketService.ObtenerEstatusPermitidos(SesionSistema.IdRolUsuario, estatusActual);
                _view.CargarEstatusPermitidos(estatusPermitidos, estatusActual);

                var todosUsuarios = await catalogoService.ObtenerTodosAsync<Usuario>();
                var tecnicos = todosUsuarios.Where(u => u.IdRol == (int)RolUsuarioEnum.Tecnico || u.IdRol == (int)RolUsuarioEnum.Administrador).OrderBy(u => u.Nombre).ToList();
                _view.CargarTecnicos(tecnicos, ticket.IdTecnico, SesionSistema.EsAdmin);

                await RecargarHistorialYMaterialesAsync(idTicket);
            }
            catch (Exception ex)
            {
                _view.MostrarError($"Error al cargar ticket: {ex.Message}");
            }
            finally
            {
                _view.MostrarCargando(false);
            }
        }

        public async Task RecargarHistorialYMaterialesAsync(int idTicket)
        {
            if (_view == null) return;
            var historial = await ticketService.ObtenerHistorialPorTicketAsync(idTicket);
            _view.CargarHistorial(historial);

            var detalles = await ticketDetalleService.ObtenerDetallesTicketAsync(idTicket);
            _view.CargarDetallesMaterial(detalles);
        }

        public async Task ActualizarTicketAsync(TicketDto ticketActual, string estatusSeleccionado, int? idTecnico, string solucionIngresada, string prioridadSeleccionada)
        {
            if (_view == null) return;

            bool huboCambios = ticketActual.Estatus != estatusSeleccionado ||
                               ticketActual.IdTecnico != idTecnico ||
                               (ticketActual.Solucion ?? string.Empty) != solucionIngresada ||
                               (ticketActual.Prioridad ?? string.Empty) != prioridadSeleccionada;

            if (!huboCambios)
            {
                _view.CerrarFormulario();
                return;
            }

            try
            {
                _view.MostrarCargando(true);
                var updateDto = new TicketUpdateDto
                {
                    IdTicket = ticketActual.IdTicket,
                    Estatus = estatusSeleccionado,
                    IdTecnico = idTecnico,
                    Solucion = solucionIngresada,
                    FechaAtencion = ticketActual.FechaAtencion,
                    FechaCierre = ticketActual.FechaCierre,
                    Prioridad = prioridadSeleccionada
                };

                if (updateDto.Estatus == ConstantesEstatus.REABIERTO)
                {
                    updateDto.FechaCierre = null;
                }
                else if (updateDto.Estatus == ConstantesEstatus.EN_PROCESO && updateDto.FechaAtencion == null)
                {
                    updateDto.FechaAtencion = DateTime.Now;
                }
                else if (updateDto.Estatus == ConstantesEstatus.CERRADO && updateDto.FechaCierre == null)
                {
                    updateDto.FechaCierre = DateTime.Now;
                }
                else if (updateDto.Estatus == ConstantesEstatus.ABIERTO)
                {
                    updateDto.FechaAtencion = null;
                    updateDto.FechaCierre = null;
                }

                await ticketService.ActualizarTicketAsync(updateDto);
                _view.MostrarExito("Ticket actualizado correctamente.");
                _view.CerrarFormulario();
            }
            catch (FluentValidation.ValidationException ex)
            {
                string errores = string.Join("\n", ex.Errors.Select(e => "- " + e.ErrorMessage));
                _view.MostrarError($"Datos inválidos:\n{errores}");
            }
            catch (Exception ex)
            {
                _view.MostrarError($"Error al actualizar ticket: {ex.Message}");
            }
            finally
            {
                _view.MostrarCargando(false);
            }
        }

        public async Task AgregarMaterialAsync(TicketDetalleDto detalleDto)
        {
            if (_view == null) return;
            try
            {
                _view.MostrarCargando(true);
                await ticketDetalleService.AgregarMaterialATicketAsync(detalleDto);
                _view.MostrarExito("Material asignado al ticket.");
                await RecargarHistorialYMaterialesAsync(detalleDto.IdTicket);
            }
            catch (Exception ex)
            {
                _view.MostrarError($"Error al agregar material: {ex.Message}");
            }
            finally
            {
                _view.MostrarCargando(false);
            }
        }

        public async Task EliminarMaterialAsync(int idTicket, int idMaterial)
        {
            if (_view == null) return;
            try
            {
                _view.MostrarCargando(true);
                await ticketDetalleService.EliminarMaterialDeTicketAsync(idTicket, idMaterial);
                _view.MostrarExito("Material retirado del ticket.");
                await RecargarHistorialYMaterialesAsync(idTicket);
            }
            catch (Exception ex)
            {
                _view.MostrarError($"Error al eliminar material: {ex.Message}");
            }
            finally
            {
                _view.MostrarCargando(false);
            }
        }
    }
}

