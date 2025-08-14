using Shared.Models;
using Shared.Services;

namespace Shared.Services
{
    public interface ITimeLogService
    {
        Result StartTracking(Guid accountId);
        Result PauseTracking(Guid accountId);
        Result StopTracking(Guid accountId);
        List<TimeLog> GetLogs(Guid accountId);
        Result UpdateLog(Guid logId, DateTime newTime, TimeLogType newType, int? totalHours = null);
        string GetTodayTotalHours(Guid id);
        Dictionary<string, double> GetWeeklyHours(Guid id);
        Dictionary<string, double> GetWeeklyPause(Guid id);
        string GetMonthlyTotalHours(Guid id);
        Result ManualAdd(Guid accountId, DateTime time, TimeLogType type);
    }
}