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


    //[HttpGet]
    //public IActionResult GetPlanners()
    //{
    //    throw ();
    //}

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> PostFixedPlanner(CreatePlannerDto fixedPlanner)
    {
        ResponsePlannerMapper mapper = new();
        var result = await _PlannerService.CreatePlanner(fixedPlanner);
        
        return Ok(mapper.ToDto(result));
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll()
    {
        var result = await _PlannerService.GetAll();
        return Ok(result);
    }
}
