// C# migration of controller/dto/response/StoredAttachmentResDto.java

namespace Common.controller.dto.response;

public class StoredAttachmentResDto
{
    [Key] [JsonPropertyName("id")]
    public long? Id { get; set; }
    [JsonPropertyName("entityId")]
    public string? EntityId { get; set; }
    [JsonPropertyName("fileName")]
    public string? FileName { get; set; }
    [JsonPropertyName("fileType")]
    public string? FileType { get; set; }
    [JsonPropertyName("fileSize")]
    public long? FileSize { get; set; }
    [JsonPropertyName("description")]
    public string? Description { get; set; }
    [JsonPropertyName("downloadUrl")]
    public string? DownloadUrl { get; set; }
    [JsonPropertyName("createdBy")]
    public long? CreatedBy { get; set; }
    [JsonPropertyName("modifiedBy")]
    public long? ModifiedBy { get; set; }
}
