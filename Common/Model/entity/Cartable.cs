// C# migration of model/entity/Cartable.java

namespace Common.model.entity;

public class Cartable
{
    [JsonPropertyName("entityId")]
    public long? EntityId { get; set; }
    [JsonPropertyName("entityName")]
    public string? EntityName { get; set; }
    [JsonPropertyName("title")]
    public string? Title { get; set; }
    [JsonPropertyName("tashilgar")]
    public string? Tashilgar { get; set; }
    [JsonPropertyName("modir")]
    public string? Modir { get; set; }
    [JsonPropertyName("sentDate")]
    public DateTime? SentDate { get; set; }
    [JsonPropertyName("createdDate")]
    public string? CreatedDate { get; set; }
    [JsonPropertyName("taskUrl")]
    public string? TaskUrl { get; set; }
    [JsonPropertyName("assigneeId")]
    public long? AssigneeId { get; set; }
    [JsonPropertyName("statusDes")]
    public string? StatusDes { get; set; }
    [JsonPropertyName("isDeleted")]
    public bool? IsDeleted { get; set; } = false;
    [JsonPropertyName("levelId")]
    public long? LevelId { get; set; }
    [JsonPropertyName("desLevel")]
    public string? DesLevel { get; set; }
}
