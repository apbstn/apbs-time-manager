using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Context;
using Shared.DTOs.TenantDtos;
using Shared.DTOs.TenantDtos.Mappers;
using Shared.Models;
using Shared.Services;

namespace Apbs_Time_App.Client.TimeManager.Controllers;


    [ApiController]
    [Route("api/[controller]")]
    public class TeamsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TeamsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/teams
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Team>>> GetTeams()
        {
            return await _context.Teams.ToListAsync();
        }

        // GET: api/teams/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Team>> GetTeam(Guid id)
        {
            var team = await _context.Teams.FindAsync(id);
            return team == null ? NotFound() : team;
        }

        // POST: api/teams
        [HttpPost]
        public async Task<ActionResult<Team>> CreateTeam(Team team)
        {
            _context.Teams.Add(team);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetTeam), new { id = team.Id }, team);
        }

        // PUT: api/teams/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTeam(Guid id, Team team)
        {
            if (id != team.Id) return BadRequest();

            _context.Entry(team).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/teams/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeam(Guid id)
        {
            var team = await _context.Teams.FindAsync(id);
            if (team == null) return NotFound();

            _context.Teams.Remove(team);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}

