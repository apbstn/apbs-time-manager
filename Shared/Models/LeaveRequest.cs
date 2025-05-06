using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models
{
    public class LeaveRequest
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int Sequence { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [ForeignKey("EmployeeId")]
        public virtual Employee Employee { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [NotMapped]
        public int DaysRequested => (EndDate - StartDate).Days + 1;

        [Required]
        public LeaveStatus Status { get; set; }

        [Required, MaxLength(50)]
        public string LeaveType { get; set; }

        public string Reason { get; set; }
    }
}
