#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SemBackDotNet10.basicDomain.Model.Dto;

public class StrategyPillarsMeasurePageDto
{
    public long? StrategyPillarsId { get; set; }
    public string? MeasureName { get; set; }
    public long? MeasureUnitId { get; set; }
    public string? MeasureUnitName { get; set; }
    public bool IsDeleted { get; set; }
    public List<long> Ids { get; set; } = new();
    public List<long> Year { get; set; } = new();
    public List<string?> MeasurePlan { get; set; } = new();
}
