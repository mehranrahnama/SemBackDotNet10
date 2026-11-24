namespace Common.config.security.audit;
public interface IAuditIdentity
{
    long? CurrentUserId { get; }
    bool IsImpersonating { get; }
    long? ImpersonatorUserId { get; }
}
public interface IAuditUserLookup<TUser> where TUser:class { TUser? FindById(long id); }
public sealed class AuditorAwareImpl<TUser>(IAuditIdentity identity,IAuditUserLookup<TUser> users) where TUser:class
{
    public TUser? GetCurrentAuditor()
    {
        var id=identity.IsImpersonating?identity.ImpersonatorUserId:identity.CurrentUserId;
        return id is null?null:users.FindById(id.Value);
    }
}
