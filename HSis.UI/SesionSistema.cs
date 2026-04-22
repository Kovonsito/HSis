using HSis.Data.Models;
namespace HSis.UI
{
    public static class SesionSistema
    {
        public static Usuario UsuarioActual { get; set; }
        public static int IdUsuario => UsuarioActual?.IdUsuario ?? 0;
        public static string NombreUsuario => UsuarioActual?.Nombre ?? string.Empty;
        public static int IdRolUsuario => UsuarioActual?.IdRol ?? 0;

        public static void ConfigurarMenuSesion(Form form)
        {
            var menu = new MenuStrip();

            var menuUsuario = new ToolStripMenuItem($"Sesión de: {NombreUsuario}");
            menuUsuario.Alignment = ToolStripItemAlignment.Right;

            var itemPerfil = new ToolStripMenuItem("Mi Perfil");
            itemPerfil.Click += (s, e) =>
            {
                string rol = IdRolUsuario == 1 ? "Administrador" : (IdRolUsuario == 2 ? "Técnico" : "Cliente");

                string depto = UsuarioActual?.IdDepartamentoNavigation?.Nombre ?? "Sin Asignar";
                string puesto = UsuarioActual?.IdPuestoNavigation?.Nombre ?? "Sin Asignar";
                string sucursal = UsuarioActual?.IdSucursalNavigation?.Nombre ?? "Sin Asignar";

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
