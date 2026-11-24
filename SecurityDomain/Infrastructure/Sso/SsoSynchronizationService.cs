using SemBackDotNet10.SecurityDomain.Interfaces;
using SemBackDotNet10.SecurityDomain.Model.Entity;
using SemBackDotNet10.SecurityDomain.Tenancy;

namespace SemBackDotNet10.SecurityDomain.Infrastructure.Sso;

public sealed class SsoSynchronizationService(
    TenantContext tenantContext,
    ISsoUserClient userClient,
    ISsoAccountRepository accountRepository) : ISsoSynchronizationService
{
    public async Task<SsoSynchronizationResult> SynchronizeAsync(
        string username,
        string accessToken,
        CancellationToken cancellationToken = default)
    {
        if (!tenantContext.HasTenant)
            throw new InvalidOperationException("A tenant must be resolved before SSO synchronization.");

        var user = await userClient.GetUserAsync(accessToken, cancellationToken);
        if (string.IsNullOrWhiteSpace(user.Username) ||
            !string.Equals(user.Username, username, StringComparison.OrdinalIgnoreCase))
            throw new UnauthorizedAccessException("The SSO identity does not match the requested user.");

        return await accountRepository.SynchronizeAsync(
            tenantContext.TenantId,
            user,
            cancellationToken);
    }
}
