using Microsoft.EntityFrameworkCore;
using Shared.Services;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Shared.Models;
[Table("TIMELOG")]
public class TimeLog
{
    [Key]
    [Column("TM_ID")]
    public Guid TM_Id { get; set; } = Guid.NewGuid();

    [Column("TM_USER_ID")]
    public Guid UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public virtual UserTenant User { get; set; } 

    [Required]
    [Column("TM_TIME")]
    public DateTime Time { get; set; }

    [Required]
    [Column("TM_TYPE")]
    public TimeLogType Type { get; set; }

    [Required]
    [Column("TM_ACTIV")]
    public bool Activ { get; set; } = true;

    [Column("TM_TOTALHOURS")]
    public TimeSpan? TotalHours { get; set; } // Null until updated
}
