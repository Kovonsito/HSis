using HSis.Logic.Services;

namespace HSis.UI
{
    public class CurrentUserService : ICurrentUserService
    {
        public int GetCurrentUserId()
        {
            return SesionSistema.IdUsuario;
        }
    }
}
