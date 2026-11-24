#nullable enable
using System.Text.Json.Serialization;

namespace SemBackDotNet10.basicDomain.Model.Dto;

[JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
public class StrategyObjectDto
{
    public long? Id { get; set; }
    public long? LevelId { get; set; }
    public int? MainStrategyId { get; set; }
    public string? Code { get; set; }
    public string? Title { get; set; }
    [JsonConverter(typeof(EmptyStringNullableDoubleConverter))]
    public double? Weight { get; set; }
    public int? ThemeId { get; set; }
    public long? PlanId { get; set; }
    public long? PerspectId { get; set; }
    public int? OriginId { get; set; }
    public string? PerspectDes { get; set; }
    public double PercentageOfRealization { get; set; }

    public StrategyObjectDto() { }

    public StrategyObjectDto(long? id, long? levelId, int? mainStrategyId, string? code, string? title, double? weight, int? themeId, long? planId, int? originId, long? perspectId, string? perspectDes)
    {
        Id = id;
        LevelId = levelId;
        MainStrategyId = mainStrategyId;
        Code = code;
        Title = title;
        Weight = weight;
        ThemeId = themeId;
        PlanId = planId;
        OriginId = originId;
        PerspectId = perspectId;
        PerspectDes = perspectDes;
    }

    public StrategyObjectDto(long? id, string? title)
    {
        Id = id;
        Title = title;
    }

    public StrategyObjectDto(long? id, string? title, long? levelId)
    {
        Id = id;
        Title = title;
        LevelId = levelId;
    }
}
