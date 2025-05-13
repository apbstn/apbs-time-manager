using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Shared.Models.Enumerations;
using Microsoft.EntityFrameworkCore;
using Shared.Models.Join;

namespace Shared.Models;

[Table("LEAVEREQUEST")]
public class LeaveRequest
{
    [Key]
    [Column("L_ID")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }


    [Column("L_USER_ID")]
    public Guid UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public virtual UserTenant User { get; set; }

    [Required]
    [Column("L_START")]
    public DateTime StartDate { get; set; }

    [Required]
    [Column("L_END")]
    public DateTime EndDate { get; set; }

    [Required]
    [Column("L_NUMBEROFDAYS")]
    public int NumberOfDays
    {
        get => (EndDate - StartDate).Days + 1;
        set { }
    }

    [Required]
    [Column("L_STATUS")]
    public required LeaveRequestStatus Status { get; set; } = LeaveRequestStatus.Pending;


    [Required]
    [Column("L_TYPE")]
    public string? Type { get; set; } = string.Empty;
    [Required]
    [Column("L_REASON")]
    public string? Reason { get; set; } = string.Empty;
}
