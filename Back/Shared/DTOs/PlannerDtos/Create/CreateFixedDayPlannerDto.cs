namespace Shared.DTOs.PlannerDtos.Create;

public class CreateFixedDayPlannerDto
{
    public bool Active { get; set; }
    public TimeOnly? StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }
}
