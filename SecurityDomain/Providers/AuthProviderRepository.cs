using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.SecurityDomain.Persistence;
using SemBackDotNet10.SecurityDomain.Interfaces;
using SemBackDotNet10.SecurityDomain.Model.Entity;

namespace SemBackDotNet10.SecurityDomain.Providers
{

    public sealed class AuthProviderRepository
        : IAuthProviderRepository
        , IOAuthProviderRepository
    {
        private readonly AppDbContext _db;

        public AuthProviderRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<AuthProviderEntity>> GetOidcProvidersAsync(
            long tenantId,
            CancellationToken cancellationToken = default)
        {
            return await _db.AuthProviders
                .AsNoTracking()
                .Where(provider => provider.TenantId == tenantId && provider.Type == "OIDC")
                .OrderBy(provider => provider.Id)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<AuthProviderEntity>>
            GetProvidersByTenantAsync(
                long tenantId,
                CancellationToken cancellationToken = default)
        {
            return await _db.AuthProviders
                .AsNoTracking()
                .Where(x => x.TenantId == tenantId)
                .OrderBy(x => x.Id)
                .ToListAsync(cancellationToken);
        }
    }
    }
