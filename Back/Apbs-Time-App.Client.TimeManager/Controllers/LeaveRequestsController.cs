using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.Leave;
using Shared.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

    [HttpGet("last/{id}")]
    public async Task<ActionResult<int>> GetLastLeaveRequestStatus(Guid id)
    {
        var status = await _leaveRequestService.GetLastLeaveRequestStatusAsync(id);
        if (status == null)
        {
            return NotFound();
        }
        return Ok(status);
    }

    [HttpPost]
    public async Task<ActionResult<LeaveRequestDto>> Create(CreateLeaveRequestDto createDto)
    {
        try
        {
            var leaveRequest = await _leaveRequestService.CreateLeaveRequestAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = leaveRequest.Id }, leaveRequest);
        }
        catch (InvalidOperationException ex) when (ex.Message == "Insufficient leave balance.")
        {
            return BadRequest(new { Message = "Insufficient leave balance for this request." });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<LeaveRequestDto>> Update(Guid id, UpdateLeaveRequestDto updateDto)
    {
        try
        {
            var leaveRequest = await _leaveRequestService.UpdateLeaveRequestAsync(id, updateDto);
            if (leaveRequest == null)
            {
                return NotFound();
            }
            return Ok(leaveRequest);
        }
        catch (InvalidOperationException ex) when (ex.Message == "Insufficient leave balance.")
        {
            return BadRequest(new { Message = "Insufficient leave balance for updated dates." });
        }
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

    [AllowAnonymous]
    [HttpGet("balance/{userId}")]
    public async Task<ActionResult<LeaveBalanceDto>> GetLeaveBalance(Guid userId)
    {
        var balance = await _leaveRequestService.GetLeaveBalanceByUserIdAsync(userId);
        if (balance == null)
        {
            return NotFound();
        }
        return Ok(balance);
    }

    [HttpPut("balance/{userId}")]
    public async Task<IActionResult> UpdateLeaveBalance(Guid userId, [FromBody] decimal newBalance)
    {
        if (newBalance < 0)
        {
            return BadRequest(new { Message = "Leave balance cannot be negative." });
        }

        var result = await _leaveRequestService.UpdateLeaveBalanceAsync(userId, newBalance);
        if (!result)
        {
            return StatusCode(500, new { Message = "Failed to update leave balance." });
        }
        return Ok(new { Message = "Leave balance updated successfully." });
    }

    [AllowAnonymous]
    [HttpPost("balance/allocate/{userId}")]
    public async Task<IActionResult> AllocateMonthlyLeave(Guid userId, [FromBody] decimal monthlyAllocation)
    {
        if (monthlyAllocation < 0)
        {
            return BadRequest(new { Message = "Monthly allocation cannot be negative." });
        }

        var result = await _leaveRequestService.AllocateMonthlyLeaveAsync(userId, monthlyAllocation);
        if (!result)
        {
            return Ok(new { Message = "No allocation needed for this month." });
        }
        return Ok(new { Message = "Monthly leave allocated successfully." });
    }
}