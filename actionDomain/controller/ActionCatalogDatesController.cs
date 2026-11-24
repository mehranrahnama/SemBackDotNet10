using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.actionDomain.model.dto;
using SemBackDotNet10.Controllers;
using SemBackDotNet10.SecurityDomain.Tenancy;
using SemBackDotNet10.service.impl;

namespace SemBackDotNet10.actionDomain.controller;

[ApiController]
[Route("api/v1/actionPlanCatalog/dates")]
public sealed class ActionCatalogDatesController(ServiceRuntime runtime, PlaningLevelUserServiceImpl levels, TenantContext tenant) : ControllerBase
{
    [HttpGet("~/api/v1/actionPlanCatalog/{id:long}")]
    [CheckAccess]
    public async Task<IActionResult> GetCatalog(long id, CancellationToken ct)
    {
        if (!tenant.HasTenant) return Forbid();
        if (id <= 0) return BadRequest(new { message = "شناسهٔ کاتالوگ اقدام معتبر نیست." });
        var catalog = await runtime.Db.Database.SqlQuery<ActionCatalogDetailsRow>($"""
            SELECT CAST(c.actionplancatalogid AS bigint) AS Id, c.code AS Code, c.description AS Description,
                   c.startdate AS StartDate, c.enddate AS EndDate, CAST(c.totalbudget AS int) AS TotalBudget,
                   CAST(c.confirmstatus AS bigint) AS ActionConfirmStatus, CAST(l.levelid AS bigint) AS LevelId,
                   l.deslevel AS DesLevel, CAST(c.isdeleted AS bit) AS DeleteStatus,
                   CONCAT(p.namef, ' ', p.familyf) AS Payeshgar, CONCAT(m.namef, ' ', m.familyf) AS Modir,
                   CAST(m.userid AS bigint) AS ModirId, CAST(p.userid AS bigint) AS PayeshgarId
            FROM dbo.tblactionplancatalog c
            JOIN dbo.tblplaninglevel l ON l.levelid = c.levelid
            LEFT JOIN dbo.tblUser m ON m.userid = c.modir
            LEFT JOIN dbo.tblUser p ON p.userid = c.payeshgar
            WHERE c.actionplancatalogid = {id} AND l.tenantid = {tenant.TenantId}
            """).SingleOrDefaultAsync(ct);
        if (catalog is null) return NotFound();
        await levels.HasAccessInLevelIdAsync(catalog.LevelId, runtime.User.Id, ct);
        return Ok(new ActionPlanCatalogDto
        {
            Id = catalog.Id, Code = catalog.Code, Description = catalog.Description,
            StartDate = catalog.StartDate, EndDate = catalog.EndDate, TotalBudget = catalog.TotalBudget,
            ActionConfirmStatus = catalog.ActionConfirmStatus, LevelId = catalog.LevelId, DesLevel = catalog.DesLevel,
            DeleteStatus = catalog.DeleteStatus, Payeshgar = catalog.Payeshgar, Modir = catalog.Modir,
            ModirId = catalog.ModirId, PayeshgarId = catalog.PayeshgarId
        });
    }

    [HttpGet("{id:long}")]
    [CheckAccess]
    public async Task<IActionResult> GetDates(long id, CancellationToken ct)
    {
        if (!tenant.HasTenant) return Forbid();
        if (id <= 0) return BadRequest(new { message = "شناسهٔ کاتالوگ اقدام معتبر نیست." });

        var catalog = await runtime.Db.Database.SqlQuery<ActionCatalogDateRow>($"""
            SELECT CAST(c.levelid AS bigint) AS LevelId, c.startdate AS StartDate,
                   CASE WHEN log.actiondatelogid IS NOT NULL AND c.replannedstatus IS NOT NULL
                                  AND c.replannedstatus NOT IN (0, 70)
                        THEN log.enddateold ELSE c.enddate END AS EndDate
            FROM dbo.tblactionplancatalog c
            JOIN dbo.tblplaninglevel l ON l.levelid = c.levelid
            OUTER APPLY (
                SELECT TOP (1) d.actiondatelogid, d.enddateold
                FROM dbo.tblactiondatelog d
                WHERE d.actionplancatalogid = c.actionplancatalogid
                ORDER BY d.modifieddate DESC, d.actiondatelogid DESC
            ) log
            WHERE c.actionplancatalogid = {id} AND l.tenantid = {tenant.TenantId}
            """).SingleOrDefaultAsync(ct);
        if (catalog is null) return NotFound();
        await levels.HasAccessInLevelIdAsync(catalog.LevelId, runtime.User.Id, ct);
        return Ok(new { catalog.StartDate, catalog.EndDate });
    }
}

public sealed class ActionCatalogDateRow
{
    public long LevelId { get; set; }
    public string? StartDate { get; set; }
    public string? EndDate { get; set; }
}

public sealed class ActionCatalogDetailsRow
{
    public long Id { get; set; }
    public string? Code { get; set; }
    public string? Description { get; set; }
    public string? StartDate { get; set; }
    public string? EndDate { get; set; }
    public int? TotalBudget { get; set; }
    public long? ActionConfirmStatus { get; set; }
    public long LevelId { get; set; }
    public string? DesLevel { get; set; }
    public bool? DeleteStatus { get; set; }
    public string? Payeshgar { get; set; }
    public string? Modir { get; set; }
    public long? ModirId { get; set; }
    public long? PayeshgarId { get; set; }
}
