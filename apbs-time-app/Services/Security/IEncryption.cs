namespace apbs_time_app.Services.Security;

public interface IEncryption
{
    public string EncryptString(string plainText);
    public string DecryptString(string cipherText);
}
