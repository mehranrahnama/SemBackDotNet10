using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SemBackDotNet10.dashboardDomain.model.entity;

[Keyless]
[Table("tblstrategymapevalbymeasurehoscodto")]
public class StrategyMapEvalByMeasureHoscoDto
{
    [Column("deslevel")] public string? DesLevel { get; set; }
    [Column("strategymapid")] public int? StrategyMapId { get; set; }
    [Column("perspectid")] public int? PerspectId { get; set; }
    [Column("parentid")] public int? ParentId { get; set; }
    [Column("desperspectf")] public string? DesPerspectF { get; set; }
    [Column("strategyobjectname")] public string? StrategyObjectName { get; set; }
    [Column("weight")] public decimal? Weight { get; set; }
    [Column("weightaverage")] public decimal? WeightAverage { get; set; }
    [Column("summeasureactualselfobject")] public decimal? SumMeasureActualSelfObject { get; set; }
    [Column("summeasureevalselfobject")] public decimal? SumMeasureEvalSelfObject { get; set; }
}
