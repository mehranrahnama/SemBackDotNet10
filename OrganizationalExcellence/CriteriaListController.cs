using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.Controllers;
using SemBackDotNet10.Data;
using SemBackDotNet10.SecurityDomain.Tenancy;

namespace SemBackDotNet10.OrganizationalExcellence;

[ApiController]
[Route("api/v1")]
public sealed class CriteriaListController(SemDbContext db, TenantContext tenant) : ControllerBase
{
    [HttpGet("criteria/getAll/{levelId:long}")]
    [CheckAccess]
    public async Task<IActionResult> GetByLevel(long levelId, CancellationToken ct)
    {
        if (!tenant.HasTenant) return Forbid();
        var rows = await db.Database.SqlQuery<CriteriaRow>($"""
            SELECT CAST(p.criteriaid AS bigint) AS Id, p.criterianame AS CriteriaName,
                   CAST(p.zarib AS bigint) AS Zarib,
                   CAST(c.criteriaid AS bigint) AS SubCriteriaId, c.criterianame AS SubCriteriaName,
                   CAST(c.zarib AS bigint) AS SubZarib
            FROM dbo.tblcriteria p
            JOIN dbo.tblplaninglevel l ON l.levelid = p.levelid AND l.tenantid = {tenant.TenantId}
            LEFT JOIN dbo.tblcriteria c ON c.parentid = p.criteriaid AND c.levelid = p.levelid AND c.isdeleted = 0
            WHERE p.parentid IS NULL AND p.levelid = {levelId} AND p.isdeleted = 0
            """).ToListAsync(ct);

        return Ok(rows.GroupBy(x => new { x.Id, x.CriteriaName, x.Zarib }).Select(g => new
        {
            g.Key.Id,
            g.Key.CriteriaName,
            g.Key.Zarib,
            SubCriteriaList = g.Where(x => x.SubCriteriaId.HasValue).Select(x => new
            {
                x.SubCriteriaId,
                ParentId = x.Id,
                x.SubCriteriaName,
                Zarib = x.SubZarib
            }).ToList()
        }).ToList());
    }

    private sealed class CriteriaRow
    {
        public long Id { get; set; }
        public string? CriteriaName { get; set; }
        public long? Zarib { get; set; }
        public long? SubCriteriaId { get; set; }
        public string? SubCriteriaName { get; set; }
        public long? SubZarib { get; set; }
    }
}
