using Shared.Models.Enumerations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Models;

public abstract class PlannerBase
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("P_ID")]
    public Guid Id { get; set; }
    [Required]
    [Column("P_Name")]
    public string PlannerName { get; set; }
    [Column("P_TYPE")]
    public PlannerType PlanType { get; protected set; }
}
