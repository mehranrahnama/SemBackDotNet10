using System.Net;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.Data;
using SemBackDotNet10.SecurityDomain.Persistence;
using DomainTenant = SemBackDotNet10.basicDomain.Model.Entity.Tenant;

namespace SemBackDotNet10.SecurityDomain.Tenancy;

public sealed class TenantResolver(
    AppDbContext db,
    SemDbContext semDb,
    IConfiguration configuration,
    IHostEnvironment environment,
    IHttpContextAccessor httpContextAccessor) : ITenantResolver
{
    public async Task<long> ResolveTenantIdAsync(string host, CancellationToken cancellationToken = default)
    {
        var normalizedHost = host.Trim().Trim('[', ']').ToLowerInvariant();
        var remoteAddress = httpContextAccessor.HttpContext?.Connection.RemoteIpAddress;
        var isLoopbackHost = normalizedHost == "localhost" ||
            (IPAddress.TryParse(normalizedHost, out var address) && IPAddress.IsLoopback(address));

        // A configured default is only safe for local development requests; do not
        // let an arbitrary Host header select a tenant on a remotely exposed server.
        if (environment.IsDevelopment() && isLoopbackHost && remoteAddress != null &&
            IPAddress.IsLoopback(remoteAddress.IsIPv4MappedToIPv6 ? remoteAddress.MapToIPv4() : remoteAddress))
        {
            var configuredName = configuration["Security:DefaultTenantName"]?.Trim();
            if (!string.IsNullOrEmpty(configuredName))
            {
                var normalizedName = configuredName.ToLowerInvariant();
                var tenantId = await semDb.Set<DomainTenant>().AsNoTracking()
                    .Where(t => (t.TenantCode != null && t.TenantCode.Trim().ToLower() == normalizedName) ||
                                (t.TenantName != null && t.TenantName.Trim().ToLower() == normalizedName))
                    .Select(t => (long?)t.Id).SingleOrDefaultAsync(cancellationToken);
                return tenantId ?? throw new TenantResolutionException();
            }
        }

        var domainTenantId = await db.Set<TenantDomain>()
            .Where(x => x.IsActive && x.Domain.ToLower() == normalizedHost)
            .Select(x => (long?)x.TenantId).SingleOrDefaultAsync(cancellationToken);
        return domainTenantId ?? throw new TenantResolutionException();
    }
}
