using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.PlannerDtos.Create;
using Shared.DTOs.PlannerDtos.Create.Mapper;
using Shared.DTOs.PlannerDtos.Response.Mapper;
using Shared.Services;

namespace Apbs_Time_App.Client.TimeManager.Controllers;

[Route("api/planner")]
[ApiController]
public class PlannerController : ControllerBase
{
    private readonly IPlannerService _PlannerService;

    public PlannerController(IPlannerService PlannerService)
    {
        _PlannerService = PlannerService;
    }

    [HttpPost]
    [Authorize(Roles = "Owner")]
    public async Task<IActionResult> PostFixedPlanner(CreatePlannerDto fixedPlanner)
    {
        try { 
            ResponsePlannerMapper mapper = new();
            var result = await _PlannerService.CreatePlanner(fixedPlanner);
            return Ok(mapper.ToDto(result));
        }
        catch(Exception ex)
        {
            return Unauthorized(ex.ToString());
        }
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll()
    {
        var result = await _PlannerService.GetAll();
        return Ok(result);
    }

    //[HttpDelete("{id}")]
    //[Authorize(Roles = "Owner")]
    //public async Task<IActionResult> Delete(Guid id)
    //{
        
    //}

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetPlanner()
    {
        var result = await _PlannerService.GetAll();
        return Ok(result);
    }
}
