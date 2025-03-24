using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;

namespace Shared.Services;

public class EncryptionService : IEncryptionService
{
    private readonly string _base64key = "j1pHRHKFA7qQtB1HuFFglp53Mp9GLXPvVtKBDmvNsH8=";
    private readonly byte[] _key;
    public EncryptionService(IConfiguration configuration)
    {
        _key = Convert.FromBase64String(_base64key);
    }

    public string Decrypt(string cipherText)
    {
        var buffer = Convert.FromBase64String(cipherText);

        using (var aes = Aes.Create())
        {
            aes.Key = _key;

            // Read IV from the beginning of the buffer
            var iv = new byte[16];
            Array.Copy(buffer, 0, iv, 0, iv.Length);
            aes.IV = iv;

            var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using (var ms = new MemoryStream(buffer, iv.Length, buffer.Length - iv.Length))
            using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
            using (var sr = new StreamReader(cs))
            {
                return sr.ReadToEnd();
            }
        }
    }

    public string Encrypt(string plainText)
    {
        using (var aes = Aes.Create())
        {
            aes.Key = _key;
            aes.GenerateIV();

            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using (var ms = new MemoryStream())
            {
                ms.Write(aes.IV, 0, aes.IV.Length); // Write IV to the stream
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                using (var sw = new StreamWriter(cs))
                {
                    sw.Write(plainText);
                }

                return Convert.ToBase64String(ms.ToArray());
            }
        }
    }
}
