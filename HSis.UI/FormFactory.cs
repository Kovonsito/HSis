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

        public frmDetalleCliente CreateDetalleCliente(int idTicket)
        {
            return ActivatorUtilities.CreateInstance<frmDetalleCliente>(serviceProvider, idTicket);
        }

        public frmTicketDetalle CreateTicketDetalle(int idTicket)
        {
            return ActivatorUtilities.CreateInstance<frmTicketDetalle>(serviceProvider, idTicket);
        }

        public frmEditorDinamico CreateEditorDinamico(object entidad, string titulo)
        {
            return ActivatorUtilities.CreateInstance<frmEditorDinamico>(serviceProvider, entidad, titulo);
        }
    }
}
