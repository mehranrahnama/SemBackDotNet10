using Microsoft.Extensions.Diagnostics.HealthChecks;
using SemBackDotNet10.SecurityDomain.Providers;
using SemBackDotNet10.SecurityDomain.Tenancy;

namespace SemBackDotNet10.SecurityDomain.Infrastructure.Health;

public sealed class LdapHealthCheck(
    IAuthProviderRepository providerRepository,
    ILdapAuthenticator ldapAuthenticator,
    TenantContext tenantContext) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        if (!tenantContext.HasTenant)
            return HealthCheckResult.Unhealthy("A tenant could not be resolved.");

        var providers = await providerRepository.GetProvidersByTenantAsync(
            tenantContext.TenantId, cancellationToken);
        var ldapProviders = providers.Where(provider => ProviderTypeResolver.IsLdap(provider.Type)).ToArray();

        if (ldapProviders.Length == 0)
            return HealthCheckResult.Healthy("LDAP is not configured for the current tenant.");

        var failures = 0;
        foreach (var provider in ldapProviders)
        {
            try
            {
                await ldapAuthenticator.CheckConnectionAsync(provider, cancellationToken);
            }
            catch
            {
                failures++;
            }
        }

        return failures == 0
            ? HealthCheckResult.Healthy($"All LDAP providers are available. Count={ldapProviders.Length}.")
            : failures == ldapProviders.Length
                ? HealthCheckResult.Unhealthy("All configured LDAP providers are unavailable.")
                : HealthCheckResult.Degraded("Some configured LDAP providers are unavailable.");
    }
}
