// C# migration of controller/dto/request/CreateAttachmentDto.java

namespace Common.controller.dto.request;

public class CreateAttachmentDto
{
    [JsonPropertyName("entityName")]
    public global::Common.enums.EntityNames? EntityName { get; set; }
    [JsonPropertyName("entityId")]
    public string? EntityId { get; set; }
    [JsonPropertyName("description")]
    public string? Description { get; set; }
}
