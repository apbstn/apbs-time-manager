
    using Microsoft.AspNetCore.Mvc;
    using Shared.Services;
    using Shared.Models;
    using Shared.DTOs;
    using static Shared.Services.TimeLogService;
    using Microsoft.Extensions.Logging;
    using Microsoft.AspNetCore.Authorization;

namespace Apbs_Time_App.Client.TimeManager.Controllers
{
    [ApiController]
        [Route("api/timelog")]
        public class TimeLogController : ControllerBase
        {
            private readonly TimeLogService _timeLogService;
            private readonly ILogger<TimeLogController> _logger;

            public TimeLogController(TimeLogService timeLogService, ILogger<TimeLogController> logger)
            {
                _timeLogService = timeLogService;
                _logger = logger;
            }

            [HttpPost("start/{accountId}")]
            [Authorize]
            public ActionResult StartTracking(int accountId)
            {
                var result = _timeLogService.StartTracking(accountId);
                if (result.Success)
                {
                    return Ok("Tracking started successfully.");
                }
                else
                {
                    _logger.LogWarning(result.Exception.Message);
                    return BadRequest(result.Exception.Message);
                }
            }

            [HttpPost("pause/{accountId}")]
            [Authorize]
            public ActionResult PauseTracking(int accountId)
            {
                var result = _timeLogService.PauseTracking(accountId);
                if (result.Success)
                {
                    return Ok("Tracking paused successfully.");
                }
                else
                {
                    _logger.LogWarning(result.Exception.Message);
                    return BadRequest(result.Exception.Message);
                }
            }

            [HttpPost("stop/{accountId}")]
            [Authorize]
            public ActionResult StopTracking(int accountId)
            {
                var result = _timeLogService.StopTracking(accountId);
                if (result.Success)
                {
                    return Ok("Tracking stopped successfully.");
                }
                else
                {
                    _logger.LogWarning(result.Exception.Message);
                    return BadRequest(result.Exception.Message);
                }
            }

            [HttpGet("{accountId}")]
            [Authorize]
            public ActionResult<List<TimeLog>> GetLogs(int accountId)
            {
                var logs = _timeLogService.GetLogs(accountId);
                if (logs == null || logs.Count == 0)
                {
                    return NotFound("No logs found.");
                }
                return Ok(logs);
            }

            [HttpPut("update/{logId}")]
            [Authorize]
            public ActionResult UpdateLog(Guid logId, [FromBody] TimeLogUpdateDto updateDto)
            {
                // Parse the NewType string to TimeLogType enum
                if (!Enum.TryParse(updateDto.NewType, out TimeLogType newType))
                {
                    return BadRequest("Invalid time type.");
                }

                var result = _timeLogService.UpdateLog(logId, updateDto.NewTime, newType, updateDto.TotalHours);
                if (result.Success)
                {
                    return Ok("Log updated successfully.");
                }
                else
                {
                    _logger.LogWarning(result.Exception.Message);
                    return BadRequest(result.Exception.Message);
                }
            }
        }
    }
