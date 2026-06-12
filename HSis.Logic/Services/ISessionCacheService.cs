namespace HSis.Logic.Services

{
    public interface ISessionCacheService
    {
        void SaveCredentials(string username, string password);
        (string Username, string Password)? GetCredentials();
        void ClearCredentials();
    }
}
