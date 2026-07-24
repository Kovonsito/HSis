#nullable enable
using System.Runtime.Versioning;
using System.Windows.Forms;
using HSis.Logic.DTOs;
using HSis.Logic.Services;

namespace HSis.UI.Helpers
{
    [SupportedOSPlatform("windows")]
    public static class SesionSistema
    {
        public static UsuarioDto? UsuarioActual { get; set; }
        public static string TokenJWT { get; set; } = string.Empty;
        public static int IdUsuario => UsuarioActual?.IdUsuario ?? 0;
        public static string NombreUsuario => UsuarioActual?.Nombre ?? string.Empty;
        public static int IdRolUsuario => UsuarioActual?.IdRol ?? 0;

        public static void ConfigurarMenuSesion(Form form, ISessionCacheService sessionCache)
        {
            var menu = new MenuStrip();

            var menuUsuario = new ToolStripMenuItem($"Sesión de: {NombreUsuario}")
            {
                Alignment = ToolStripItemAlignment.Right
            };

            var itemPerfil = new ToolStripMenuItem("Mi Perfil");
            itemPerfil.Click += (s, e) =>
            {
                string rol = IdRolUsuario == 1 ? "Administrador" : (IdRolUsuario == 2 ? "Técnico" : "Cliente");

                string depto = UsuarioActual?.DepartamentoNombre ?? "Sin Asignar";
                string puesto = UsuarioActual?.PuestoNombre ?? "Sin Asignar";
                string sucursal = UsuarioActual?.SucursalNombre ?? "Sin Asignar";

                string info = $"Nombre de Usuario: {NombreUsuario}\n" +
                              $"Rol asignado: {rol}\n\n" +
                              $"Departamento: {depto}\n" +
                              $"Puesto: {puesto}\n" +
                              $"Sucursal: {sucursal}";

                MessageBox.Show(info, "Información de Sesión", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            var itemCerrarSesion = new ToolStripMenuItem("Cerrar Sesión");
            itemCerrarSesion.Click += (s, e) =>
            {
                var confirmResult = MessageBox.Show("¿Estás seguro de que deseas cerrar sesión?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirmResult == DialogResult.Yes)
                {
                    // Limpiar credenciales guardadas en caché
                    sessionCache.ClearCredentials();

                    Application.Restart();
                }
            };

            menuUsuario.DropDownItems.Add(itemPerfil);
            menuUsuario.DropDownItems.Add(itemCerrarSesion);

            menu.Items.Add(menuUsuario);
            form.MainMenuStrip = menu;
            form.Controls.Add(menu);
        }
    }
}
