using System;
using System.Runtime.Versioning;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;

namespace HSis.UI
{
    [SupportedOSPlatform("windows")]
    public class FormFactory(IServiceProvider serviceProvider) : IFormFactory
    {
        public T Create<T>() where T : Form
        {
            return serviceProvider.GetRequiredService<T>();
        }

        public DetalleClienteForm CreateDetalleCliente(int idTicket)
        {
            return ActivatorUtilities.CreateInstance<DetalleClienteForm>(serviceProvider, idTicket);
        }

        public TicketDetalleForm CreateTicketDetalle(int idTicket)
        {
            return ActivatorUtilities.CreateInstance<TicketDetalleForm>(serviceProvider, idTicket);
        }

        public EditorDinamicoForm CreateEditorDinamico(object entidad, string titulo)
        {
            return ActivatorUtilities.CreateInstance<EditorDinamicoForm>(serviceProvider, entidad, titulo);
        }
    }
}
