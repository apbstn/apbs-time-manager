using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Shared.Services;
using Shared.Context;
using Shared.Models;
using Shared.Services;

namespace Shared.Services;


public class TimeLogService : ITimeLogService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<TimeLogService> _logger;
    private readonly IExxception _ex;

    public TimeLogService(ApplicationDbContext context, ILogger<TimeLogService> logger, IExxception ex)
    {
        _context = context;
        _logger = logger;
        _ex = ex;
    }

    private void CalculateTotalHours(TimeLog log, DateTime nextTime)
    {
        if (log == null) throw new ArgumentNullException(nameof(log), "TimeLog cannot be null.");
        log.TotalHours = nextTime - log.Time;
    }

    private TimeLog GetLastLog(Guid accountId)
    {
        return _context.TimeLogs
            .Where(t => t.UserId == accountId)
            .OrderByDescending(t => t.Time)
            .FirstOrDefault();
    }

    public Result StartTracking(Guid accountId)
    {
        var lastLog = GetLastLog(accountId);

        if (lastLog?.Type == TimeLogType.PE)
        {
            _logger.LogWarning("Tracking is already started for AccountId {AccountId}.", accountId);
            _ex.Message = "Tracking is already started. Cannot start again.";
            return new Result { Success = false };
        }

        if (lastLog != null && lastLog.Type != TimeLogType.P && lastLog.Type != TimeLogType.PS)
        {
            _logger.LogWarning("Invalid previous state to start tracking for AccountId {AccountId}.", accountId);
            _ex.Message = "Invalid previous state to start tracking.";
            return new Result { Success = false };
        }

        if (lastLog != null)
        {
            lastLog.Activ = false;
            CalculateTotalHours(lastLog, DateTime.UtcNow);
            _context.TimeLogs.Update(lastLog);
        }

        var newLog = new TimeLog
        {
            Time = DateTime.UtcNow,
            Type = TimeLogType.PE,
            UserId = accountId
        };

        _context.TimeLogs.Add(newLog);
        _context.SaveChanges();
        _logger.LogInformation("Tracking started successfully for AccountId {AccountId}.", accountId);

        return new Result { Success = true };
    }

    public Result PauseTracking(Guid accountId)
    {
        var lastLog = GetLastLog(accountId);

        if (lastLog == null)
        {
            _logger.LogWarning("No active tracking found to pause for AccountId {AccountId}.", accountId);
            _ex.Message = "No active tracking found to pause.";
            return new Result { Success = false };
        }

        if (lastLog.Type == TimeLogType.PS)
        {
            _logger.LogWarning("Cannot pause after stopping the session for AccountId {AccountId}.", accountId);
            _ex.Message = "Cannot pause after stopping the session.";
            return new Result { Success = false };
        }

        if (lastLog.Type == TimeLogType.P)
        {
            _logger.LogWarning("Session is already paused for AccountId {AccountId}.", accountId);
            _ex.Message = "Session is alreaddy paused";
            return new Result { Success = false };
        }

        lastLog.Activ = false;
        CalculateTotalHours(lastLog, DateTime.UtcNow);
        _context.TimeLogs.Update(lastLog);

        var newLog = new TimeLog
        {
            Time = DateTime.UtcNow,
            Type = TimeLogType.P,
            UserId = accountId
        };

        _context.TimeLogs.Add(newLog);
        _context.SaveChanges();
        _logger.LogInformation("Tracking paused successfully for AccountId {AccountId}.", accountId);

        return new Result { Success = true };
    }
    

    public Result StopTracking(Guid accountId)
    {
        var lastLog = GetLastLog(accountId);

        if (lastLog == null || !lastLog.Activ)
        {
            _logger.LogWarning("No active tracking found to stop for AccountId {AccountId}.", accountId);
            _ex.Message = "No Active tracking found to stop";
            return new Result { Success = false };
        }

        if (lastLog.Type == TimeLogType.P)
        {
            _logger.LogWarning("Cannot stop while in paused state for AccountId {AccountId}.", accountId);
            _ex.Message = "Cannot stop while in paused state";
            return new Result { Success = false };
        }

        if (lastLog.Type == TimeLogType.PS)
        {
            _logger.LogWarning("Session is already stopped for AccountId {AccountId}.", accountId);
            _ex.Message = "Session is already stopped";
            return new Result { Success = false };
        }

        lastLog.Activ = false;
        CalculateTotalHours(lastLog, DateTime.UtcNow);
        _context.TimeLogs.Update(lastLog);

        var newLog = new TimeLog
        {
            Time = DateTime.UtcNow,
            Type = TimeLogType.PS,
            UserId = accountId
        };

        _context.TimeLogs.Add(newLog);
        _context.SaveChanges();
        _logger.LogInformation("Tracking stopped successfully for AccountId {AccountId}.", accountId);

        return new Result { Success = true };
    }

    public List<TimeLog> GetLogs(Guid accountId)
    {
        return _context.TimeLogs
                       .Where(log => log.UserId == accountId)
                       .OrderBy(log => log.Time)
                       .AsNoTracking()
                       .ToList();
    }



    public Result UpdateLog(Guid logId, DateTime newTime, TimeLogType newType, int? totalHours)
    {
        var log = _context.TimeLogs.FirstOrDefault(t => t.TM_Id == logId);

        if (log == null)
        {
            _logger.LogWarning("TimeLog not found for logId {LogId}.", logId);
            return new Result { Success = false };
        }

        log.Time = newTime;
        log.Type = newType;

        if (totalHours.HasValue)
        {
            log.TotalHours = TimeSpan.FromMinutes(totalHours.Value);
        }

        _context.SaveChanges();
        _logger.LogInformation("TimeLog updated successfully for logId {LogId}.", logId);

        return new Result { Success = true };
    }
}