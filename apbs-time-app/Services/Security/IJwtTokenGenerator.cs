using Shared.Models;

namespace apbs_time_app.Services.Security;

public interface IJwtTokenGenerator
{
    Task<string> GenerateAuthToken(User user);
    Task<string> GenerateAccessToken(User user);
    Task<string> GenerateAccessToken(User user, string tenatId, string authToken);
}
