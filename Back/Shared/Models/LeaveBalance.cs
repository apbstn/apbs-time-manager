using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Shared.Models;

[Table("LEAVEBALANCE")]
public class LeaveBalance
{
    [Key]
    [Column("LB_ID")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Column("LB_USER_ID")]
    public Guid UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public virtual UserTenant User { get; set; }

    [Column("LB_BALANCE")]
    [Precision(10, 2)]
    public decimal Balance { get; set; } // In days, supports partial days (e.g., 10.5)

    [Column("LB_LAST_UPDATED")]
    public DateTime LastUpdated { get; set; }

    [Column("LB_LAST_ALLOCATION_MONTH")]
    public DateTime LastAllocationMonth { get; set; }
}