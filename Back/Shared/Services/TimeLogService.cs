using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using Shared.Context;
using Shared.Models;
using Shared.Services;
using System;

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
                return new Result { Success = true };
            }
            catch (Npgsql.PostgresException ex) when (ex.SqlState == "42P01")
            {
                _logger.LogInformation("Table TIMELOG does not exist for AccountId {AccountId}. Ignoring and returning success.", accountId);
                return new Result { Success = true };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting tracking for AccountId {AccountId}.", accountId);
                return new Result { Success = false, Exception = new Exxception(ex.Message) };
            }
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
                return new Result { Success = true };
            }
            catch (Npgsql.PostgresException ex) when (ex.SqlState == "42P01")
            {
                _logger.LogInformation("Table TIMELOG does not exist for AccountId {AccountId}. Ignoring and returning success.", accountId);
                return new Result { Success = true };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error pausing tracking for AccountId {AccountId}.", accountId);
                return new Result { Success = false, Exception = new Exxception(ex.Message) };
            }
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
                return new Result { Success = true };
            }
            catch (Npgsql.PostgresException ex) when (ex.SqlState == "42P01")
            {
                _logger.LogInformation("Table TIMELOG does not exist for AccountId {AccountId}. Ignoring and returning success.", accountId);
                return new Result { Success = true };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error stopping tracking for AccountId {AccountId}.", accountId);
                return new Result { Success = false, Exception = new Exxception(ex.Message) };
            }
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
                RecalculateLogs(log.UserId, newTime);
                _logger.LogInformation("TimeLog updated successfully for logId {LogId}.", logId);

                return new Result { Success = true };
            }
            catch (Npgsql.PostgresException ex) when (ex.SqlState == "42P01")
            {
                _logger.LogInformation("Table TIMELOG does not exist for logId {LogId}. Ignoring and returning success.", logId);
                return new Result { Success = true };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating log for logId {LogId}.", logId);
                return new Result { Success = false, Exception = new Exxception(ex.Message) };
            }
        }

        public string GetTodayTotalHours(Guid id)
        {
            try
            {
                var currentDate = DateTime.UtcNow.Date;

                var totalHours = _context.TimeLogs
                    .Where(log => log.UserId == id && log.Time.Date == currentDate && log.Type == TimeLogType.PE)
                    .ToList()
                    .Sum(log => log.TotalHours?.TotalHours ?? 0);

                if (totalHours <= 0)
                    return null;

                int hours = (int)totalHours;
                int minutes = (int)((totalHours - hours) * 60);
                return $"{hours}h {minutes:D2}m";
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
                var currentDate = DateTime.UtcNow.Date;
                var startOfWeek = currentDate.AddDays(-(int)currentDate.DayOfWeek);

                var weeklyData = _context.TimeLogs
                    .Where(log => log.UserId == id && log.Time.Date >= startOfWeek && log.Type == TimeLogType.PE)
                    .ToList()
                    .GroupBy(log => log.Time.DayOfWeek)
                    .Select(g => new
                    {
                        Day = g.Key,
                        TotalHours = g.Sum(log => log.TotalHours?.TotalHours ?? 0)
                    })
                    .ToDictionary(
                        x => Enum.GetName(typeof(DayOfWeek), x.Day) ?? "Unknown",
                        x => Math.Round(x.TotalHours, 2)
                    );

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

        public Dictionary<string, double> GetWeeklyPause(Guid id)
        {
            try
            {
                var currentDate = DateTime.UtcNow.Date;
                var startOfWeek = currentDate.AddDays(-(int)currentDate.DayOfWeek);

                var weeklyData = _context.TimeLogs
                    .Where(log => log.UserId == id && log.Time.Date >= startOfWeek && log.Type == TimeLogType.P)
                    .ToList()
                    .GroupBy(log => log.Time.DayOfWeek)
                    .Select(g => new
                    {
                        Day = g.Key,
                        TotalHours = g.Sum(log => log.TotalHours?.TotalHours ?? 0)
                    })
                    .ToDictionary(
                        x => Enum.GetName(typeof(DayOfWeek), x.Day) ?? "Unknown",
                        x => Math.Round(x.TotalHours, 2)
                    );

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

        public string GetMonthlyTotalHours(Guid id)
        {
            try
            {
                var currentDate = DateTime.UtcNow;
                var startOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                var endOfMonth = startOfMonth.AddMonths(1).AddTicks(-1);

                var totalHours = _context.TimeLogs
                    .Where(log => log.UserId == id
                                && log.Time >= startOfMonth
                                && log.Time <= endOfMonth
                                && log.Type == TimeLogType.PE)
                    .Sum(log => log.TotalHours.HasValue ? log.TotalHours.Value.TotalHours : 0);

                int hours = (int)totalHours;
                int minutes = (int)((totalHours - hours) * 60);
                return $"{hours}h {minutes:D2}m";
            }
            catch (Npgsql.PostgresException ex) when (ex.SqlState == "42P01")
            {
                _logger.LogInformation("Table TIMELOG does not exist for userId {UserId}. Returning 0.", id);
                return "No Track were found";
            }
        }

        public Result ManualAdd(Guid accountId, DateTime time, TimeLogType type)
        {
            if (time > DateTime.UtcNow)
            {
                return new Result { Success = false, Exception = new Exxception("Cannot add log in the future.") };
            }

            try
            {
                // Validate log sequence
                var lastLog = GetLastLog(accountId);
                if (lastLog != null)
                {
                    if (lastLog.Type == TimeLogType.PE && type == TimeLogType.PE)
                    {
                        return new Result { Success = false, Exception = new Exxception("Cannot add consecutive start logs.") };
                    }
                    if (lastLog.Type == TimeLogType.PS && type != TimeLogType.PE)
                    {
                        return new Result { Success = false, Exception = new Exxception("Session is stopped. Only start log allowed.") };
                    }
                }

                var newLog = new TimeLog
                {
                    Time = time,
                    Type = type,
                    UserId = accountId,
                    Activ = true // Will be adjusted in recalculation
                };

                using (var transaction = _context.Database.BeginTransaction())
                {
                    _context.TimeLogs.Add(newLog);
                    _context.SaveChanges();
                    RecalculateLogs(accountId, time);
                    transaction.Commit();
                }

                _logger.LogInformation("Manual log added successfully for AccountId {AccountId}.", accountId);
                return new Result { Success = true };
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error adding manual log for AccountId {AccountId}. InnerException: {InnerException}, StackTrace: {StackTrace}",
                    accountId, ex.InnerException?.Message ?? "None", ex.StackTrace);
                return new Result { Success = false, Exception = new Exxception($"Database error: {ex.Message}, Inner: {ex.InnerException?.Message ?? "None"}") };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error adding manual log for AccountId {AccountId}. InnerException: {InnerException}, StackTrace: {StackTrace}",
                    accountId, ex.InnerException?.Message ?? "None", ex.StackTrace);
                return new Result { Success = false, Exception = new Exxception($"Unexpected error: {ex.Message}, Inner: {ex.InnerException?.Message ?? "None"}") };
            }
        }

        private void RecalculateLogs(Guid userId, DateTime time)
        {
            var logs = _context.TimeLogs
                .Where(l => l.UserId == userId && l.Time.Date == time.Date)
                .OrderBy(l => l.Time)
                .ToList();

            if (!logs.Any()) return;

            for (int i = 0; i < logs.Count - 1; i++)
            {
                logs[i].Activ = false;
                if (logs[i].Type == TimeLogType.PE && logs[i + 1].Type != TimeLogType.PE)
                {
                    logs[i].TotalHours = logs[i + 1].Time - logs[i].Time;
                }
                else
                {
                    logs[i].TotalHours = null;
                }
            }

            var lastLog = logs.Last();
            lastLog.Activ = lastLog.Type != TimeLogType.PS;
            lastLog.TotalHours = null;

            _context.TimeLogs.UpdateRange(logs);
            _context.SaveChanges();
        }
    }
}