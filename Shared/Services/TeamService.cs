using Shared.DTOs;
using Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shared.Services
{
    public class TeamService : ITeamService
    {
        private readonly ITeamRepository _teamRepository;

        public TeamService(ITeamRepository teamRepository)
        {
            _teamRepository = teamRepository;
        }

        public async Task<List<TeamDTO>> GetAllTeamsAsync()
        {
            var teams = await _teamRepository.GetAllTeamsAsync();
            return teams.Select(t => new TeamDTO
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description
            }).ToList();
        }

        public async Task<TeamDTO?> GetTeamByIdAsync(Guid id)
        {
            var team = await _teamRepository.GetTeamByIdAsync(id);
            return team is null ? null : new TeamDTO
            {
                Id = team.Id,
                Name = team.Name,
                Description = team.Description
            };
        }

        public async Task<TeamDTO> CreateTeamAsync(CreateTeamDTO teamDTO)
        {
            var team = new Team
            {
                Name = teamDTO.Name,
                Description = teamDTO.Description
            };

            var createdTeam = await _teamRepository.AddTeamAsync(team);
            return new TeamDTO
            {
                Id = createdTeam.Id,
                Name = createdTeam.Name,
                Description = createdTeam.Description
            };
        }

        public async Task<TeamDTO?> UpdateTeamAsync(Guid id, UpdateTeamDTO teamDTO)
        {
            var existingTeam = await _teamRepository.GetTeamByIdAsync(id);
            if (existingTeam is null) return null;

            existingTeam.Name = teamDTO.Name;
            existingTeam.Description = teamDTO.Description;

            var updatedTeam = await _teamRepository.UpdateTeamAsync(existingTeam);
            return updatedTeam is null ? null : new TeamDTO
            {
                Id = updatedTeam.Id,
                Name = updatedTeam.Name,
                Description = updatedTeam.Description
            };
        }

        public async Task<bool> DeleteTeamAsync(Guid id)
        {
            var team = await _teamRepository.DeleteTeamAsync(id);
            return team is not null;
        }
    }
}