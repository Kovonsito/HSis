using Mapster;
using HSis.Data.Models;
using HSis.Logic.DTOs;

namespace HSis.Logic.Profiles
{
    public class TicketProfile : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            // Entidad a DTO
            config.NewConfig<Ticket, TicketDto>()
                .Map(dest => dest.NombreUsuario, src => src.IdUsuarioNavigation != null ? src.IdUsuarioNavigation.Nombre : string.Empty)
                .Map(dest => dest.NombreTecnico, src => src.IdTecnicoNavigation != null ? src.IdTecnicoNavigation.Nombre : string.Empty)
                .Map(dest => dest.Descripcion, src => src.Descripción)
                .Map(dest => dest.Solucion, src => src.Solución)
                .Map(dest => dest.Atencion, src => src.Atención);

            // Create DTO a Entidad
            config.NewConfig<TicketCreateDto, Ticket>()
                .Map(dest => dest.Descripción, src => src.Descripcion)
                .Ignore(dest => dest.Alta)
                .Ignore(dest => dest.Status);

            // Update DTO a Entidad
            config.NewConfig<TicketUpdateDto, Ticket>()
                .Map(dest => dest.Solución, src => src.Solucion)
                .Map(dest => dest.Atención, src => src.Atencion);
        }
    }
}
