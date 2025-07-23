using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using Shared.Context;
using Shared.Models;
using Shared.Services;

namespace Shared.Services
{
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
                var currentDate = DateTime.UtcNow.Date;

                var allLogs = _context.TimeLogs
                    .Where(log => log.UserId == accountId)
                    .OrderBy(log => log.Time)
                    .AsNoTracking()
                    .ToList();

                var todayLogs = allLogs
                    .Where(log => log.Time.Date == currentDate)
                    .ToList();

                if (todayLogs.Any())
                {
                    return todayLogs;
                }

                var yesterday = currentDate.AddDays(-1);
                var lastActiveLog = allLogs
                    .Where(log => log.Time.Date == yesterday && log.Activ)
                    .OrderByDescending(log => log.Time)
                    .FirstOrDefault();

                return lastActiveLog != null ? new List<TimeLog> { lastActiveLog } : new List<TimeLog>();
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

        public string GetTodayTotalHours(Guid id)
        {
            try
            {
                var currentDate = DateTime.UtcNow.Date; // 2025-07-23

                var totalHours = _context.TimeLogs
                    .Where(log => log.UserId == id && log.Time.Date == currentDate && log.Type == TimeLogType.PE)
                    .ToList()
                    .Sum(log => log.TotalHours?.TotalHours ?? 0);

                if (totalHours <= 0)
                    return null; // Will trigger NotFound in controller

                int hours = (int)totalHours;
                int minutes = (int)((totalHours - hours) * 60); // Convert decimal fraction to minutes
                return $"{hours}h {minutes:D2}m"; // Format as "0h 00m"
            }
            catch (Npgsql.PostgresException ex) when (ex.SqlState == "42P01")
            {
                _logger.LogInformation("Table TIMELOG does not exist for userId {UserId}. Returning null.", id);
                return null;
            }
        }

        public Dictionary<string, double> GetWeeklyHours(Guid id)
        {
            try
            {
                var currentDate = DateTime.UtcNow.Date; // 2025-07-23 00:00:00 UTC
                var startOfWeek = currentDate.AddDays(-(int)currentDate.DayOfWeek); // Start of week (Monday, July 21, 2025)

                var weeklyData = _context.TimeLogs
                    .Where(log => log.UserId == id && log.Time.Date >= startOfWeek && log.Type == TimeLogType.PE)
                    .ToList() // Load into memory
                    .GroupBy(log => log.Time.DayOfWeek)
                    .Select(g => new
                    {
                        Day = g.Key,
                        TotalHours = g.Sum(log => log.TotalHours?.TotalHours ?? 0) // Client-side sum
                    })
                    .ToDictionary(
                        x => Enum.GetName(typeof(DayOfWeek), x.Day) ?? "Unknown",
                        x => Math.Round(x.TotalHours, 2)
                    );

                // Ensure all days (Monday to Sunday) are included with 0 if no data
                var allDays = Enum.GetValues(typeof(DayOfWeek))
                    .Cast<DayOfWeek>()
                    .ToDictionary(d => d.ToString(), d => weeklyData.GetValueOrDefault(d.ToString(), 0.0));

                return allDays;
            }
            catch (Npgsql.PostgresException ex) when (ex.SqlState == "42P01")
            {
                _logger.LogInformation("Table TIMELOG does not exist for userId {UserId}. Returning empty data.", id);
                return new Dictionary<string, double>();
            }
        }
    }
}