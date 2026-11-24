using SemBackDotNet10.SecurityDomain.Model.Entity;

namespace SemBackDotNet10.SecurityDomain.Infrastructure.Sso;

public interface ISsoUserClient
{
    Task<SsoUserDto> GetUserAsync(string accessToken, CancellationToken cancellationToken = default);
}
