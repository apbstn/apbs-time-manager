using System;
using System.Collections.Generic;
using Shared.Models;
using Shared.Services;

namespace Shared.Services
{
    public interface ITimeLogService
    {
        Result StartTracking(int accountId);
        Result PauseTracking(int accountId);
        Result StopTracking(int accountId);
        List<TimeLog> GetLogs(int accountId);
        Result UpdateLog(Guid logId, DateTime newTime, TimeLogType newType, int? totalHours = null);
    }
}