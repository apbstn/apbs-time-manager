using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using Shared.Services;

namespace Apbs_Time_App.Client.TimeManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeaveRequestsController : ControllerBase
    {
        private readonly ILeaveRequestService _leaveRequestService;

        public LeaveRequestsController(ILeaveRequestService leaveRequestService)
        {
            _leaveRequestService = leaveRequestService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LeaveRequestRequest request)
        {
            try
            {
                var result = await _leaveRequestService.CreateLeaveRequest(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] LeaveRequestRequest request)
        {
            try
            {
                var result = await _leaveRequestService.UpdateLeaveRequest(id, request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await _leaveRequestService.DeleteLeaveRequest(id);
                return success ? Ok() : NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var result = await _leaveRequestService.GetLeaveRequest(id);
                return result != null ? Ok(result) : NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("employee/{employeeId}")]
        public async Task<IActionResult> GetEmployeeHistory(int employeeId)
        {
            try
            {
                var result = await _leaveRequestService.GetEmployeeLeaveHistory(employeeId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("status/{id}")]
        public async Task<IActionResult> UpdateStatus(int id, [FromQuery] string action)
        {
            try
            {
                var result = await _leaveRequestService.UpdateLeaveStatus(id, action);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("type/{type}/{employeeId}")]
        public async Task<IActionResult> GetByType(string type, int employeeId)
        {
            try
            {
                var result = await _leaveRequestService.GetLeaveRequestsByType(type, employeeId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _leaveRequestService.GetAllLeaveRequests();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("balance/{employeeId}")]
        public async Task<IActionResult> GetBalance(int employeeId)
        {
            try
            {
                var result = await _leaveRequestService.GetEmployeeLeaveBalance(employeeId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
