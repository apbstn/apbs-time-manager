using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Services;

public interface IUserTenantRepository
{
    Task<Guid?> GetIdByEmailAsync(string email);
    Task<IEnumerable<UserTenant>> GetAllAccountsAsync();
}
