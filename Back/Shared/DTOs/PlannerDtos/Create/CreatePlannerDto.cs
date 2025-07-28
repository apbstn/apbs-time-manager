
using Shared.Models.Enumerations;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.PlannerDtos.Create;

public class CreatePlannerDto
{
    public string PlannerName { get; set; }
    public string PlanType { get; set; }
    public bool Monday { get; set; }
    public bool Tuesday { get; set; }
    public bool Wednesday { get; set; }
    public bool Thursday { get; set; }
    public bool Friday { get; set; }
    public bool Saturday { get; set; }
    public bool Sunday { get; set; }
    public ICollection<CreateDayPlanDto> DayPlans { get; set; } = new List<CreateDayPlanDto>();
    public TimeOnly? TotalWeeklyHours { get; set; }
}
