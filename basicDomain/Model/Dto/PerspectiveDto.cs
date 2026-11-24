using System.Text.Json.Serialization;

namespace SemBackDotNet10.basicDomain.Model.Dto;

/// <summary>معادل PerspectiveDto جاوا.</summary>
public class PerspectiveDto
{
    public long Id { get; set; }

    public string? DesperspectF { get; set; }

    [JsonConverter(typeof(EmptyStringNullableLongConverter))]
    public long? Orders { get; set; }

    [JsonConverter(typeof(EmptyStringNullableLongConverter))]
    public long? Levelid { get; set; }

    public string? DesLevel { get; set; }

    [JsonConverter(typeof(EmptyStringNullableLongConverter))]
    public long? Planid { get; set; }

    [JsonConverter(typeof(EmptyStringNullableLongConverter))]
    public long? ParentId { get; set; }

    public int? Weight { get; set; }
}
