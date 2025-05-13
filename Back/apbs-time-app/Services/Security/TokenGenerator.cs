namespace apbs_time_app.Services.Security;

public class TokenGenerator : ITokenGenerator
{
    public string Gen(int length = 64)
    {
        byte[] bytes = new byte[length];
        return Convert.ToBase64String(bytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");
    }
}
