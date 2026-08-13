using HSis.Data.Models;
using HSis.Logic.DTOs;
using Mapster;

namespace HSis.Logic.Profiles
{
    public class TicketProfile : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            // Entidad a DTO
            config.NewConfig<Ticket, TicketDto>()
                .Map(dest => dest.NombreUsuario, src => src.Usuario != null ? src.Usuario.Nombre : string.Empty)
                .Map(dest => dest.NombreTecnico, src => src.Tecnico != null ? src.Tecnico.Nombre : string.Empty)
                .Map(dest => dest.DepartamentoUsuario, src => src.Usuario != null && src.Usuario.Departamento != null ? src.Usuario.Departamento.Nombre : string.Empty);

            // Create DTO a Entidad
            config.NewConfig<TicketCreateDto, Ticket>()
                .Ignore(dest => dest.FechaAlta!)
                .Ignore(dest => dest.Estatus!);

            // Update DTO a Entidad
            config.NewConfig<TicketUpdateDto, Ticket>();

            // DetTicket <-> TicketDetalleDto
            config.NewConfig<DetTicket, TicketDetalleDto>()
                .Map(dest => dest.NombreMaterial, src => src.Material != null ? src.Material.Nombre : null)
                .Map(dest => dest.UnidadMedidaMaterial, src => src.Material != null ? src.Material.UnidadMedida : null);

            config.NewConfig<TicketDetalleDto, DetTicket>();
        }
    }
}

