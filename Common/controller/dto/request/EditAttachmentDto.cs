// C# migration of controller/dto/request/EditAttachmentDto.java

namespace Common.controller.dto.request;

public class EditAttachmentDto
{
    [Key] [JsonPropertyName("id")]
    public long? Id { get; set; }
    [JsonPropertyName("entityId")]
    public string? EntityId { get; set; }
    [JsonPropertyName("description")]
    public string? Description { get; set; }
    [JsonPropertyName("removeFile")]
    public bool? RemoveFile { get; set; }
}
