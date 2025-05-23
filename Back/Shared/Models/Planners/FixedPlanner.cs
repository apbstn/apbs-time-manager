
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Shared.Models.Enumerations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Models.Planners;

public class FixedPlanner : PlannerBase
{
    [Column("P_MONDAY_ID")]
    public Guid MondayId { get; set; }
    [ForeignKey(nameof(MondayId))]
    public virtual FixedDayPlanner Monday { get; set; } = new();
    [Column("P_TUESDAY_ID")]
    public Guid TuesdayId { get; set; }
    [ForeignKey(nameof(TuesdayId))]
    public virtual FixedDayPlanner Tuesday { get; set; } = new();
    [Column("P_WENDSDAY_ID")]
    public Guid WendsdayId { get; set; }
    [ForeignKey(nameof(WendsdayId))]
    public virtual FixedDayPlanner Wendsday { get; set; } = new();
    [Column("P_THURSDAY_ID")]
    public Guid ThursdayId { get; set; }
    [ForeignKey(nameof(ThursdayId))]
    public virtual FixedDayPlanner Thursday { get; set; } = new();
    [Column("P_FRIDAY_ID")]
    public Guid FridayId { get; set; }
    [ForeignKey(nameof(FridayId))]
    public virtual FixedDayPlanner Friday { get; set; } = new();
    [Column("P_SATURDAY_ID")]
    public Guid SaturdayId { get; set; }
    [ForeignKey(nameof(SaturdayId))]
    public virtual FixedDayPlanner Saturday { get; set; } = new();
    [Column("P_SUNDAY_ID")]
    public Guid SundayId { get; set; }
    [ForeignKey(nameof(SundayId))]
    public virtual FixedDayPlanner Sunday { get; set; } = new();

    public FixedPlanner()
    {
        PlanType = PlannerType.Fixed;
    }
}
