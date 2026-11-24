using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.Controllers;
using SemBackDotNet10.SecurityDomain.Tenancy;
using SemBackDotNet10.service.impl;

namespace SemBackDotNet10.measureDomain.controller;

[ApiController]
[Route("api/v1/measureRepository")]
public sealed class MeasureRepositoryPlanStatusController(ServiceRuntime runtime, TenantContext tenant) : ControllerBase
{
    [HttpGet("hasMeasureActPlan/{measureRepositoryId:long}")]
    [CheckAccess]
    public async Task<IActionResult> HasMeasureActPlan(long measureRepositoryId, CancellationToken ct)
    {
        if (measureRepositoryId <= 0) return BadRequest();
        if (!tenant.HasTenant) return Forbid();

        var exists = await runtime.Db.Database.SqlQuery<int>($"""
            SELECT 1 AS [Value]
            FROM dbo.tbldimmeasureactplan ap
            JOIN dbo.tbldimmeasure m ON m.measureid = ap.dimmeasureid
            JOIN dbo.tbldimmeasurecatalog c ON c.dimmeasurecatalogid = m.dimmeasurecatalogid
            JOIN dbo.tblplaninglevel l ON l.levelid = c.levelid
            WHERE c.dimmeasurecatalogrepositoryid = {measureRepositoryId}
              AND ap.isdeleted = 0 AND l.tenantid = {tenant.TenantId}
            """).AnyAsync(ct);

        return Ok(exists);
    }
}
