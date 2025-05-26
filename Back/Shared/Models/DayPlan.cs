using Shared.Models.Enumerations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Models;

public class DayPlan
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public TimeOnly? StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }
    public TimeOnly? NumberOfHours { get; set; }
    public Days Day { get; set; }
    public Guid PlanId { get; set; }
    [ForeignKey(nameof(PlanId))]
    public virtual Planner Planner { get; set; } = null!;
}
