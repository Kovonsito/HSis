using HSis.Data.Models;
using HSis.Logic.Constants;
using HSis.Logic.DTOs;
using HSis.Logic.Services;
using HSis.UI.Helpers;

namespace HSis.UI.Presenters
{
    public class NuevoTicketPresenter(
        ITicketService ticketService,
        ICatalogoService catalogoService)
    {
        private INuevoTicketView? _view;

        public void SetView(INuevoTicketView view)
        {
            _view = view;
        }

        public async Task CargarCatalogosAsync()
        {
            if (_view == null) return;
            try
            {
                _view.MostrarCargando(true);
                _view.CargarPrioridades();

                var todosUsuarios = await catalogoService.ObtenerTodosAsync<Usuario>();
                var clientes = todosUsuarios.Where(u => u.IdRol == (int)RolUsuarioEnum.Cliente).OrderBy(u => u.Nombre).ToList();
                var tecnicos = todosUsuarios.Where(u => u.IdRol == (int)RolUsuarioEnum.Tecnico || u.IdRol == (int)RolUsuarioEnum.Administrador).OrderBy(u => u.Nombre).ToList();

                _view.CargarClientes(clientes, SesionSistema.IdUsuario);
                _view.CargarTecnicos(tecnicos, SesionSistema.EsTecnico, SesionSistema.IdUsuario);
            }
            catch (Exception ex)
            {
                _view.MostrarError("Error de Carga", $"Ocurrió un error al cargar catálogos: {ex.Message}");
            }
            finally
            {
                _view.MostrarCargando(false);
            }
        }

        public async Task GuardarTicketAsync(int idSolicitanteSeleccionado, int? idTecnicoSeleccionado, string? prioridadSeleccionada)
        {
            if (_view == null) return;

            if (_view.EsEnRepresentacion && string.IsNullOrWhiteSpace(_view.NombreSolicitanteTercero))
            {
                _view.MostrarError("Validación", "Por favor, ingrese el nombre de la persona que solicitó la atención.");
                return;
            }

            if (string.IsNullOrWhiteSpace(_view.Descripcion))
            {
                _view.MostrarError("Validación", "Por favor, ingrese una descripción del problema.");
                return;
            }

            try
            {
                _view.MostrarCargando(true);

                int idUsuarioFinal = SesionSistema.IdUsuario;
                int? idTecnicoFinal = null;
                string? prioridadFinal = null;

                if (SesionSistema.EsAdmin || SesionSistema.EsTecnico)
                {
                    if (!_view.EsEnRepresentacion)
                    {
                        idUsuarioFinal = idSolicitanteSeleccionado;
                    }
                    prioridadFinal = prioridadSeleccionada;
                    idTecnicoFinal = idTecnicoSeleccionado;
                }

                string descripcionFinal = _view.Descripcion.Trim();
                if (_view.EsEnRepresentacion && !string.IsNullOrWhiteSpace(_view.NombreSolicitanteTercero))
                {
                    descripcionFinal = $"[Solicitante no registrado: {_view.NombreSolicitanteTercero.Trim()}]\r\n\r\n{descripcionFinal}";
                }

                var nuevoTicketDto = new TicketCreateDto
                {
                    IdUsuario = idUsuarioFinal,
                    Descripcion = descripcionFinal,
                    IdTecnico = idTecnicoFinal,
                    Prioridad = prioridadFinal
                };

                var ticketGuardado = await ticketService.CrearTicketAsync(nuevoTicketDto);
                _view.MostrarExito($"Ticket registrado exitosamente con Folio: TK-{ticketGuardado.IdTicket:d6}");
                _view.CerrarExitoso();
            }
            catch (FluentValidation.ValidationException ex)
            {
                string errores = string.Join("\n", ex.Errors.Select(e => "- " + e.ErrorMessage));
                _view.MostrarError("Validación", $"Datos inválidos:\n{errores}");
            }
            catch (Exception ex)
            {
                _view.MostrarError("Error", $"Error al registrar el ticket: {ex.Message}");
            }
            finally
            {
                _view.MostrarCargando(false);
            }
        }
    }
}
