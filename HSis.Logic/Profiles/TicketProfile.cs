using HSis.Data.Models;
using HSis.Contracts.DTOs;
using HSis.Logic.Helpers;
using Mapster;

namespace HSis.Logic.Profiles
{
    public class TicketProfile : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            // Entidad a DTO
            config.NewConfig<Ticket, TicketDto>()
                .Map(dest => dest.NombreUsuario, src => NombreDisplayHelper.Normalizar(src.Usuario != null ? src.Usuario.Nombre : null))
                .Map(dest => dest.NombreTecnico, src => NombreDisplayHelper.Normalizar(src.Tecnico != null ? src.Tecnico.Nombre : null))
                .Map(dest => dest.DepartamentoUsuario, src => NombreDisplayHelper.Normalizar(src.Usuario != null && src.Usuario.Departamento != null ? src.Usuario.Departamento.Nombre : null));

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

