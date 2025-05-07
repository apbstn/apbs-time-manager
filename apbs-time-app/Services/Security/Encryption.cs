using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NPOI.SS.Formula.Functions;
using System.Security.Cryptography;
using System.Text;

namespace apbs_time_app.Services.Security;

public class Encryption : IEncryption
{
    const string _key = "HcUaHxjxx8Xp8PLC2eaBb0guth//xFrFj7L8kqlZK9tobCCRrVyx+sIC86ZUAT4AWHUVbzy3w5uCFFMNmmXhAKvQUuIRUbjBhJVzKR1af/nFdqL9l6b5P4dcnO1I2EadD0APqLcNQzBgLbEc17uOtMZ16w1fvbgz9SPSu91Nofs=";

    public string DecryptString(string cipherText)
    {
        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(_key.PadRight(32).Substring(0, 32));
        aes.IV = new byte[16];

        var buffer = Base64UrlDecode(cipherText);
        var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

        using var ms = new MemoryStream(buffer);
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var sr = new StreamReader(cs);

        return sr.ReadToEnd();
    }

    public string EncryptString(string plainText)
    {
        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(_key.PadRight(32).Substring(0, 32));
        aes.IV = new byte[16];

        var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

        using var ms = new MemoryStream();
        using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
        using (var sw = new StreamWriter(cs))
            sw.Write(plainText);

        return Base64UrlEncode(ms.ToArray());
    }


    private string Base64UrlEncode(byte[] input)
    {
        return Convert.ToBase64String(input)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    private byte[] Base64UrlDecode(string input)
    {
        string padded = input
            .Replace('-', '+')
            .Replace('_', '/');

        switch (padded.Length % 4)
        {
            case 2: padded += "=="; break;
            case 3: padded += "="; break;
        }

        return Convert.FromBase64String(padded);
    }

}
