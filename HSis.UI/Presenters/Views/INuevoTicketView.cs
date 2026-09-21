using HSis.Logic.DTOs;

namespace HSis.UI.Presenters
{
    public interface INuevoTicketView
    {
        string Descripcion { get; set; }
        string NombreSolicitanteTercero { get; set; }
        bool EsEnRepresentacion { get; set; }

        void CargarClientes(List<UsuarioDto> clientes, int idUsuarioSesion);
        void CargarTecnicos(List<UsuarioDto> tecnicos, bool esTecnicoSesion, int idUsuarioSesion);
        void CargarPrioridades();
        void MostrarError(string titulo, string mensaje);
        void MostrarExito(string mensaje);
        void CerrarExitoso();
        void MostrarCargando(bool cargando);
    }
}
