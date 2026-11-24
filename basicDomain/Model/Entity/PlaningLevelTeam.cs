using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SemBackDotNet10.Base.Model.Entity;
using SemBackDotNet10.SecurityDomain.Model.Entity;

namespace SemBackDotNet10.basicDomain.Model.Entity;

[Table("tblplaninglevelteam", Schema = "dbo")]
public class PlaningLevelTeam : AuditEntity<long>
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("planlevelteamid")]
    public long Id { get; set; }

    [Column("levelid")]
    public long? LevelId { get; set; }

    [ForeignKey(nameof(LevelId))]
    public virtual PlaningLevel? Level { get; set; }

    [Column("userid")]
    public long? UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public virtual User? User { get; set; }
}
