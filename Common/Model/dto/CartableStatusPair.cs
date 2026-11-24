// C# migration of model/dto/CartableStatusPair.java

namespace Common.model.dto;

public class CartableStatusPair
{
    [JsonPropertyName("oldStatus")]
    public long? OldStatus { get; set; }
    [JsonPropertyName("newStatus")]
    public long? NewStatus { get; set; }
}
