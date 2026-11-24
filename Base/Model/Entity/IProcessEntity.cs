namespace SemBackDotNet10.Base.Model.Entity;

/// <summary>Non-generic process metadata consumed by the process-service defaults.</summary>
public interface IProcessEntity
{
    long? ProcessId { get; }
    DateTime? Modifieddate { get; }
    long Confirmstatus { get; }
    long DeleteFirstStatus { get; }
    long ReplanStatusForHistory { get; }
    bool ReplannedForHistory { get; }
    string? OutCome { get; }
}
