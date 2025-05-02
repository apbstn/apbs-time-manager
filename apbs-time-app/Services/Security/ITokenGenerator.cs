namespace apbs_time_app.Services.Security;

public interface ITokenGenerator
{
    public string Gen(int length);
}
