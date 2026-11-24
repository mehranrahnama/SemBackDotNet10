using SemBackDotNet10.SecurityDomain.Model.Entity;

namespace SemBackDotNet10.SecurityDomain.Interfaces;

public interface ILdapAuthenticator
{
    Task CheckConnectionAsync(AuthProviderEntity provider, CancellationToken cancellationToken = default);
}
