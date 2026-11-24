using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.Controllers;
using SemBackDotNet10.Data;
using SemBackDotNet10.SecurityDomain.Tenancy;

namespace SemBackDotNet10.OrganizationalExcellence;

[ApiController]
[Route("api/v1")]
public sealed class ActionCriteriaListController(SemDbContext db, TenantContext tenant) : ControllerBase
{
    [HttpGet("ActionCriteria/getAll/{levelId:long}")]
    [CheckAccess]
    public async Task<IActionResult> GetByLevel(long levelId, CancellationToken ct)
    {
        if (!tenant.HasTenant) return Forbid();
        var rows = await db.Database.SqlQuery<Row>($"""
            SELECT CAST(a.actioncriteriaid AS bigint) AS Id, CAST(c.criteriaid AS bigint) AS CriteriaId,
                   CAST(p.criteriaid AS bigint) AS ParentId, a.actionname AS ActionName,
                   a.startdate AS StartDate, a.enddate AS EndDate, CAST(a.score AS bigint) AS Score,
                   a.actualvalue AS ActualValue, a.feedback AS Feedback, CAST(c.levelid AS bigint) AS LevelId
            FROM dbo.tblactioncriteria a
            JOIN dbo.tblcriteria c ON c.criteriaid = a.criteriaid
            JOIN dbo.tblplaninglevel l ON l.levelid = c.levelid AND l.tenantid = {tenant.TenantId}
            JOIN dbo.tblcriteria p ON p.criteriaid = c.parentid AND p.levelid = c.levelid
            WHERE c.levelid = {levelId} AND a.isdeleted = 0
            """).ToListAsync(ct);
        return Ok(rows.Select(x => new { x.Id, x.CriteriaId, criteriaName = (string?)null,
            x.ParentId, parentCriteriaName = (string?)null, x.ActionName, x.StartDate, x.EndDate,
            x.Score, x.ActualValue, x.Feedback, x.LevelId }).ToList());
    }

    private sealed class Row
    {
        public long Id { get; set; }
        public long CriteriaId { get; set; }
        public long ParentId { get; set; }
        public string? ActionName { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public long? Score { get; set; }
        public decimal? ActualValue { get; set; }
        public string? Feedback { get; set; }
        public long LevelId { get; set; }
    }
}
