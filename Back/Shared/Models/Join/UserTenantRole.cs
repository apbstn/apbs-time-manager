using Shared.Models.Enumerations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Models.Join;
[Table("JOIN_TENANT_USER")]
public class UserTenantRole
{
    [Key]
    [Column("J_ID")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Required]
    [Column("J_USER_ID")]
    public Guid UserId { get; set; }

    [Required]
    [Column("J_TENANT_ID")]
    public Guid TenantId { get; set; }

    [ForeignKey(nameof(TenantId))]
    public virtual Tenant? Tenant { get; set; }

    [ForeignKey(nameof(UserId))]
    public virtual User? User { get; set; }

    [Column("J_USER_TENANT_ROLE")]
    public RoleEnum Role { get; set; }
}
