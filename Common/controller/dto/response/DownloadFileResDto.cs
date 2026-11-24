// C# migration of controller/dto/response/DownloadFileResDto.java

namespace Common.controller.dto.response;

public class DownloadFileResDto
{
    [JsonPropertyName("fileName")]
    public string? FileName { get; set; }
    [JsonPropertyName("data")]
    public Stream? Data { get; set; }
    [JsonPropertyName("fileSize")]
    public long? FileSize { get; set; }
}
