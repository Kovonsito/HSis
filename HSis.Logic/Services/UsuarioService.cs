using HSis.Contracts.Services;
using HSis.Data.Models;
using HSis.Contracts.Constants;
using HSis.Contracts.DTOs;
using FluentValidation;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace HSis.Logic.Services
{
    /// <summary>
    /// Servicio para gestionar operaciones relacionadas con Usuarios.
    /// Incluye autenticación, creación y obtención de datos de usuario.
    /// </summary>
    public class UsuarioService(
        IDbContextFactory<HSisDbContext> dbContextFactory,
        IMapper mapper,
        IValidator<UsuarioDto>? validator = null) : IUsuarioService
    {

        public async Task<List<UsuarioDto>> ObtenerUsuariosAsync()
        {
            using var db = dbContextFactory.CreateDbContext();
            var usuarios = await db.Usuarios
                .AsNoTracking()
                .Include(usuario => usuario.Departamento)
                .Include(usuario => usuario.Puesto)
                .Include(usuario => usuario.Sucursal)
                .Include(usuario => usuario.Rol)
                .OrderBy(usuario => usuario.Nombre)
                .ToListAsync();

            return mapper.Map<List<UsuarioDto>>(usuarios);
        }

        public async Task<UsuarioDto> CrearUsuarioAsync(UsuarioCatalogoRequestDto request)
        {
            var usuarioDto = new UsuarioDto
            {
                Nombre = request.Nombre,
                IdDepartamento = request.IdDepartamento,
                IdPuesto = request.IdPuesto,
                IdSucursal = request.IdSucursal,
                IdRol = request.IdRol,
                Contraseña = request.Contraseña
            };
            await CatalogoValidation.ValidarAsync(validator, usuarioDto);

            using var db = dbContextFactory.CreateDbContext();
            var usuario = new Usuario
            {
                Nombre = request.Nombre,
                IdDepartamento = request.IdDepartamento,
                IdPuesto = request.IdPuesto,
                IdSucursal = request.IdSucursal,
                IdRol = request.IdRol,
                Contraseña = HashPassword(request.Contraseña)
            };
            db.Usuarios.Add(usuario);
            await db.SaveChangesAsync();
            await CargarRelacionesAsync(db, usuario);
            return mapper.Map<UsuarioDto>(usuario);
        }

        public async Task<UsuarioDto?> ActualizarUsuarioAsync(int idUsuario, UsuarioCatalogoRequestDto request)
        {
            var usuarioDto = new UsuarioDto
            {
                IdUsuario = idUsuario,
                Nombre = request.Nombre,
                IdDepartamento = request.IdDepartamento,
                IdPuesto = request.IdPuesto,
                IdSucursal = request.IdSucursal,
                IdRol = request.IdRol,
                Contraseña = request.Contraseña
            };
            await CatalogoValidation.ValidarAsync(validator, usuarioDto);

            using var db = dbContextFactory.CreateDbContext();
            var usuario = await db.Usuarios.FindAsync(idUsuario);
            if (usuario is null)
            {
                return null;
            }

            usuario.Nombre = request.Nombre;
            usuario.IdDepartamento = request.IdDepartamento;
            usuario.IdPuesto = request.IdPuesto;
            usuario.IdSucursal = request.IdSucursal;
            usuario.IdRol = request.IdRol;
            if (!string.IsNullOrWhiteSpace(request.Contraseña))
            {
                usuario.Contraseña = HashPassword(request.Contraseña);
            }

            await db.SaveChangesAsync();
            await CargarRelacionesAsync(db, usuario);
            return mapper.Map<UsuarioDto>(usuario);
        }

        public async Task<bool> EliminarUsuarioAsync(int idUsuario)
        {
            using var db = dbContextFactory.CreateDbContext();
            var usuario = await db.Usuarios.FindAsync(idUsuario);
            if (usuario is null)
            {
                return false;
            }

            db.Usuarios.Remove(usuario);
            await db.SaveChangesAsync();
            return true;
        }

        // Hash de contraseña con BCrypt
        public static string HashPassword(string? password)
        {
            if (string.IsNullOrEmpty(password)) return string.Empty;
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
        }

        private static bool VerifyPassword(string? password, string? hash)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hash)) return false;
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }

        public async Task RehashearContraseñasAsync()
        {
            using var db = dbContextFactory.CreateDbContext();
            var usuarios = await db.Usuarios.ToListAsync();

            foreach (var usuario in usuarios)
            {
                // Asume que están en texto plano actualmente
                usuario.Contraseña = HashPassword(usuario.Contraseña);
            }

            await db.SaveChangesAsync();
        }

        public async Task<UsuarioDto?> AutenticarAsync(string nombreUsuario, string contraseña)
        {
            using var db = dbContextFactory.CreateDbContext();
            var usuario = await db.Usuarios
                .Include(u => u.Departamento)
                .Include(u => u.Puesto)
                .Include(u => u.Sucursal)
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Nombre == nombreUsuario);

            if (usuario != null && VerifyPassword(contraseña, usuario.Contraseña))
            {
                return mapper.Map<UsuarioDto>(usuario);
            }
            return null;
        }

        public async Task<List<UsuarioDto>> ObtenerUsuariosPorRolAsync(int idRol)
        {
            using var db = dbContextFactory.CreateDbContext();
            var usuarios = await db.Usuarios
                .Where(u => u.IdRol == idRol)
                .Include(u => u.Departamento)
                .Include(u => u.Puesto)
                .Include(u => u.Sucursal)
                .Include(u => u.Rol)
                .ToListAsync();

            return mapper.Map<List<UsuarioDto>>(usuarios);
        }
        public async Task<List<UsuarioDto>> ObtenerUsuariosPorRolAsync(RolUsuarioEnum rol)
        {
            return await ObtenerUsuariosPorRolAsync((int)rol);
        }

        private static async Task CargarRelacionesAsync(HSisDbContext db, Usuario usuario)
        {
            await db.Entry(usuario).Reference(u => u.Departamento).LoadAsync();
            await db.Entry(usuario).Reference(u => u.Puesto).LoadAsync();
            await db.Entry(usuario).Reference(u => u.Sucursal).LoadAsync();
            await db.Entry(usuario).Reference(u => u.Rol).LoadAsync();
        }

    }
}

