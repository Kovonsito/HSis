using HSis.Logic.DTOs;

namespace HSis.UI.Presenters
{
    public interface IIniciarSesionView
    {
        string NombreUsuario { get; set; }
        string Contraseña { get; set; }
        void LimpiarCredenciales();
        void CargarCredencialesGuardadas(string usuario, string contraseña);
        void NavegarADashboard(UsuarioDto usuario, string rolNombre);
        void MostrarError(string mensaje);
        void MostrarCargando(bool cargando);
    }
}

