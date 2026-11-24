using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SemBackDotNet10.basicDomain.Model.Entity;

[Table("tblplaninglevelorg", Schema = "dbo")]
public class PlaningLevelOrganization : AuditEntity<long>
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("planlevelorgid")]
    public long Id { get; set; }

    [Column("levelid")]
    public long? LevelId { get; set; }

    [ForeignKey(nameof(LevelId))]
    public virtual PlaningLevel? Level { get; set; }

    [Column("organizationid")]
    public long? OrganizationId { get; set; }

    [ForeignKey(nameof(OrganizationId))]
    public virtual Organization? Organization { get; set; }

    [Column("dummy_field")]
    public bool? DummyField { get; set; }
}
