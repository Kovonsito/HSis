using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Text;
using HSis.Contracts.Services;

namespace HSis.Desktop.Infrastructure;

[SupportedOSPlatform("windows")]
public sealed class AlmacenamientoCredencialesLocal : IAlmacenamientoCredencialesLocal
{
    private static readonly byte[] Entropy = [14, 55, 99, 102, 23, 76, 5, 88];
    private readonly string _cacheFilePath;

    public AlmacenamientoCredencialesLocal(string? cacheFilePath = null)
    {
        _cacheFilePath = string.IsNullOrWhiteSpace(cacheFilePath)
            ? ConfiguracionEscritorio.ObtenerRutaDatos(ConfiguracionEscritorio.ArchivoCredenciales)
            : cacheFilePath;
    }

    public void SaveCredentials(string username, string password)
    {
        try
        {
            var directory = Path.GetDirectoryName(_cacheFilePath);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var rawBytes = Encoding.UTF8.GetBytes($"{username}\t{password}");
            var encryptedBytes = ProtectedData.Protect(
                rawBytes,
                Entropy,
                DataProtectionScope.CurrentUser);

            File.WriteAllBytes(_cacheFilePath, encryptedBytes);
        }
        catch
        {
        }
    }

    public (string Username, string Password)? GetCredentials()
    {
        try
        {
            if (!File.Exists(_cacheFilePath))
            {
                return null;
            }

            var encryptedBytes = File.ReadAllBytes(_cacheFilePath);
            var decryptedBytes = ProtectedData.Unprotect(
                encryptedBytes,
                Entropy,
                DataProtectionScope.CurrentUser);
            var rawData = Encoding.UTF8.GetString(decryptedBytes);
            var separatorIndex = rawData.IndexOf('\t');

            if (separatorIndex <= 0 || separatorIndex == rawData.Length - 1)
            {
                return null;
            }

            var username = rawData[..separatorIndex];
            var password = rawData[(separatorIndex + 1)..];
            return string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password)
                ? null
                : (username, password);
        }
        catch
        {
            ClearCredentials();
            return null;
        }
    }

    public void ClearCredentials()
    {
        try
        {
            if (File.Exists(_cacheFilePath))
            {
                File.Delete(_cacheFilePath);
            }
        }
        catch
        {
        }
    }
}
