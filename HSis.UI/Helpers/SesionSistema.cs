#nullable enable
using System.Runtime.Versioning;
using HSis.Logic.Constants;
using HSis.Logic.DTOs;
using HSis.Logic.Services;

namespace HSis.UI.Helpers
{
    [SupportedOSPlatform("windows")]
    public static class SesionSistema
    {
        private static IContextoSesion? _contexto;

        public static void Inicializar(IContextoSesion contexto)
        {
            _contexto = contexto;
        }

        public static UsuarioDto? UsuarioActual
        {
            get => _contexto?.UsuarioActual;
            set
            {
                if (_contexto != null) _contexto.UsuarioActual = value;
            }
        }

        public static string TokenJWT
        {
            get => _contexto?.TokenJWT ?? string.Empty;
            set
            {
                if (_contexto != null) _contexto.TokenJWT = value;
            }
        }

        public static int IdUsuario => _contexto?.IdUsuario ?? 0;
        public static string NombreUsuario => _contexto?.NombreUsuario ?? string.Empty;
        public static int IdRolUsuario => _contexto?.IdRolUsuario ?? 0;
        public static bool EsAdmin => _contexto?.EsAdmin ?? false;
        public static bool EsTecnico => _contexto?.EsTecnico ?? false;

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
                string rol = IdRolUsuario == (int)RolUsuarioEnum.Administrador ? "Administrador" : (IdRolUsuario == (int)RolUsuarioEnum.Tecnico ? "Técnico" : "Cliente");

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

            var versionAssembly = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            string versionTexto = versionAssembly != null
                ? $"{versionAssembly.Major}.{versionAssembly.Minor}.{versionAssembly.Build}"
                : Application.ProductVersion;

            var itemVersion = new ToolStripMenuItem($"Versión: {versionTexto}");
            itemVersion.Click += (s, e) =>
            {
                MessageBox.Show($"HSis - Sistema de Soporte e Inventario\n\nVersión instalada: v{versionTexto}", "Acerca de HSis", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            var itemCerrarSesion = new ToolStripMenuItem("Cerrar Sesión");
            itemCerrarSesion.Click += (s, e) =>
            {
                var confirmResult = MessageBox.Show("¿Estás seguro de que deseas cerrar sesión?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirmResult == DialogResult.Yes)
                {
                    sessionCache.ClearCredentials();
                    _contexto?.CerrarSesion();
                    Application.Restart();
                }
            };

            menuUsuario.DropDownItems.Add(itemPerfil);
            menuUsuario.DropDownItems.Add(itemVersion);
            menuUsuario.DropDownItems.Add(itemCerrarSesion);

            menu.Items.Add(menuUsuario);
            form.MainMenuStrip = menu;
            form.Controls.Add(menu);
        }
    }
}
