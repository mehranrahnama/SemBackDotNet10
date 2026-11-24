// C# migration of model/dto/DesDto.java

namespace Common.model.dto;

public class DesDto
{
    [Key] [JsonPropertyName("id")]
    public long? Id { get; set; }
    [JsonPropertyName("desF")]
    public string? DesF { get; set; }
    [JsonPropertyName("val")]
    public string? Val { get; set; }
}
