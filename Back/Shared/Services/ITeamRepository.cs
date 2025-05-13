using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Models;

namespace Shared.Services
{
    public interface ITeamRepository
    {
        Task<List<Team>> GetAllTeamsAsync();
        Task<Team?> GetTeamByIdAsync(Guid id);
        Task<Team> AddTeamAsync(Team team);
        Task<Team?> UpdateTeamAsync(Team team);
        Task<Team?> DeleteTeamAsync(Guid id);
    }
}
