// C# migration of model/dto/CartableStatusDto.java

namespace Common.model.dto;

public class CartableStatusDto
{
    [Key] [JsonPropertyName("id")]
    public long? Id { get; set; }
    [JsonPropertyName("entityName")]
    public string? EntityName { get; set; }
    [JsonPropertyName("outCome")]
    public string? OutCome { get; set; }
    [JsonPropertyName("oldStatus")]
    public long? OldStatus { get; set; }
    [JsonPropertyName("newStatus")]
    public long? NewStatus { get; set; }
    [JsonPropertyName("operationType")]
    public string? OperationType { get; set; }
    [JsonPropertyName("fromRoleId")]
    public long? FromRoleId { get; set; }
    [JsonPropertyName("fromRoleName")]
    public string? FromRoleName { get; set; }
    [JsonPropertyName("toRoleId")]
    public long? ToRoleId { get; set; }
    [JsonPropertyName("toRoleName")]
    public string? ToRoleName { get; set; }
    [JsonPropertyName("oldStatusMeaning")]
    public string? OldStatusMeaning { get; set; }
    [JsonPropertyName("newStatusMeaning")]
    public string? NewStatusMeaning { get; set; }
}
