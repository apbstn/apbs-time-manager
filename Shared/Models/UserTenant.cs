using Shared.Models.Join;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shared.Models.Enumerations;

namespace Shared.Models;

[Table("ACCOUNT")]
[Index(nameof(Email), IsUnique = true)]
public class UserTenant
{
    [Key]
    [Column("A_ID")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; }

    [Column("A_EMAIL")]
    public string? Email { get; set; }
    [Column("A_USERNAME")]
    public string? Username { get; set; }
    [Column("A_ROLE")]
    public RoleEnum? Role { get; set; }
    [Column("A_PHONE_NUMBER")]
    public string? PhoneNumber { get; set; }
}
