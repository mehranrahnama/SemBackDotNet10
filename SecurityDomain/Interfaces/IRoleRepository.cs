namespace SemBackDotNet10.SecurityDomain.Interfaces;

public interface IRoleRepository
{
    Task<IReadOnlyList<(long Id, string Name)>> GetForUserAsync(long tenantId, long userId, CancellationToken cancellationToken = default);
}