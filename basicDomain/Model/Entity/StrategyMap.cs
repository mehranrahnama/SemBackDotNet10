using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SemBackDotNet10.basicDomain.Model.Entity;
public class StrategyMap
{
    [Key, Column("strategymapid")] public long? Id { get; set; }
    [Column("perspectid")] public long? Perspectid { get; set; }
    [Column("desperspectf")] public string? Desperspectf { get; set; }
    [Column("parentid")] public long? ParentId { get; set; }
    [Column("strategyobjectname")] public string? StrategyObjectName { get; set; }
    [Column("strategyobjectpcn")] public decimal? StrategyObjectPcn { get; set; }
}
