using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.PlannerDtos.Response;

public class ResponseDayPlanDto
{
    public TimeOnly? StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }
    public TimeOnly? NumberOfHours { get; set; }
    public string Day { get; set; }
}
