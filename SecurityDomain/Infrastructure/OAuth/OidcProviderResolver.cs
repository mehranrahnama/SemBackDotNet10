using SemBackDotNet10.SecurityDomain.Interfaces;
using SemBackDotNet10.SecurityDomain.Model.Entity;

namespace SemBackDotNet10.SecurityDomain.Infrastructure.OAuth;

public sealed class OidcProviderResolver(IOAuthProviderRepository repository)
{
    public async Task<IReadOnlyList<OidcProviderOptions>> ResolveAsync(
        long tenantId,
        CancellationToken cancellationToken = default)
    {
        var providers = await repository.GetOidcProvidersAsync(tenantId, cancellationToken);
        return providers
            .Where(provider => string.Equals(provider.Type, "OIDC", StringComparison.OrdinalIgnoreCase))
            .Where(provider => !string.IsNullOrWhiteSpace(provider.RegistrationId) &&
                               !string.IsNullOrWhiteSpace(provider.ClientId))
            .Select(provider => new OidcProviderOptions
            {
                Id = provider.Id,
                TenantId = provider.TenantId,
                RegistrationId = provider.RegistrationId!,
                ClientId = provider.ClientId!,
                ClientSecret = provider.ClientSecret ?? string.Empty,
                RedirectUri = provider.RedirectUri,
                IssuerUri = provider.IssuerUri,
                AuthorizationUri = provider.AuthorizationUri,
                TokenUri = provider.TokenUri,
                JwkSetUri = provider.JwkSetUri,
                Scope = provider.Scope
            })
            .ToArray();
    }
}
