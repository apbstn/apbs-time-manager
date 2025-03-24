using Shared.Models;

namespace apbs_time_app_admin.Services.Security;

public interface IJwtTokenGenerator
{
    Task<string> Generate(User user);
}
