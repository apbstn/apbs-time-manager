using Riok.Mapperly.Abstractions;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.PlannerDtos.Create.Mapper;

[Mapper]
public partial class CreatePlannerMapper
{
    public partial Planner ToEntity(CreatePlannerDto dto);
    public partial DayPlan ToEntity(CreateDayPlanDto dto);
}

