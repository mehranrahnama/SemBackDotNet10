using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.Data;

namespace SemBackDotNet10.SecurityDomain.Service.Impl;

public sealed class ApplicationConfigService(SemDbContext db)
{
    public IReadOnlyList<ApplicationConfig> FindActives() => db.Database
        .SqlQuery<ApplicationConfig>($"""
            SELECT applicationconfigid AS Id, servicetype AS ServiceType,
                   defaulttenantid AS DefaultTenantId, status AS Status
            FROM dbo.tblapplicationconfig
            WHERE status = {"ACTIVE"}
            """)
        .ToList();
}

public sealed class ApplicationConfig
{
    public long Id { get; set; }
    public string? ServiceType { get; set; }
    public long DefaultTenantId { get; set; }
    public string? Status { get; set; }
}
