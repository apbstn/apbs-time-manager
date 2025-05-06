using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Context;
using Shared.DTOs;
using Shared.DTOs.TenantDtos;
using Shared.DTOs.TenantDtos.Mappers;
using Shared.Models;
using Shared.Services;

namespace Apbs_Time_App.Client.TimeManager.Controllers;

[Authorize(Roles = "Owner")]
[Route("api/teams")]
[ApiController]
public class TeamsController : ControllerBase
{
    private readonly ITeamService _teamService;

    public TeamsController(ITeamService teamService)
    {
        _teamService = teamService;
    }
    [HttpGet("test")]  // Responds to GET /api/teams/test
    public IActionResult Test()
    {
        return Ok("API is working");
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TeamDTO>>> GetTeams()
    {
        var teams = await _teamService.GetAllTeamsAsync();
        return Ok(teams);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TeamDTO>> GetTeam(Guid id)
    {
        var team = await _teamService.GetTeamByIdAsync(id);
        if (team == null) return NotFound();
        return Ok(team);
    }

    [HttpPost]
    public async Task<ActionResult<TeamDTO>> PostTeam(CreateTeamDTO teamDTO)
    {
        var createdTeam = await _teamService.CreateTeamAsync(teamDTO);
        return CreatedAtAction(nameof(GetTeam), new { id = createdTeam.Id }, createdTeam);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutTeam(Guid id, UpdateTeamDTO teamDTO)
    {
        var updatedTeam = await _teamService.UpdateTeamAsync(id, teamDTO);
        if (updatedTeam == null) return NotFound();
        return Ok(updatedTeam);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTeam(Guid id)
    {
        var success = await _teamService.DeleteTeamAsync( id);
        if (!success) return NotFound();
        return Ok("Team Deleted !!");
    }
}    


