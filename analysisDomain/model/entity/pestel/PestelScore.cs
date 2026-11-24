using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SemBackDotNet10.Base.Model.Entity;
using SemBackDotNet10.basicDomain.Model.Entity;

namespace SemBackDotNet10.analysisDomain.model.entity.pestel;

[Table("tblpestelscore", Schema = "analysis")]
public class PestelScore : AuditEntity<long>
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column("pestelscoreid")]
    public override long Id { get; set; }

    [Column("pestelid")]
    public long PestelId { get; set; }
    public Pestel Pestel { get; set; } = null!;

    [Column("planid")]
    public long? PlanId { get; set; }
    public PlanPeriod? PlanPeriod { get; set; }

    [Required, Column("opportunityorthreat")]
    public string OpportunityOrThreat { get; set; } = null!;

    [Column("currentscore")]
    public int CurrentScore { get; set; }
    [Column("futurescore")]
    public int FutureScore { get; set; }
    [Column("previousyearcurrentscore")]
    public int PreviousYearCurrentScore { get; set; }
    [Column("previousyearfuturescore")]
    public int PreviousYearFutureScore { get; set; }
}
