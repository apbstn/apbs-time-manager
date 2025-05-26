using Shared.Models.Enumerations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Models;

public class Planner
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("P_ID")]
    public Guid Id { get; set; }
    [Required]
    [Column("P_Name")]
    public string PlannerName { get; set; }
    [Column("P_TYPE")]
    public PlannerType PlanType { get; set; }
    public bool Monday { get; set; }
    public bool Tuesday { get; set; }
    public bool Wendsday { get; set; }
    public bool Thursday { get; set; }
    public bool Friday { get; set; }
    public bool Saturday { get; set; }
    public bool Sunday { get; set; }
    public virtual ICollection<DayPlan> DayPlans { get; set; } = new List<DayPlan>();
    public TimeOnly TotalWeeklyHours { get; set; }

}
