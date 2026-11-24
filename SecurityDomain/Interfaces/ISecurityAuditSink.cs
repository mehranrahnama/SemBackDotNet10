using SemBackDotNet10.SecurityDomain.Model.Entity;

namespace SemBackDotNet10.SecurityDomain.Interfaces;

public interface ISecurityAuditSink
{
    Task RecordAsync(SecurityAuditEvent auditEvent, CancellationToken cancellationToken = default);
}