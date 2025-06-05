using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
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
        try
        {
            return _context.TimeLogs
                .Where(t => t.UserId == accountId)
                .OrderByDescending(t => t.Time)
                .FirstOrDefault();
        }
        catch (Npgsql.PostgresException ex) when (ex.SqlState == "42P01")
        {
            _logger.LogInformation("Table TIMELOG does not exist for AccountId {AccountId}. Returning null.", accountId);
            return null;
        }
    }

    public Result StartTracking(Guid accountId)
    {
        var lastLog = GetLastLog(accountId);

        // Ignore if already started (PE) or in invalid state; proceed to create new log
        if (lastLog?.Type == TimeLogType.PE)
        {
            _logger.LogInformation("Tracking already started for AccountId {AccountId}. Proceeding with new log.", accountId);
        }
        else if (lastLog != null && lastLog.Type != TimeLogType.P && lastLog.Type != TimeLogType.PS)
        {
            _logger.LogInformation("Invalid previous state for AccountId {AccountId}. Proceeding with new log.", accountId);
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

        try
        {
            _context.TimeLogs.Add(newLog);
            _context.SaveChanges();
            _logger.LogInformation("Tracking started successfully for AccountId {AccountId}.", accountId);
        }
        catch (Npgsql.PostgresException ex) when (ex.SqlState == "42P01")
        {
            _logger.LogInformation("Table TIMELOG does not exist for AccountId {AccountId}. Ignoring and returning success.", accountId);
        }

        return new Result { Success = true };
    }

    public Result PauseTracking(Guid accountId)
    {
        var lastLog = GetLastLog(accountId);

        // If no log exists, create a new pause log
        if (lastLog == null)
        {
            _logger.LogInformation("No active tracking found for AccountId {AccountId}. Creating new pause log.", accountId);
        }
        else if (lastLog.Type == TimeLogType.PS)
        {
            _logger.LogInformation("Session already stopped for AccountId {AccountId}. Ignoring pause request.", accountId);
            return new Result { Success = true };
        }
        else if (lastLog.Type == TimeLogType.P)
        {
            _logger.LogInformation("Session already paused for AccountId {AccountId}. Ignoring pause request.", accountId);
            return new Result { Success = true };
        }
        else
        {
            lastLog.Activ = false;
            CalculateTotalHours(lastLog, DateTime.UtcNow);
            _context.TimeLogs.Update(lastLog);
        }

        var newLog = new TimeLog
        {
            Time = DateTime.UtcNow,
            Type = TimeLogType.P,
            UserId = accountId
        };

        try
        {
            _context.TimeLogs.Add(newLog);
            _context.SaveChanges();
            _logger.LogInformation("Tracking paused successfully for AccountId {AccountId}.", accountId);
        }
        catch (Npgsql.PostgresException ex) when (ex.SqlState == "42P01")
        {
            _logger.LogInformation("Table TIMELOG does not exist for AccountId {AccountId}. Ignoring and returning success.", accountId);
        }

        return new Result { Success = true };
    }

    public Result StopTracking(Guid accountId)
    {
        var lastLog = GetLastLog(accountId);

        // If no log or not active, proceed or return success
        if (lastLog == null || !lastLog.Activ)
        {
            _logger.LogInformation("No active tracking found for AccountId {AccountId}. Creating stop log.", accountId);
        }
        else if (lastLog.Type == TimeLogType.P)
        {
            _logger.LogInformation("Session paused for AccountId {AccountId}. Proceeding to stop.", accountId);
        }
        else if (lastLog.Type == TimeLogType.PS)
        {
            _logger.LogInformation("Session already stopped for AccountId {AccountId}. Ignoring stop request.", accountId);
            return new Result { Success = true };
        }
        else
        {
            lastLog.Activ = false;
            CalculateTotalHours(lastLog, DateTime.UtcNow);
            _context.TimeLogs.Update(lastLog);
        }

        var newLog = new TimeLog
        {
            Time = DateTime.UtcNow,
            Type = TimeLogType.PS,
            UserId = accountId
        };

        try
        {
            _context.TimeLogs.Add(newLog);
            _context.SaveChanges();
            _logger.LogInformation("Tracking stopped successfully for AccountId {AccountId}.", accountId);
        }
        catch (Npgsql.PostgresException ex) when (ex.SqlState == "42P01")
        {
            _logger.LogInformation("Table TIMELOG does not exist for AccountId {AccountId}. Ignoring and returning success.", accountId);
        }

        return new Result { Success = true };
    }

    public List<TimeLog> GetLogs(Guid accountId)
    {
        try
        {
            // Get today's date in UTC
            var currentDate = DateTime.UtcNow.Date;

            // Get all logs for the user
            var allLogs = _context.TimeLogs
                .Where(log => log.UserId == accountId)
                .OrderBy(log => log.Time)
                .AsNoTracking()
                .ToList();

            // Check for logs from today
            var todayLogs = allLogs
                .Where(log => log.Time.Date == currentDate)
                .ToList();

            if (todayLogs.Any())
            {
                return todayLogs;
            }

            // Find last active log from yesterday
            var yesterday = currentDate.AddDays(-1);
            var lastActiveLog = allLogs
                .Where(log => log.Time.Date == yesterday && log.Activ)
                .OrderByDescending(log => log.Time)
                .FirstOrDefault();

            if (lastActiveLog != null)
            {
                return new List<TimeLog> { lastActiveLog };
            }

            return new List<TimeLog>();
        }
        catch (Npgsql.PostgresException ex) when (ex.SqlState == "42P01")
        {
            _logger.LogInformation("Table TIMELOG does not exist for AccountId {AccountId}. Returning empty list.", accountId);
            return new List<TimeLog>();
        }
    }

    public Result UpdateLog(Guid logId, DateTime newTime, TimeLogType newType, int? totalHours)
    {
        try
        {
            var log = _context.TimeLogs.FirstOrDefault(t => t.TM_Id == logId);

            if (log == null)
            {
                _logger.LogInformation("TimeLog not found for logId {LogId}. Ignoring update request.", logId);
                return new Result { Success = true };
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
        catch (Npgsql.PostgresException ex) when (ex.SqlState == "42P01")
        {
            _logger.LogInformation("Table TIMELOG does not exist for logId {LogId}. Ignoring and returning success.", logId);
            return new Result { Success = true };
        }
    }
}