using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.PlannerDtos;

public record UpdateScheduleDto(
    string Name,
    string WorkType,
    string[] SelectedDays,
    Dictionary<string, FlexibleHoursDto> FlexibleHours,
    Dictionary<string, FixedHoursDto> FixedHours,
    int WeeklyHours);
