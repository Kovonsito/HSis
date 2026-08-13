using System.Runtime.Versioning;
using HSis.UI.Forms.Otros;
using HSis.UI.Forms.Tickets;
using Microsoft.Extensions.DependencyInjection;

namespace HSis.UI.Factories
{
    [SupportedOSPlatform("windows")]
    public class FabricaFormularios(IServiceProvider serviceProvider) : IFabricaFormularios
    {
        public T Crear<T>() where T : Form
        {
            return serviceProvider.GetRequiredService<T>();
        }


        public DetalleClienteForm CrearDetalleCliente(int idTicket)
        {
            return ActivatorUtilities.CreateInstance<DetalleClienteForm>(serviceProvider, idTicket);
        }


        public TicketDetalleForm CrearTicketDetalle(int idTicket)
        {
            return ActivatorUtilities.CreateInstance<TicketDetalleForm>(serviceProvider, idTicket);
        }


        public EditorDinamicoForm CrearEditorDinamico(object entidad, string titulo)
        {
            return ActivatorUtilities.CreateInstance<EditorDinamicoForm>(serviceProvider, entidad, titulo);
        }

    }
}
