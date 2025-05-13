using Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Shared.Services
{
    public interface ITeamService
    {
        Task<List<TeamDTO>> GetAllTeamsAsync();
        Task<TeamDTO?> GetTeamByIdAsync(Guid id);
        Task<TeamDTO> CreateTeamAsync(CreateTeamDTO teamDTO);
        Task<TeamDTO?> UpdateTeamAsync(Guid id, UpdateTeamDTO teamDTO);
        Task<bool> DeleteTeamAsync(Guid id);
    }
}
