using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs
{
    public class TimeLogUpdateDto
    {
        public DateTime NewTime { get; set; }
        public string NewType { get; set; }
        public int? TotalHours { get; set; } // Optional, only needed if updating total hours
    }
}
