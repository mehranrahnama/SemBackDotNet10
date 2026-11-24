using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SemBackDotNet10.analysisDomain.model.entity.enums;
using SemBackDotNet10.Base.Model.Entity;

namespace SemBackDotNet10.analysisDomain.model.entity.stakeHolder;

[Table("tblstakeholderneedspriority", Schema = "analysis")]
public class StakeholderNeedsPriority : AuditEntity<long>
{
    public StakeholderNeedsPriority()
    {
    }

    public StakeholderNeedsPriority(long id, string needTitle, long importance, long impact, StakeHolder stakeHolder)
    {
        Id = id;
        NeedTitle = needTitle;
        Importance = importance;
        Impact = impact;
        StakeHolder = stakeHolder;
        StakeHolderId = stakeHolder.Id;
    }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("stakeholderneedspriorityid")]
    public override long Id { get; set; }

    [Required]
    [Column("needtitle")]
    public string NeedTitle { get; set; } = string.Empty;

    [Column("importance")]
    public long Importance { get; set; }

    [Column("impact")]
    public long Impact { get; set; }

    [Column("stakeholderid")]
    public long StakeHolderId { get; set; }

    [ForeignKey(nameof(StakeHolderId))]
    public StakeHolder StakeHolder { get; set; } = null!;

    [Column("answeringtype")]
    public AnsweringType? AnsweringType { get; set; }
}
