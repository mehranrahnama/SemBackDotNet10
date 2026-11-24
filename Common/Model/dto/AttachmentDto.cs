// C# migration of model/dto/AttachmentDto.java

namespace Common.model.dto;

public class AttachmentDto
{
    [Key] [JsonPropertyName("id")]
    public long? Id { get; set; }
    [JsonPropertyName("fileName")]
    public string? FileName { get; set; }
    [JsonPropertyName("des")]
    public string? Des { get; set; }
    [JsonPropertyName("createdBy")]
    public long? CreatedBy { get; set; }
}
