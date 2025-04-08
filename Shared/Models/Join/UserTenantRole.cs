using Shared.Models.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models.Join;
[Table("JOIN_TENANT_USER")]
public class UserTenantRole
{
    [Key]
    [Column("J_ID")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; }
    [Column("J_TENANT_ID")]
    public Tenant Tenant { get; set; }
    [Column("J_USER_ID")]
    public User user { get; set; }
    [Column("J_USER_TENANT_ROLE")]
    public RoleEnum role { get; set; }
}
