using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using Shared.Context;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Services
{
    public class TeamRepository : ITeamRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TeamService>? _logger;

        public TeamRepository(ApplicationDbContext context, ILogger<TeamService>? logger = null)
        {
            _context = context;
            _logger = logger;
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
            if (id == Guid.Empty)
            {
                _logger?.LogWarning("Team ID cannot be empty.");
                throw new ArgumentException("Team ID cannot be empty.", nameof(id));
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var team = await _context.Teams.FindAsync(id);
                if (team == null)
                {
                    _logger?.LogWarning("Team with ID {TeamId} not found.", id);
                    return null;
                }

                // Update Users table to remove references to the team
                var usersUpdated = await _context.Users
                    .Where(u => u.TeamId == id)
                    .ExecuteUpdateAsync(setters => setters.SetProperty(u => u.TeamId, (Guid?)null));
                _logger?.LogInformation("Updated {Count} users to remove TeamId {TeamId}.", usersUpdated, id);

                // Remove the team
                _context.Teams.Remove(team);
                _logger?.LogInformation("Deleted team with ID {TeamId}.", id);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return team;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger?.LogError(ex, "Failed to delete team with ID {TeamId}.", id);
                throw new InvalidOperationException($"Failed to delete team with ID {id}: {ex.Message}", ex);
            }
        }
    }
}
