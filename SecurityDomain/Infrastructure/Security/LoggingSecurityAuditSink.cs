using System.Text.Json;
using SemBackDotNet10.SecurityDomain.Interfaces;
using SemBackDotNet10.SecurityDomain.Model.Entity;

namespace SemBackDotNet10.SecurityDomain.Infrastructure.Security;

public sealed class LoggingSecurityAuditSink(ILogger<LoggingSecurityAuditSink> logger) : ISecurityAuditSink
{
    public Task RecordAsync(SecurityAuditEvent auditEvent, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Security audit event: {Event}", JsonSerializer.Serialize(auditEvent));
        return Task.CompletedTask;
    }
}