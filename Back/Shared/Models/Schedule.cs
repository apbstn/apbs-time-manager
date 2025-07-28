using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models;

public class Schedule
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string WorkType { get; set; } = string.Empty;
    public string SelectedDays { get; set; } = string.Empty; // Stored as comma-separated string
    public string FlexibleHoursJson { get; set; } = string.Empty; // Stored as JSON
    public string FixedHoursJson { get; set; } = string.Empty; // Stored as JSON
    public int WeeklyHours { get; set; }
    public DateTime CreatedAt { get; set; }
}
