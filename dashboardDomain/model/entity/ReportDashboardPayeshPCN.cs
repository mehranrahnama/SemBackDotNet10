using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SemBackDotNet10.dashboardDomain.model.entity;

[Keyless]
[Table("tblreportdashboardpayeshpcn")]
public class ReportDashboardPayeshPCN
{
    [Column("planid")] public long? PlanId { get; set; }
    [Column("month")] public long? Month { get; set; }
    [Column("currentdate")] public string? CurrentDate { get; set; }
    [Column("levelid")] public long? LevelId { get; set; }
    [Column("deslevel")] public string? DesLevel { get; set; }
    [Column("actionpayeshdcount")] public int? ActionPayeshdCount { get; set; }
    [Column("actionnotpayeshdcount")] public int? ActionNotPayeshdCount { get; set; }
    [Column("measurepayeshdcount")] public int? MeasurePayeshdCount { get; set; }
    [Column("measurenotpayeshdcount")] public int? MeasureNotPayeshdCount { get; set; }
    [Column("actionachievedcount")] public int? ActionAchievedCount { get; set; }
    [Column("actionnotachievedcount")] public int? ActionNotAchievedCount { get; set; }
    [Column("measureachievedcount")] public int? MeasureAchievedCount { get; set; }
    [Column("measurenotachievedcount")] public int? MeasureNotAchievedCount { get; set; }
}
