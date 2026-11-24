namespace SemBackDotNet10.SecurityDomain.Model.Entity;

public sealed record SecurityAuditEvent(
    string EventType,
    long TenantId,
    long ActorUserId,
    long? SubjectUserId,
    string? Reason,
    DateTimeOffset OccurredAt);