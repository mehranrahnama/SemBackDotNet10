using System.Text.Json.Serialization;

namespace SemBackDotNet10.analysisDomain.model.entity.enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AnsweringType
{
    BusinessLevel,
    TaskLevel,
    CurrentActivity
}

public static class AnsweringTypeExtensions
{
    public static long GetId(this AnsweringType value) => value switch
    {
        AnsweringType.BusinessLevel => 1L,
        AnsweringType.TaskLevel => 2L,
        AnsweringType.CurrentActivity => 3L,
        _ => throw new ArgumentOutOfRangeException(nameof(value))
    };

    public static string GetPersianName(this AnsweringType value) => value switch
    {
        AnsweringType.BusinessLevel => "در استراتژی سطح کسب و کار",
        AnsweringType.TaskLevel => "در استراتژی سطح وظیفه ای",
        AnsweringType.CurrentActivity => "در فرایندها و فعالیت های جاری",
        _ => throw new ArgumentOutOfRangeException(nameof(value))
    };

    public static string GetName(this AnsweringType value) => value.ToString();
}
