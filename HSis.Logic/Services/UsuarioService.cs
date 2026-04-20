using Microsoft.EntityFrameworkCore;
using HSis.Data.Models;
using System.Security.Cryptography;
using System.Text;
using System.Security;

namespace HSis.Logic.Services
{
    /// <summary>
    /// Servicio para gestionar operaciones relacionadas con Usuarios.
    /// Incluye autenticación, creación y obtención de datos de usuario.
    /// </summary>
    public class UsuarioService
    {
        // Hash de contraseña con SHA256
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
        }

        private static bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }

        public async Task RehashearContraseñasAsync()
        {

            using var db = new HSisDbContext();
            var usuarios = await db.Usuarios.ToListAsync();

            foreach (var usuario in usuarios)
            {
                // Asume que están en texto plano actualmente
                usuario.Contraseña = HashPassword(usuario.Contraseña);
            }

            await db.SaveChangesAsync();
        }

        public async Task<Usuario?> AutenticarAsync(string nombreUsuario, string contraseña)
        {
            using var db = new HSisDbContext();
            var usuario = await db.Usuarios
                .Include(u => u.IdDepartamentoNavigation)
                .Include(u => u.IdPuestoNavigation)
                .Include(u => u.IdSucursalNavigation)
                .Include(u => u.IdRolNavigation)
                .FirstOrDefaultAsync(u => u.Nombre == nombreUsuario);
            return usuario !=null && VerifyPassword(contraseña, usuario.Contraseña) ? usuario : null;
        }

        // Obtener listados complementarios - Async
        public async Task<List<Departamento>> ObtenerDepartamentosAsync()
        {
            using var db = new HSisDbContext();
            return await db.Departamentos.OrderBy(d => d.Nombre).ToListAsync();
        }

        public async Task<List<Puesto>> ObtenerPuestosAsync()
        {
            using var db = new HSisDbContext();
            return await db.Puestos.OrderBy(p => p.Nombre).ToListAsync();
        }

        public async Task<List<Sucursal>> ObtenerSucursalesAsync()
        {
            using var db = new HSisDbContext();
            return await db.Sucursals.OrderBy(s => s.Nombre).ToListAsync();
        }

        // CRUD Usuario - Async
        public async Task CrearUsuarioAsync(Usuario usuario)
        {
            using var db = new HSisDbContext();
            // Hash de la contraseña antes de guardar
            usuario.Contraseña = HashPassword(usuario.Contraseña);
            db.Usuarios.Add(usuario);
            await db.SaveChangesAsync();
        }

        public async Task<Usuario?> ObtenerUsuarioPorIdAsync(int idUsuario)
        {
            using var db = new HSisDbContext();
            return await db.Usuarios
                .Include(u => u.IdDepartamentoNavigation)
                .Include(u => u.IdPuestoNavigation)
                .Include(u => u.IdSucursalNavigation)
                .Include(u => u.IdRolNavigation)
                .FirstOrDefaultAsync(u => u.IdUsuario == idUsuario);
        }

        public async Task<List<Usuario>> ObtenerUsuariosAsync()
        {
            using var db = new HSisDbContext();
            return await db.Usuarios
                .Include(u => u.IdDepartamentoNavigation)
                .Include(u => u.IdPuestoNavigation)
                .Include(u => u.IdSucursalNavigation)
                .Include(u => u.IdRolNavigation)
                .ToListAsync();
        }

        public async Task<List<Usuario>> ObtenerUsuariosPorRolAsync(int idRol)
        {
            using var db = new HSisDbContext();
            return await db.Usuarios
                .Where(u => u.IdRol == idRol)
                .Include(u => u.IdDepartamentoNavigation)
                .Include(u => u.IdPuestoNavigation)
                .Include(u => u.IdSucursalNavigation)
                .Include(u => u.IdRolNavigation)
                .ToListAsync();
        }

        public async Task ActualizarUsuarioAsync(Usuario usuario)
        {
            using var db = new HSisDbContext();
            db.Usuarios.Update(usuario);
            await db.SaveChangesAsync();
        }

        public async Task EliminarUsuarioAsync(int idUsuario)
        {
            using var db = new HSisDbContext();
            var usuario = await db.Usuarios.FindAsync(idUsuario);
            if (usuario != null)
            {
                db.Usuarios.Remove(usuario);
                await db.SaveChangesAsync();
            }
        }

        
    }
}
