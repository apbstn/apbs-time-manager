using Riok.Mapperly.Abstractions;
using Shared.Models;

namespace Shared.DTOs.PlannerDtos.Response.Mapper;
[Mapper]
public partial class ResponsePlannerMapper
{
    public partial ResponsePlannerDto ToDto(Planner planner);
    public partial ResponseDayPlanDto ToDto(DayPlan dayPlan);
}
