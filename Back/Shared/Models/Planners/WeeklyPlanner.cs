using Shared.Models.Enumerations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Models.Planners;

public class WeeklyPlanner : PlannerBase
{
    [Column("P_WEEKLY_HOURS")]
    public TimeOnly WeeklyHours { get; set; }

    public WeeklyPlanner()
    {
        PlanType = PlannerType.Weekly;
    }
}
