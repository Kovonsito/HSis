using System.Windows.Forms;

namespace HSis.UI
{
    public interface IFormFactory
    {
        T Create<T>() where T : Form;
        DetalleClienteForm CreateDetalleCliente(int idTicket);
        TicketDetalleForm CreateTicketDetalle(int idTicket);
        EditorDinamicoForm CreateEditorDinamico(object entidad, string titulo);
    }
}
