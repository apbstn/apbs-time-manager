using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.PlannerDtos.Create;

public class CreateFlexibleDayPlannerDto
{
    public bool Active { get; set; }
    public TimeOnly? NumberOfHours { get; set; }
}
