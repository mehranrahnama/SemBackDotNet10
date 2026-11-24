namespace SemBackDotNet10.SecurityDomain.Interfaces;

public interface IAuthorizationService
{
    bool CheckAdminRole();

    Task<bool> CheckAccessAsync(string uri, CancellationToken cancellationToken = default);
}
