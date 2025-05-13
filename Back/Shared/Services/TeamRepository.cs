using Microsoft.EntityFrameworkCore;
using Shared.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Models;

namespace Shared.Services
{
    public class TeamRepository : ITeamRepository
    {
        private readonly ApplicationDbContext _context;

        public TeamRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Team>> GetAllTeamsAsync()
        {
            return await _context.Teams.ToListAsync();
        }

        public async Task<Team?> GetTeamByIdAsync(Guid id)
        {
            return await _context.Teams.FindAsync(id);
        }

        public async Task<Team> AddTeamAsync(Team Team)
        {
            _context.Teams.Add(Team);
            await _context.SaveChangesAsync();
            return Team;
        }

        public async Task<Team?> UpdateTeamAsync(Team team)
        {
            var existingTeam = await _context.Teams.FindAsync(team.Id);
            if (existingTeam == null) return null;

            existingTeam.Name = team.Name;
            existingTeam.Description = team.Description;

            await _context.SaveChangesAsync();
            return existingTeam;
        }

        public async Task<Team?> DeleteTeamAsync(Guid id)
        {
            var team = await _context.Teams.FindAsync(id);
            if (team == null) return null;

            _context.Teams.Remove(team);
            await _context.SaveChangesAsync();
            return team;
        }
    }
}
