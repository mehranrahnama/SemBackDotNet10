using SemBackDotNet10.SecurityDomain.Authentication;

namespace SemBackDotNet10.SecurityDomain.Interfaces;

public interface IAuthenticationService
{
    Task<AuthenticationResult> AuthenticateAsync(
        AuthenticationRequest request,
        CancellationToken cancellationToken = default);
}
