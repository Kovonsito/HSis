using AutoMapper;
using HSis.Data.Models;
using HSis.Logic.DTOs;

namespace HSis.Logic.Profiles
{
    public class TicketProfile : Profile
    {
        public TicketProfile()
        {
            // Entidad a DTO
            CreateMap<Ticket, TicketDto>()
                .ForMember(dest => dest.NombreUsuario, opt => opt.MapFrom(src => src.IdUsuarioNavigation != null ? src.IdUsuarioNavigation.Nombre : string.Empty))
                .ForMember(dest => dest.NombreTecnico, opt => opt.MapFrom(src => src.IdTecnicoNavigation != null ? src.IdTecnicoNavigation.Nombre : string.Empty))
                .ForMember(dest => dest.Descripcion, opt => opt.MapFrom(src => src.Descripción))
                .ForMember(dest => dest.Solucion, opt => opt.MapFrom(src => src.Solución))
                .ForMember(dest => dest.Atencion, opt => opt.MapFrom(src => src.Atención));

            // Create DTO a Entidad
            CreateMap<TicketCreateDto, Ticket>()
                .ForMember(dest => dest.Descripción, opt => opt.MapFrom(src => src.Descripcion))
                .ForMember(dest => dest.Alta, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore());

            // Update DTO a Entidad
            CreateMap<TicketUpdateDto, Ticket>()
                .ForMember(dest => dest.Solución, opt => opt.MapFrom(src => src.Solucion))
                .ForMember(dest => dest.Atención, opt => opt.MapFrom(src => src.Atencion));
        }
    }
}
