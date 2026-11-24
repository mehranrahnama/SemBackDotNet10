using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SemBackDotNet10.basicDomain.Model.Entity;

[Table("tblplaninglevelunit", Schema = "dbo")]
public class PlaningLevelUnit
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("planinglevelunitid")]
    public long Id { get; set; }

    [Required, Column("unittitle")]
    public string UnitTitle { get; set; } = string.Empty;

    [Column("Levelid")]
    public long? PlaningLevelId { get; set; }

    [ForeignKey(nameof(PlaningLevelId))]
    public virtual PlaningLevel? PlaningLevel { get; set; }

    [Column("unitcode")]
    public string? UnitCode { get; set; }
}
