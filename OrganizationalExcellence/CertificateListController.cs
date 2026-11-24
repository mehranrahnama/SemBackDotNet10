using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.Controllers;
using SemBackDotNet10.Data;
using SemBackDotNet10.SecurityDomain.Tenancy;

namespace SemBackDotNet10.OrganizationalExcellence;

[ApiController]
[Route("api/v1")]
public sealed class CertificateListController(SemDbContext db, TenantContext tenant) : ControllerBase
{
    [HttpPost("Certificate/getAll")]
    [CheckAccess]
    public async Task<IActionResult> GetAll([FromBody] CertificateFilter body, CancellationToken ct)
    {
        if (!tenant.HasTenant) return Forbid();
        var pattern = body.Filter is null ? null : $"%{body.Filter}%";
        var rows = await db.Database.SqlQuery<CertificateRow>($"""
            SELECT CAST(l.levelid AS bigint) AS LevelId, l.deslevel AS DesLevel,
                   l.desthreechar AS DesThreeChar, CAST(x.certificateid AS bigint) AS CertificateId,
                   CAST(x.[year] AS bigint) AS [Year], CAST(x.desid AS bigint) AS CertificateTypeId,
                   x.desf AS CertificateTypeName
            FROM dbo.tblplaninglevel l
            LEFT JOIN (
                SELECT c.levelid, c.certificateid, c.[year], d.desid, d.desf
                FROM dbo.tblcertificate c
                JOIN dbo.tbldes d ON d.desid = c.certificatetype
            ) x ON x.levelid = l.levelid
            WHERE l.tenantid = {tenant.TenantId} AND l.deslevel LIKE {pattern}
              AND EXISTS (SELECT 1 FROM dbo.tblcertificate c WHERE c.levelid = l.levelid)
            """).ToListAsync(ct);

        return Ok(rows.GroupBy(x => new { x.LevelId, x.DesLevel, x.DesThreeChar }).Select(g => new
        {
            g.Key.LevelId,
            g.Key.DesLevel,
            g.Key.DesThreeChar,
            CertificateDtoList = g.Where(x => x.CertificateId.HasValue).OrderBy(x => x.Year)
                .Select(ToCertificateDto).ToList()
        }).ToList());
    }

    [HttpGet("Certificate/getAll/{levelId:long}")]
    [CheckAccess]
    public async Task<IActionResult> GetByLevel(long levelId, CancellationToken ct)
    {
        if (!tenant.HasTenant) return Forbid();
        var rows = await db.Database.SqlQuery<CertificateRow>($"""
            SELECT CAST(l.levelid AS bigint) AS LevelId, l.deslevel AS DesLevel,
                   l.desthreechar AS DesThreeChar, CAST(c.certificateid AS bigint) AS CertificateId,
                   CAST(c.[year] AS bigint) AS [Year], CAST(d.desid AS bigint) AS CertificateTypeId,
                   d.desf AS CertificateTypeName
            FROM dbo.tblcertificate c
            JOIN dbo.tblplaninglevel l ON l.levelid = c.levelid
            JOIN dbo.tbldes d ON d.desid = c.certificatetype
            WHERE l.levelid = {levelId} AND l.tenantid = {tenant.TenantId}
            """).OrderBy(x => x.Year).ToListAsync(ct);
        return Ok(rows.Select(ToCertificateDto).ToList());
    }

    private static object ToCertificateDto(CertificateRow x) => new
    {
        x.CertificateId, levelId = (long?)null, desLevel = (string?)null,
        x.Year, x.CertificateTypeId, x.CertificateTypeName
    };

    public sealed record CertificateFilter(string? Filter);

    private sealed class CertificateRow
    {
        public long LevelId { get; set; }
        public string? DesLevel { get; set; }
        public string? DesThreeChar { get; set; }
        public long? CertificateId { get; set; }
        public long? Year { get; set; }
        public long? CertificateTypeId { get; set; }
        public string? CertificateTypeName { get; set; }
    }
}
