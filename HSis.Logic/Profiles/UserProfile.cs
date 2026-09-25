using HSis.Data.Models;
using HSis.Contracts.DTOs;
using HSis.Logic.Helpers;
using Mapster;

namespace HSis.Logic.Profiles
{
    public class UserProfile : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            // Entidad a DTO (Ocultamos la contraseña por seguridad)
            config.NewConfig<Usuario, UsuarioDto>()
                .Map(dest => dest.Nombre, src => NombreDisplayHelper.Normalizar(src.Nombre))
                .Map(dest => dest.DepartamentoNombre, src => NombreDisplayHelper.Normalizar(src.Departamento != null ? src.Departamento.Nombre : null))
                .Map(dest => dest.PuestoNombre, src => NombreDisplayHelper.Normalizar(src.Puesto != null ? src.Puesto.Nombre : null))
                .Map(dest => dest.SucursalNombre, src => NombreDisplayHelper.Normalizar(src.Sucursal != null ? src.Sucursal.Nombre : null))
                .Map(dest => dest.RolNombre, src => NombreDisplayHelper.Normalizar(src.Rol != null ? src.Rol.Descripcion : null))
                .Ignore(dest => dest.Contraseña!);

            // DTO a Entidad
            config.NewConfig<UsuarioDto, Usuario>()
                .Ignore(dest => dest.Contraseña!); // No mapeamos contraseña automáticamente
        }
    }
}

