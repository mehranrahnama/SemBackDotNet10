using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.basicDomain.Repositories;
using SemBackDotNet10.Data;
using SemBackDotNet10.webserviceDomain.domain.entity;

namespace SemBackDotNet10.webserviceDomain.repository;

public interface IApiKeyRepository : IBaseRepository<ApiKeys>
{
    Task<ApiKeys?> FindByApiKeyAsync(string apiKey, CancellationToken ct = default);
}

public class ApiKeyRepository : BaseRepository<ApiKeys>, IApiKeyRepository
{
    public ApiKeyRepository(SemDbContext context) : base(context) { }

    public Task<ApiKeys?> FindByApiKeyAsync(string apiKey, CancellationToken ct = default) =>
        Query.FirstOrDefaultAsync(apk => apk.ApiKey == apiKey, ct);
}
