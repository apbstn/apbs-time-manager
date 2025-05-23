using Riok.Mapperly.Abstractions;
using Shared.Models.Planners;

namespace Shared.DTOs.PlannerDtos.Create.Mapper;

[Mapper]
public partial class CreateFixedPlannerMapper
{
    public partial FixedPlanner ToEntity(CreateFixedPlannerDto dto);
    public partial FixedDayPlanner ToEntity(CreateFixedDayPlannerDto dto);
}
