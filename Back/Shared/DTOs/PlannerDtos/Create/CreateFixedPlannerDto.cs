using Shared.Models.Enumerations;

namespace Shared.DTOs.PlannerDtos.Create;

public class CreateFixedPlannerDto
{
    public PlannerType Type = PlannerType.Fixed;
    public CreateFixedDayPlannerDto Monday { get; set; }
    public CreateFixedDayPlannerDto Tuesday { get; set; }
    public CreateFixedDayPlannerDto Wendsday { get; set; }
    public CreateFixedDayPlannerDto Thursday { get; set; }
    public CreateFixedDayPlannerDto Friday { get; set; }
    public CreateFixedDayPlannerDto Saturday { get; set; }
    public CreateFixedDayPlannerDto Sunday { get; set; }
}
