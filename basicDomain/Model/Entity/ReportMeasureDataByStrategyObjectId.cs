using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SemBackDotNet10.basicDomain.Model.Entity;
public class ReportMeasureDataByStrategyObjectId
{
    [Key, Column("measuredatabystrategyobjectid")] public long? Id { get; set; }
    [Column("deslevel")] public string? Deslevel { get; set; }
    [Column("measuremasool")] public string? Measuremasool { get; set; }
    [Column("measurename")] public string? Measurename { get; set; }
    [Column("desiredmethod")] public string? Desiredmethod { get; set; }
    [Column("measurepayeshtype")] public string? Measurepayeshtype { get; set; }
    [Column("measureperiod")] public string? Measureperiod { get; set; }
    [Column("month")] public long? Month { get; set; }
    [Column("planvalue")] public decimal? Planvalue { get; set; }
    [Column("actualvalue")] public decimal? Actualvalue { get; set; }
    [Column("achievedpcnnormalnew")] public decimal? Achievedpcnnormalnew { get; set; }
    [Column("weight")] public decimal? Weight { get; set; }
}
