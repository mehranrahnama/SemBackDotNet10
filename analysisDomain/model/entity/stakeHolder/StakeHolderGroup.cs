using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SemBackDotNet10.Base.Model.Entity;
using SemBackDotNet10.basicDomain.Model.Entity;

namespace SemBackDotNet10.analysisDomain.model.entity.stakeHolder;

[Table("tblstakeholdergroup", Schema = "analysis")]
public class StakeHolderGroup : AuditEntity<long>
{
    public StakeHolderGroup()
    {
    }

    public StakeHolderGroup(string stakeHolderGroupTitle)
    {
        StakeHolderGroupTitle = stakeHolderGroupTitle;
    }

    public StakeHolderGroup(string stakeHolderGroupTitle, PlanPeriod? planPeriod)
    {
        StakeHolderGroupTitle = stakeHolderGroupTitle;
        PlanPeriod = planPeriod;
    }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("stakeholdergroupid")]
    public override long Id { get; set; }

    [Required]
    [Column("stakeholdergrouptitle")]
    public string StakeHolderGroupTitle { get; set; } = string.Empty;

    [Column("planid")]
    public long? PlanId { get; set; }

    [ForeignKey(nameof(PlanId))]
    public PlanPeriod? PlanPeriod { get; set; }
}
