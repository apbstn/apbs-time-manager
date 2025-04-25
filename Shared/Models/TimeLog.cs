using Shared.Services;

namespace Shared.Models
{
    public class TimeLog
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime Time { get; set; }
        public TimeLogType Type { get; set; } // Use the enum directly
        public bool Activ { get; set; } = true; // 1 (true) if active, 0 (false) if another row is added
        public int AccountId { get; set; }
        public TimeSpan? TotalHours { get; set; } // Null until updated
    }
}
