using System.Security.Cryptography;
using System.Text;

namespace HSis.Logic.Services;

[System.Runtime.Versioning.SupportedOSPlatform("windows")]
public class SessionCacheService
{
    private static readonly string CacheFilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "HSis",
        "session.bin"
    );

    // Entropía adicional opcional para robustecer el cifrado DPAPI
    private static readonly byte[] Entropy = [14, 55, 99, 102, 23, 76, 5, 88];

    public static void SaveCredentials(string username, string password)
    {
        try
        {
            var directory = Path.GetDirectoryName(CacheFilePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Unir usuario y contraseña delimitados por una pestaña o caracter especial
            var rawData = $"{username}\t{password}";
            var rawBytes = Encoding.UTF8.GetBytes(rawData);

            // Cifrar los bytes usando DPAPI (Cifrado a nivel de usuario de Windows actual)
            var encryptedBytes = ProtectedData.Protect(rawBytes, Entropy, DataProtectionScope.CurrentUser);

            File.WriteAllBytes(CacheFilePath, encryptedBytes);
        }
        catch
        {
            // Ignorar fallos de escritura o de cifrado de caché para que no altere el funcionamiento principal
        }
    }

    public static (string Username, string Password)? GetCredentials()
    {
        try
        {
            if (!File.Exists(CacheFilePath)) return null;

            var encryptedBytes = File.ReadAllBytes(CacheFilePath);

            // Decodificar los bytes usando DPAPI
            var decryptedBytes = ProtectedData.Unprotect(encryptedBytes, Entropy, DataProtectionScope.CurrentUser);
            var rawData = Encoding.UTF8.GetString(decryptedBytes);

            var parts = rawData.Split('\t');
            if (parts.Length == 2)
            {
                return (parts[0], parts[1]);
            }
        }
        catch
        {
            // Si el archivo está corrupto o la desencriptación falla, eliminamos el archivo corrupto
            ClearCredentials();
        }

        return null;
    }

    public static void ClearCredentials()
    {
        try
        {
            if (File.Exists(CacheFilePath))
            {
                File.Delete(CacheFilePath);
            }
        }
        catch
        {
            // Ignorar
        }
    }
}
