using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.actionDomain.model.entity;
using SemBackDotNet10.actionDomain.repository;
using SemBackDotNet10.Controllers;
using SemBackDotNet10.Data;
using SemBackDotNet10.SecurityDomain.Infrastructure.Security;
using SemBackDotNet10.SecurityDomain.Tenancy;
using SemBackDotNet10.service.impl;

namespace SemBackDotNet10.actionDomain.controller;

[ApiController]
[Authorize]
[Route("api/v1/actionActPlan/page")]
public sealed class ActionActPlanCatalogController(
    SemDbContext db, TenantContext tenant, UserContext user, PlaningLevelUserServiceImpl levelUsers) : ControllerBase
{
    [HttpGet("{catalogId}")]
    [CheckAccess]
    public async Task<IActionResult> Get(string catalogId, CancellationToken cancellationToken)
    {
        if (!long.TryParse(catalogId, out var id) || id <= 0)
            return BadRequest(new { message = "actionPlanCatalogId must be a positive number." });
        if (!tenant.HasTenant)
            return Forbid();

        var levelId = await db.Set<ActionPlanCatalog>().AsNoTracking()
            .Where(catalog => catalog.Id == id && catalog.PlaningLevel != null &&
                              catalog.PlaningLevel.FactoryId == tenant.TenantId)
            .Select(catalog => (long?)catalog.PlaningLevel!.Id)
            .SingleOrDefaultAsync(cancellationToken);
        if (!levelId.HasValue)
            return NotFound();

        await levelUsers.HasAccessInLevelIdAsync(levelId, user.UserId, cancellationToken);
        var plans = db.ActionActPlans.AsNoTracking().Where(plan => plan.ActionPlanCatalog != null &&
            plan.ActionPlanCatalog.PlaningLevel != null && plan.ActionPlanCatalog.PlaningLevel.FactoryId == tenant.TenantId);
        var rows = await ActionActPlanRepository.BuildCatalogDataQuery(plans, id).ToListAsync(cancellationToken);
        return Ok(rows.ToHashSet());
    }
}
