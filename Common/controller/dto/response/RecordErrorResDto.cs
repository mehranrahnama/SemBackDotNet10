// C# migration of controller/dto/response/RecordErrorResDto.java

namespace Common.controller.dto.response;

public class RecordErrorResDto
{
    [Key] [JsonPropertyName("id")]
    public long? Id { get; set; }
    [JsonPropertyName("title")]
    public string? Title { get; set; }
    [JsonPropertyName("message")]
    public string? Message { get; set; }
}
