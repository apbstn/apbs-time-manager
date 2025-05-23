using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Shared.Models.Planners;

public class FlexibleDayPlanner
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public bool Active { get; set; }
    public TimeOnly? NumberOfHours { get; set; }
}
