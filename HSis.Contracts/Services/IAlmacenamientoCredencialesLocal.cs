namespace HSis.Contracts.Services
{
    public interface IAlmacenamientoCredencialesLocal
    {
        void SaveCredentials(string username, string password);
        (string Username, string Password)? GetCredentials();
        void ClearCredentials();
    }
}
