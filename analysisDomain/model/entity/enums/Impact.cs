using System.Text.Json.Serialization;

namespace SemBackDotNet10.analysisDomain.model.entity.enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Impact
{
    LOW,
    MEDIUM,
    HIGH
}

public static class ImpactExtensions
{
    public static long GetId(this Impact value) => value switch
    {
        Impact.LOW => 1L,
        Impact.MEDIUM => 2L,
        Impact.HIGH => 3L,
        _ => throw new ArgumentOutOfRangeException(nameof(value))
    };

    public static string GetPersianName(this Impact value) => value switch
    {
        Impact.LOW => "کم",
        Impact.MEDIUM => "متوسط",
        Impact.HIGH => "زیاد",
        _ => throw new ArgumentOutOfRangeException(nameof(value))
    };

    public static string GetName(this Impact value) => value.ToString();
}
