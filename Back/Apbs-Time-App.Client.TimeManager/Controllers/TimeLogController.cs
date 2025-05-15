
    using Microsoft.AspNetCore.Mvc;
    using Shared.Services;
    using Shared.Models;
    using Shared.DTOs;
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
        private readonly IExxception _ex;
            
            public TimeLogController(TimeLogService timeLogService, ILogger<TimeLogController> logger, IExxception ex)
            {
                _timeLogService = timeLogService;
                _logger = logger;
                _ex = ex;
            }

            [HttpPost("start/{accountId}")]
            [Authorize]
            public ActionResult StartTracking(Guid accountId)
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
            public ActionResult PauseTracking(Guid accountId)
            {
                var result = _timeLogService.PauseTracking(accountId);
                if (result.Success)
                {
                    return Ok("Tracking paused successfully.");
                }
                else
                {
                    _logger.LogWarning(_ex.Message);
                    return BadRequest(_ex.Message);
                }
            }

            [HttpPost("stop/{accountId}")]
            [Authorize]
            public ActionResult StopTracking(Guid accountId)
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
            public ActionResult<List<TimeLog>> GetLogs(Guid accountId)
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
