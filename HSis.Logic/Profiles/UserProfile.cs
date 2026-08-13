using HSis.Data.Models;
using HSis.Logic.DTOs;
using Mapster;

namespace HSis.Logic.Profiles
{
    public class UserProfile : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            // Entidad a DTO (Ocultamos la contraseña por seguridad)
            config.NewConfig<Usuario, UsuarioDto>()
                .Map(dest => dest.DepartamentoNombre, src => src.Departamento != null ? src.Departamento.Nombre : string.Empty)
                .Map(dest => dest.PuestoNombre, src => src.Puesto != null ? src.Puesto.Nombre : string.Empty)
                .Map(dest => dest.SucursalNombre, src => src.Sucursal != null ? src.Sucursal.Nombre : string.Empty)
                .Ignore(dest => dest.Contraseña!);

            // DTO a Entidad
            config.NewConfig<UsuarioDto, Usuario>()
                .Ignore(dest => dest.Contraseña!); // No mapeamos contraseña automáticamente
        }
    }
}

