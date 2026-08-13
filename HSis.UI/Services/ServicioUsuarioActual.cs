#nullable enable
using System.Runtime.Versioning;
using HSis.Logic.Services;
using HSis.UI.Helpers;

namespace HSis.UI.Services
{
    [SupportedOSPlatform("windows")]
    public class ServicioUsuarioActual : ICurrentUserService
    {
        public int GetCurrentUserId()
        {
            return SesionSistema.IdUsuario;
        }

    }
}

