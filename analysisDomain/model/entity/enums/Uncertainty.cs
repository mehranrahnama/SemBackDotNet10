using System.Text.Json.Serialization;

namespace SemBackDotNet10.analysisDomain.model.entity.enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Uncertainty
{
    LOW,
    MEDIUM,
    HIGH
}

public static class UncertaintyExtensions
{
    public static long GetId(this Uncertainty value) => value switch
    {
        Uncertainty.LOW => 1L,
        Uncertainty.MEDIUM => 2L,
        Uncertainty.HIGH => 3L,
        _ => throw new ArgumentOutOfRangeException(nameof(value))
    };

    public static string GetPersianName(this Uncertainty value) => value switch
    {
        Uncertainty.LOW => "کم",
        Uncertainty.MEDIUM => "متوسط",
        Uncertainty.HIGH => "زیاد",
        _ => throw new ArgumentOutOfRangeException(nameof(value))
    };

    public static string GetName(this Uncertainty value) => value.ToString();
}
