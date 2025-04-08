using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Shared.Models.Enumerations;
using Microsoft.EntityFrameworkCore;
using Shared.Models.Join;

namespace Shared.Models
{
    [Table("ACCOUNT")]
    [Index(nameof(Email), IsUnique = true)]
    public class User
    {
        [Key]
        [Column("A_ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        [Column("A_EMAIL")]
        public string? Email { get; set; }
        [Column("A_USERNAME")]
        public string? Username { get; set; }
        [Column("A_PASSWORD_HASH")]
        public string? PasswordHash { get; set; }
        [Column("A_TENANT_ROLE")]
        public IEnumerable<UserTenantRole> Roles { get; set; } = [];
        [Column("A_SEED")]
        public string? Seed { get; set; }
        [Column("A_ACTIVE")]
        public bool Active { get; set; } = false;
        [Column("A_REGISTRED")]
        public bool Registred { get; set; } = false;
        [Column("A_PHONE_NUMBER")]
        public string? PhoneNumber { get; set; }
        public bool IsAdmin { get; set; } = false;
    }
}
