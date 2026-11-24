using Common.config.security.audit;
using Microsoft.Extensions.DependencyInjection;
namespace Common.config.datasource;
public static class PersistenceConfig
{
    // Host registers identity/user lookup and invokes auditor during actual writes.
    // This does not enable a database provider, transactions or automatic EF auditing.
    public static IServiceCollection AddSemAuditor<TUser>(this IServiceCollection services) where TUser:class
    { services.AddScoped<AuditorAwareImpl<TUser>>(); return services; }
}
