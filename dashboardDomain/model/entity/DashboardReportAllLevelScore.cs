using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SemBackDotNet10.dashboardDomain.model.entity;

[Keyless]
[Table("tblreport_alllevelscore_byaction_hosco")]
public class DashboardReportAllLevelScore
{
    [Column("planid")] public long? PlanId { get; set; }
    [Column("month")] public long? Month { get; set; }
    [Column("fetchtype")] public long? FetchType { get; set; }
    [Column("levelid")] public long? LevelId { get; set; }
    [Column("organizationid")] public long? OrganizationId { get; set; }
    [Column("deslevel")] public string? DesLevel { get; set; }
    [Column("desorganization")] public string? DesOrganization { get; set; }
    [Column("scoreactualself")] public decimal? ScoreActualSelf { get; set; }
    [Column("scoreeval")] public decimal? ScoreEval { get; set; }
    [Column("objectcountgreen")] public int? ObjectCountGreen { get; set; }
    [Column("objectcountyellow")] public int? ObjectCountYellow { get; set; }
    [Column("objectcountred")] public int? ObjectCountRed { get; set; }
    [Column("objectcountgray")] public int? ObjectCountGray { get; set; }
}
