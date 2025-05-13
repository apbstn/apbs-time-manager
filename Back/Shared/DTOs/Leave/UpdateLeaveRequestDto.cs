using Shared.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs.Leave
{
    public class UpdateLeaveRequestDto
    {
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        public LeaveRequestStatus? Status { get; set; }

        [StringLength(50)]
        public string? Type { get; set; }

        [StringLength(500)]
        public string? Reason { get; set; }

        // Read-only property for calculated days
        public int? NumberOfDays
        {
            get
            {
                if (StartDate.HasValue && EndDate.HasValue)
                {
                    return (EndDate.Value - StartDate.Value).Days + 1;
                }
                return null;
            }
            // Private setter or remove entirely since it's calculated
            private set { }
        }
    }
}