using System.Text.Json.Serialization;

namespace SemBackDotNet10.analysisDomain.model.entity.enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum BehaviorStrategy
{
    EffectiveManagement,
    Monitoring,
    Informing,
    AchievingSatisfaction
}

public static class BehaviorStrategyExtensions
{
    public static long GetId(this BehaviorStrategy value) => value switch
    {
        BehaviorStrategy.EffectiveManagement => 1L,
        BehaviorStrategy.Monitoring => 2L,
        BehaviorStrategy.Informing => 3L,
        BehaviorStrategy.AchievingSatisfaction => 4L,
        _ => throw new ArgumentOutOfRangeException(nameof(value))
    };

    public static string GetPersianName(this BehaviorStrategy value) => value switch
    {
        BehaviorStrategy.EffectiveManagement => "مدیریت موثر",
        BehaviorStrategy.Monitoring => "پایش/مراقبت",
        BehaviorStrategy.Informing => "اطلاع رسانی",
        BehaviorStrategy.AchievingSatisfaction => "کسب رضایت",
        _ => throw new ArgumentOutOfRangeException(nameof(value))
    };

    public static string GetName(this BehaviorStrategy value) => value.ToString();
}
