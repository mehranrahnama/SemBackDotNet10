using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SemBackDotNet10.basicDomain.Model.Entity;
public class ReportActionDataByStrategyObjectId
{
    [Key, Column("actiondatabystrategyobjectid")] public long? Id { get; set; }
    [Column("deslevel")] public string? Deslevel { get; set; }
    [Column("actionmasool")] public string? Actionmasool { get; set; }
    [Column("actionname")] public string? Actionname { get; set; }
    [Column("month")] public long? Month { get; set; }
    [Column("planvalue")] public decimal? Planvalue { get; set; }
    [Column("actualvalue")] public decimal? Actualvalue { get; set; }
    [Column("weight")] public decimal? Weight { get; set; }
    [Column("achievedpcnnormal")] public decimal? Achievedpcnnormal { get; set; }
}
