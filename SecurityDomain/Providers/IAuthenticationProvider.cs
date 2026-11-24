using SemBackDotNet10.SecurityDomain.Authentication;
using SemBackDotNet10.SecurityDomain.Model.Entity;

namespace SemBackDotNet10.SecurityDomain.Providers
{
    public interface IAuthenticationProvider
    {
        string Type { get; }

        Task<AuthenticationResult> AuthenticateAsync(
            AuthProviderEntity provider,
            AuthenticationRequest request,
            CancellationToken cancellationToken = default);
    }
}
