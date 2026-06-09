using Mapster;
using HSis.Data.Models;
using HSis.Logic.DTOs;

namespace HSis.Logic.Profiles
{
    public class UserProfile : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            // Entidad a DTO (Ocultamos la contraseña por seguridad)
            config.NewConfig<Usuario, UsuarioDto>()
                .Map(dest => dest.DepartamentoNombre, src => src.IdDepartamentoNavigation != null ? src.IdDepartamentoNavigation.Nombre : string.Empty)
                .Map(dest => dest.PuestoNombre, src => src.IdPuestoNavigation != null ? src.IdPuestoNavigation.Nombre : string.Empty)
                .Map(dest => dest.SucursalNombre, src => src.IdSucursalNavigation != null ? src.IdSucursalNavigation.Nombre : string.Empty)
                .Ignore(dest => dest.Contraseña!);

            // DTO a Entidad
            config.NewConfig<UsuarioDto, Usuario>()
                .Ignore(dest => dest.Contraseña!); // No mapeamos contraseña automáticamente
        }
    }
}
