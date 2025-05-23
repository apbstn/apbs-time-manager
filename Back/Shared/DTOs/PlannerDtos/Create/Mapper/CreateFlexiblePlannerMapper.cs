using Riok.Mapperly.Abstractions;
using Shared.Models.Planners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.PlannerDtos.Create.Mapper;
[Mapper]
public partial class CreateFlexiblePlannerMapper
{
    public partial FlexiblePlanner ToEntity(CreateFlexiblePlannerDto dto);
    public partial FlexibleDayPlanner ToEntity(CreateFlexibleDayPlannerDto dto);
}
