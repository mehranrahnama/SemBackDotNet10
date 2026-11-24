using SemBackDotNet10.SecurityDomain.Authentication;
using SemBackDotNet10.SecurityDomain.Interfaces;
using SemBackDotNet10.SecurityDomain.Providers;

namespace SemBackDotNet10.SecurityDomain.Infrastructure.Authentication;

public sealed class ProviderAuthenticationService(
    IAuthProviderRepository configurations,
    IEnumerable<IAuthenticationProvider> providers,
    JdbcAuthenticationService localAuthentication) : IAuthenticationService
{
    public async Task<AuthenticationResult> AuthenticateAsync(
        AuthenticationRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (string.IsNullOrEmpty(request.Password) || string.IsNullOrWhiteSpace(request.Username))
            return AuthenticationResult.Failed(request.TenantId, request.Username, "INVALID_CREDENTIALS");

        var ldapProviders = providers.Where(p => ProviderTypeResolver.IsLdap(p.Type))
            .ToDictionary(p => ProviderTypeResolver.Normalize(p.Type), StringComparer.Ordinal);
        var configured = await configurations.GetProvidersByTenantAsync(request.TenantId, cancellationToken);
        foreach (var configuration in configured)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!ldapProviders.TryGetValue(ProviderTypeResolver.Normalize(configuration.Type), out var provider))
                continue;

            var result = await provider.AuthenticateAsync(configuration, request, cancellationToken);
            if (result.IsAuthenticated)
            {
                if (result.TenantId != request.TenantId)
                    throw new InvalidOperationException("LDAP returned an identity for a different tenant.");
                return result;
            }
        }

        return await localAuthentication.AuthenticateAsync(request, cancellationToken);
    }
}
