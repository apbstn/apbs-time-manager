using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.Leave
{
    public class LeaveRequestDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int NumberOfDays { get; set; }
        public LeaveRequestStatus Status { get; set; }
        public string Type { get; set; }
        public string Reason { get; set; }
    }
}
