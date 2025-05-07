using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
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
    public Guid Id { get; set; }

    [Column("A_EMAIL")]
    public string? Email { get; set; }
    [Column("A_USERNAME")]
    public string? Username { get; set; }
    [Column("A_ROLE")]
    public RoleEnum? Role { get; set; }
    [Column("A_PHONE_NUMBER")]
    public string? PhoneNumber { get; set; }
}
