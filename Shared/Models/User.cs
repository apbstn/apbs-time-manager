using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Shared.Models.Enumerations;

namespace Shared.Models
{
    [Table("ACCOUNT")]
    public class User
    {
        [Key]
        [Column("A_ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("A_EMAIL")]
        public string? Email { get; set; }
        [Column("A_USERNAME")]
        public string? Username { get; set; }
        [Column("A_PASSWORD_HASH")]
        public string? PasswordHash { get; set; }
        [Column("A_TENANT_ID")]
        public string? TenantId { get; set; }
        [Column("A_ROLE")]
        public RoleEnum Role { get; set; }
        [Column("A_SEED")]
        public string? Seed { get; set; }
        [Column("A_ACTIVE")]
        public bool Active { get; set; } = false;
        [Column("A_REGISTRED")]
        public bool Registred { get; set; } = false;
    }
}
