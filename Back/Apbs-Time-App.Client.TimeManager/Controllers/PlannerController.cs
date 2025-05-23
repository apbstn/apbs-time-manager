using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.PlannerDtos.Create;
using Shared.DTOs.PlannerDtos.Create.Mapper;
using Shared.Models.Planners;
using Shared.Services.Planner;

namespace Apbs_Time_App.Client.TimeManager.Controllers;

[Route("api/planner")]
[ApiController]
public class PlannerController : ControllerBase
{
    private readonly IFixedPlannerService _fixedPlannerService;

    public PlannerController(IFixedPlannerService fixedPlannerService)
    {
        _fixedPlannerService = fixedPlannerService;
    }


    //[HttpGet]
    //public IActionResult GetPlanners()
    //{
    //    throw ();
    //}

    [HttpPost("fixed")]
    [Authorize]
    public async Task<IActionResult> PostFixedPlanner(CreateFixedPlannerDto fixedPlanner)
    {
        var result = await _fixedPlannerService.CreatePlanner(fixedPlanner);
        return Ok(result);
    }


    [HttpPost]
    [Authorize]
    public async Task<IActionResult> PostFlexiblePlanner(CreateFlexiblePlannerMapper flexiblePlanner)
    {
        return Ok();
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll()
    {
        var result = await _fixedPlannerService.GetAll();
        return Ok(result);
    }
}
