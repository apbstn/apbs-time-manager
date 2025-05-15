using System;
using System.Collections.Generic;
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
    }
}