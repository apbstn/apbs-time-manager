using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.Leave;
using Shared.Services;
using System.Security.Claims;

namespace Apbs_Time_App.Client.TimeManager.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class LeaveRequestsController : ControllerBase
{
    private readonly ILeaveRequestService _leaveRequestService;

    public LeaveRequestsController(ILeaveRequestService leaveRequestService)
    {
        _leaveRequestService = leaveRequestService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LeaveRequestDto>>> GetAll()
    {
        var leaveRequests = await _leaveRequestService.GetAllLeaveRequestsAsync();
        return Ok(leaveRequests);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LeaveRequestDto>> GetById(Guid id)
    {
        var leaveRequest = await _leaveRequestService.GetLeaveRequestByIdAsync(id);
        if (leaveRequest == null)
        {
            return NotFound();
        }
        return Ok(leaveRequest);
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<LeaveRequestDto>>> GetByUserId(Guid userId)
    {
        var leaveRequests = await _leaveRequestService.GetLeaveRequestsByUserIdAsync(userId);
        return Ok(leaveRequests);
    }

    [HttpPost]
    public async Task<ActionResult<LeaveRequestDto>> Create(CreateLeaveRequestDto createDto)
    {
        var userId = Request.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        var leaveRequest = await _leaveRequestService.CreateLeaveRequestAsync(Guid.Parse(userId), createDto);
        return CreatedAtAction(nameof(GetById), new { id = leaveRequest.Id }, leaveRequest);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<LeaveRequestDto>> Update(Guid id, UpdateLeaveRequestDto updateDto)
    {
        var leaveRequest = await _leaveRequestService.UpdateLeaveRequestAsync(id, updateDto);
        if (leaveRequest == null)
        {
            return NotFound();
        }
        return Ok(leaveRequest);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _leaveRequestService.DeleteLeaveRequestAsync(id);
        if (!result)
        {
            return NotFound();
        }
        return NoContent();
    }
}
