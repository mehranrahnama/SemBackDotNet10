namespace SemBackDotNet10.actionDomain.strategy.confirmStatusHandler.context;

public class CartableStatusContext(long? entityId, string? outcome)
{
    public long? EntityId { get; set; } = entityId;
    public string? Outcome { get; set; } = outcome;
}
