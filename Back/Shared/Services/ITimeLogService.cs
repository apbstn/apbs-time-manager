using Shared.Models;
using Shared.Services;

namespace Shared.Services
{
    public interface ITimeLogService
    {
        Result StartTracking(Guid accountId);
        Result PauseTracking(Guid accountId);
        Result StopTracking(Guid accountId);
        List<TimeLog> GetLogs(Guid accountId); // Fixed return type
        Result UpdateLog(Guid logId, DateTime newTime, TimeLogType newType, int? totalHours = null);
        string GetTodayTotalHours(Guid id); // Changed to string
        Dictionary<string, double> GetWeeklyHours(Guid id);
        Dictionary<string, double> GetWeeklyPause(Guid id);
        double GetMonthlyTotalHours(Guid id);
    }
}