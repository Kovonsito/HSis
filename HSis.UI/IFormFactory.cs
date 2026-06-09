using System.Windows.Forms;

namespace HSis.UI
{
    public interface IFormFactory
    {
        T Create<T>() where T : Form;
        frmDetalleCliente CreateDetalleCliente(int idTicket);
        frmTicketDetalle CreateTicketDetalle(int idTicket);
        frmEditorDinamico CreateEditorDinamico(object entidad, string titulo);
    }
}
