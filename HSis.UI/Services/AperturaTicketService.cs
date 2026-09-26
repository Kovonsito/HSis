using System.Runtime.Versioning;
using HSis.Contracts.Constants;
using HSis.Contracts.Services;
using HSis.UI.Factories;

namespace HSis.UI.Services;

[SupportedOSPlatform("windows")]
public sealed class AperturaTicketService(
    SolicitudAperturaTicket solicitud,
    IFabricaFormularios fabricaFormularios,
    IAdministradorSesionUsuario contextoSesion)
{
    public void AbrirPendiente(Form owner)
    {
        ArgumentNullException.ThrowIfNull(owner);

        if (contextoSesion.IdUsuario <= 0)
        {
            return;
        }

        var ticketId = solicitud.Consumir();
        if (!ticketId.HasValue)
        {
            return;
        }

        Form detalle = contextoSesion.IdRolUsuario == (int)RolUsuarioEnum.Cliente
            ? (Form)fabricaFormularios.CrearDetalleCliente(ticketId.Value)
            : fabricaFormularios.CrearTicketDetalle(ticketId.Value);

        using (detalle)
        {
            if (owner.IsDisposed)
            {
                return;
            }

            if (owner.WindowState == FormWindowState.Minimized)
            {
                owner.WindowState = FormWindowState.Normal;
            }

            owner.Activate();
            detalle.ShowDialog(owner);
        }
    }
}
