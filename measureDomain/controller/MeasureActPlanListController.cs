using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.Controllers;
using SemBackDotNet10.SecurityDomain.Tenancy;
using SemBackDotNet10.measureDomain.model.dto;
using SemBackDotNet10.service.impl;

namespace SemBackDotNet10.measureDomain.controller;

[ApiController]
[Route("api/v1")]
public sealed class MeasureActPlanListController(ServiceRuntime runtime, PlaningLevelUserServiceImpl levels, TenantContext tenant) : ControllerBase
{
    [HttpGet("findAllmeasureActPlansByMeasureId/{measureId:long}")]
    [CheckAccess]
    public async Task<IActionResult> FindAll(long measureId, CancellationToken ct)
    {
        if (measureId <= 0) return BadRequest();
        if (!tenant.HasTenant) return Forbid();

        var level = await runtime.Db.Database.SqlQuery<long>($"""
            SELECT CAST(c.levelid AS bigint) AS [Value]
            FROM dbo.tbldimmeasure m
            JOIN dbo.tbldimmeasurecatalog c ON c.dimmeasurecatalogid = m.dimmeasurecatalogid
            JOIN dbo.tblplaninglevel l ON l.levelid = c.levelid
            WHERE m.measureid = {measureId} AND l.tenantid = {tenant.TenantId}
            """).ToListAsync(ct);
        if (level.Count == 0) return Ok(Array.Empty<MeasureActPlanDto>());
        await levels.HasAccessInLevelIdAsync(level.Single(), runtime.User.Id, ct);

        var rows = await runtime.Db.Database.SqlQuery<MeasureActPlanListRow>($"""
            SELECT CAST(ap.dimmeasureactplanid AS bigint) AS Id,
                   CAST(ap.month AS bigint) AS Month, ap.planvalue AS PlanValue,
                   ap.actualvalue AS ActualValue, CAST(period.planid AS bigint) AS PlanId,
                   CAST(c.dimmeasurecatalogid AS bigint) AS DimMeasureId
            FROM dbo.tbldimmeasureactplan ap
            JOIN dbo.tbldimmeasure m ON m.measureid = ap.dimmeasureid
            JOIN dbo.tbldimmeasurecatalog c ON c.dimmeasurecatalogid = m.dimmeasurecatalogid
            JOIN dbo.tblplaninglevel l ON l.levelid = c.levelid
            LEFT JOIN dbo.tblplanperiod period ON period.planid = m.planid
            WHERE m.measureid = {measureId} AND l.tenantid = {tenant.TenantId}
              AND ap.isdeleted = 0 AND ap.active = 1
            ORDER BY ap.month
            """).ToListAsync(ct);
        return Ok(rows.Select(row => new MeasureActPlanDto
        {
            Id = row.Id, Month = row.Month, PlanValue = row.PlanValue, ActualValue = row.ActualValue,
            PlanId = row.PlanId, DimMeasureId = row.DimMeasureId
        }).ToList());
    }
}

public sealed class MeasureActPlanListRow
{
    public long Id { get; set; }
    public long? Month { get; set; }
    public decimal? PlanValue { get; set; }
    public decimal? ActualValue { get; set; }
    public long? PlanId { get; set; }
    public long? DimMeasureId { get; set; }
}
