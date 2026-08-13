using HSis.Data.Models;

namespace HSis.UI.Presenters
{
    public interface INuevoTicketView
    {
        string Descripcion { get; set; }
        string NombreSolicitanteTercero { get; set; }
        bool EsEnRepresentacion { get; set; }

        void CargarClientes(List<Usuario> clientes, int idUsuarioSesion);
        void CargarTecnicos(List<Usuario> tecnicos, bool esTecnicoSesion, int idUsuarioSesion);
        void CargarPrioridades();
        void MostrarError(string titulo, string mensaje);
        void MostrarExito(string mensaje);
        void CerrarExitoso();
        void MostrarCargando(bool cargando);
    }
}
