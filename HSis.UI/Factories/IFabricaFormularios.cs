using HSis.UI.Forms.Otros;
using HSis.UI.Forms.Tickets;

namespace HSis.UI.Factories
{
    public interface IFabricaFormularios
    {
        T Crear<T>() where T : Form;
        DetalleClienteForm CrearDetalleCliente(int idTicket);
        TicketDetalleForm CrearTicketDetalle(int idTicket);
        EditorDinamicoForm CrearEditorDinamico(object entidad, string titulo);
    }
}
