using SemBackDotNet10.SecurityDomain.Authentication;
using SemBackDotNet10.SecurityDomain.Interfaces;
using SemBackDotNet10.SecurityDomain.Providers;

namespace SemBackDotNet10.SecurityDomain.Infrastructure.Authentication;

public sealed class JdbcAuthenticationService(
    IUserAccountRepository accountRepository,
    IRoleRepository roleRepository,
    IPasswordVerifier passwordVerifier) : IAuthenticationService
{
    public async Task<AuthenticationResult> AuthenticateAsync(
        AuthenticationRequest request,
        CancellationToken cancellationToken = default)
    {
        var account = await accountRepository.FindJdbcAccountAsync(
            request.TenantId,
            request.Username,
            cancellationToken);

        if (account?.User is null || account.Provider is null || account.User.LoginDisabled == true || account.User.Retired == true)
            return AuthenticationResult.Failed(request.TenantId, request.Username, "INVALID_CREDENTIALS");

        if (!passwordVerifier.Verify(account.Password, request.Password))
            return AuthenticationResult.Failed(request.TenantId, request.Username, "INVALID_CREDENTIALS");

        var roles = await roleRepository.GetForUserAsync(
            request.TenantId,
            account.User.UserId,
            cancellationToken);

        return AuthenticationResult.Success(
            request.TenantId,
            account.AccountName,
            string.Join(' ', new[] { account.User.NameF, account.User.FamilyF }.Where(value => !string.IsNullOrWhiteSpace(value))),
            ProviderTypeResolver.Normalize(account.Provider.Type),
            account.User.UserId,
            account.Id,
            roles.Select(role => role.Name).ToArray(),
            roles.Select(role => role.Id).ToArray());
    }
}
