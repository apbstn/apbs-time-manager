using Shared.Models.Enumerations;

namespace Shared.DTOs.PlannerDtos.Create;

public class CreateFlexiblePlannerDto
{
    public PlannerType Type = PlannerType.Flexible;
    public CreateFlexibleDayPlannerDto Monday { get; set; }
    public CreateFlexibleDayPlannerDto Tuesday { get; set; }
    public CreateFlexibleDayPlannerDto Wendsday { get; set; }
    public CreateFlexibleDayPlannerDto Thursday { get; set; }
    public CreateFlexibleDayPlannerDto Friday { get; set; }
    public CreateFlexibleDayPlannerDto Saturday { get; set; }
    public CreateFlexibleDayPlannerDto Sunday { get; set; }
}
