using Microsoft.EntityFrameworkCore;
using NPOI.SS.Formula.Functions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Models;

[Table("Invitation")]
public class Invitation
{
    [Key]
    [Column("I_ID")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    [Column("I_USER_ID")]
    public Guid? UserId { get; set; }
    [Required]
    [Column("I_Email")]
    public string Email;
    [Column("I_PHONE_NUMBER")]
    public string? Phone_Number;
    [Required]
    [Column("I_TENANT_ID")]
    public Guid TenantId { get; set; }

    [ForeignKey(nameof(TenantId))]
    public virtual Tenant? Tenant { get; set; }
    [Required]
    [Column("I_TOKEN")]
    public string Token { get; set; }
    [Column("I_CREATED_AT")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [Column("I_EXPIRES_AT")]
    public DateTime ExpiresAt { get; set; }
    [Column("I_IS_USED")]
    public bool IsUsed { get; set; } = false;
    [Column("I_USER_EXISTS")]
    public bool UserExists;
}
