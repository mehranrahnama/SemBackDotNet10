using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SemBackDotNet10.analysisDomain.model.entity.enums;
using SemBackDotNet10.Base.Model.Entity;

namespace SemBackDotNet10.analysisDomain.model.entity.stakeHolder;

[Table("tblstakeholder", Schema = "analysis")]
public class StakeHolder : AuditEntity<long>
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("stakeholderid")]
    public override long Id { get; set; }

    [Column("stakeholdergroupid")]
    public long? StakeHolderGroupId { get; set; }

    [ForeignKey(nameof(StakeHolderGroupId))]
    public StakeHolderGroup? StakeHolderGroup { get; set; }

    [Required]
    [Column("stakeholdertitle")]
    public string StakeHolderTitle { get; set; } = string.Empty;

    [Column("Power")]
    public double? Power { get; set; }

    [Column("Interest")]
    public double? Interest { get; set; }

    [Column("behaviorstrategy")]
    public BehaviorStrategy? BehaviorStrategy { get; set; }
}
