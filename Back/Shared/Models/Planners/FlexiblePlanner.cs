using Shared.Models.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models.Planners;

public class FlexiblePlanner : PlannerBase
{
    [Column("P_MONDAY_ID")]
    public Guid MondayId { get; set; }
    [ForeignKey(nameof(MondayId))]
    public virtual FlexibleDayPlanner Monday { get; set; } = new();
    [Column("P_TUESDAY_ID")]
    public Guid TuesdayId { get; set; }
    [ForeignKey(nameof(TuesdayId))]
    public virtual FlexibleDayPlanner Tuesday { get; set; } = new();
    [Column("P_WENDSDAY_ID")]
    public Guid WendsdayId { get; set; }
    [ForeignKey(nameof(WendsdayId))]
    public virtual FlexibleDayPlanner Wendsday { get; set; } = new();
    [Column("P_THURSDAY_ID")]
    public Guid ThursdayId { get; set; }
    [ForeignKey(nameof(ThursdayId))]
    public virtual FlexibleDayPlanner Thursday { get; set; } = new();
    [Column("P_FRIDAY_ID")]
    public Guid FridayId { get; set; }
    [ForeignKey(nameof(FridayId))]
    public virtual FlexibleDayPlanner Friday { get; set; } = new();
    [Column("P_SATURDAY_ID")]
    public Guid SaturdayId { get; set; }
    [ForeignKey(nameof(SaturdayId))]
    public virtual FlexibleDayPlanner Saturday { get; set; } = new();
    [Column("P_SUNDAY_ID")]
    public Guid SundayId { get; set; }
    [ForeignKey(nameof(SundayId))]
    public virtual FlexibleDayPlanner Sunday { get; set; } = new();

    public FlexiblePlanner()
    {
        PlanType = PlannerType.Flexible;
    }
}
