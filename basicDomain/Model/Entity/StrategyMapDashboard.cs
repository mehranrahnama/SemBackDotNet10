using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SemBackDotNet10.basicDomain.Model.Entity;
public class StrategyMapDashboard
{
    [Key, Column("strategymapdashboardid")] public long? Id { get; set; }
    [Column("perspectdes")] public string? Perspectdes { get; set; }
    [Column("strategyobjectcounteachperspective")] public long? Strategyobjectcounteachperspective { get; set; }
    [Column("summeasureweightfromeachperspective")] public decimal? Summeasureweightfromeachperspective { get; set; }
    [Column("summeasureweighttoeachperspective")] public decimal? Summeasureweighttoeachperspective { get; set; }
    [Column("summeasureweightkoleachperspective")] public decimal? Summeasureweightkoleachperspective { get; set; }
    [Column("pcneachperspective")] public decimal? Pcneachperspective { get; set; }
    [Column("pcnfrom")] public decimal? Pcnfrom { get; set; }
    [Column("pcnto")] public decimal? Pcnto { get; set; }
    [Column("pcnkol")] public decimal? Pcnkol { get; set; }
    [Column("strategyobjectcountkol")] public long? Strategyobjectcountkol { get; set; }
    [Column("pcnobjectscountbellow70")] public long? Pcnobjectscountbellow70 { get; set; }
    [Column("pcnobjectscountbetween70to90")] public long? Pcnobjectscountbetween70to90 { get; set; }
    [Column("pcnobjectscountbetween90to110")] public long? Pcnobjectscountbetween90to110 { get; set; }
    [Column("pcnobjectscountmorethan110")] public long? Pcnobjectscountmorethan110 { get; set; }
    [Column("pcnobjectscountnottimepayesh")] public long? Pcnobjectscountnottimepayesh { get; set; }
}
