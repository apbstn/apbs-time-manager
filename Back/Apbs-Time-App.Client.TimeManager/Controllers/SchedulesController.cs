using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.PlannerDtos;
using Shared.Services;

namespace Apbs_Time_App.Client.TimeManager.Controllers;

[ApiController]
[Route("api/schedules")]
public class SchedulesController : ControllerBase
{
    private readonly IScheduleService _service;

    public SchedulesController(IScheduleService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult<ScheduleDto>> Create(CreateScheduleDto dto)
    {
        var schedule = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = schedule.Id }, schedule);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ScheduleDto>> GetById(int id)
    {
        var schedule = await _service.GetByIdAsync(id);
        if (schedule == null) return NotFound();
        return Ok(schedule);
    }

    [HttpGet]
    public async Task<ActionResult<List<ScheduleDto>>> GetAll()
    {
        var schedules = await _service.GetAllAsync();
        return Ok(schedules);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ScheduleDto>> Update(int id, UpdateScheduleDto dto)
    {
        var schedule = await _service.UpdateAsync(id, dto);
        if (schedule == null) return NotFound();
        return Ok(schedule);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var success = await _service.DeleteAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }
}
