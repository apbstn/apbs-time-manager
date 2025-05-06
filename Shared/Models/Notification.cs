using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string TargetRole { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public int? EmployeeId { get; set; }
    }
}
