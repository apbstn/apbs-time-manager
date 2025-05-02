using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Models;
[Table("Tenant")]
[Index(nameof(Code), IsUnique = true)]
public class Tenant
{
    [Key]
    [Column("T_ID")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    [Column("T_CODE")]
    public string? Code { get; set; }
    [Column("T_NAME")]
    public string? Name { get; set; }
    [Column("T_CONNECTION_STRING")]
    public string? ConnectionString { get; set; }
    [Required]
    [Column("J_USER_ID")]
    public Guid UserId { get; set; }
    [ForeignKey(nameof(UserId))]
    public virtual User? Owner { get; set; }
}
