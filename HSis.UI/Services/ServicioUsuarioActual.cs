#nullable enable
using System.Runtime.Versioning;
using HSis.Logic.Services;

namespace HSis.UI
{
    [SupportedOSPlatform("windows")]
    public class CurrentUserService : ICurrentUserService
    {
        public int GetCurrentUserId()
        {
            return SesionSistema.IdUsuario;
        }
    }
}
