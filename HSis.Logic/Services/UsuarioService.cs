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
        private readonly IDbContextFactory<HSisDbContext> _dbContextFactory;

        public UsuarioService(IDbContextFactory<HSisDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

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

            using var db = _dbContextFactory.CreateDbContext();
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
            using var db = _dbContextFactory.CreateDbContext();
            var usuario = await db.Usuarios
                .Include(u => u.IdDepartamentoNavigation)
                .Include(u => u.IdPuestoNavigation)
                .Include(u => u.IdSucursalNavigation)
                .Include(u => u.IdRolNavigation)
                .FirstOrDefaultAsync(u => u.Nombre == nombreUsuario);
            return usuario !=null && VerifyPassword(contraseña, usuario.Contraseña) ? usuario : null;
        }



        public async Task<List<Usuario>> ObtenerUsuariosPorRolAsync(int idRol)
        {
            using var db = _dbContextFactory.CreateDbContext();
            return await db.Usuarios
                .Where(u => u.IdRol == idRol)
                .Include(u => u.IdDepartamentoNavigation)
                .Include(u => u.IdPuestoNavigation)
                .Include(u => u.IdSucursalNavigation)
                .Include(u => u.IdRolNavigation)
                .ToListAsync();
        }


    }
}
