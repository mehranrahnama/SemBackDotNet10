#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SemBackDotNet10.basicDomain.Model.Dto;

public class StrategyPillarDto
{
    public long? Id { get; set; }
    public long? PlanId { get; set; }
    public long? LevelId { get; set; }
    public string? Philosophy { get; set; }
    public string? Mission { get; set; }
    public string? Vision { get; set; }
    public string? CorporationValues { get; set; }
    public string? AbbreviationValue { get; set; }
    public string? BigObjects { get; set; }
}
