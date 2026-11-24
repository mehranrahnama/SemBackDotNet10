using System.Text.Json.Serialization;

namespace SemBackDotNet10.analysisDomain.model.entity.enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Threat
{
    NoThreat,
    MidlleThreat,
    SeriousThreat
}

public static class ThreatExtensions
{
    public static long GetId(this Threat value) => value switch
    {
        Threat.NoThreat => 1L,
        Threat.MidlleThreat => 2L,
        Threat.SeriousThreat => 3L,
        _ => throw new ArgumentOutOfRangeException(nameof(value))
    };

    public static string GetPersianName(this Threat value) => value switch
    {
        Threat.NoThreat => "عم وجود تهدید",
        Threat.MidlleThreat => "تهدید متوسط",
        Threat.SeriousThreat => "تهدید جدی",
        _ => throw new ArgumentOutOfRangeException(nameof(value))
    };

    public static string GetName(this Threat value) => value.ToString();

    public static Threat FromId(long? id)
    {
        foreach (var threat in Enum.GetValues<Threat>())
        {
            if (threat.GetId() == id)
            {
                return threat;
            }
        }
        throw new ArgumentException("Invalid id: " + id);
    }
}
