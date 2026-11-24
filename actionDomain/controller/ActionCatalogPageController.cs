using Microsoft.AspNetCore.Mvc;
using SemBackDotNet10.Controllers;
using SemBackDotNet10.SecurityDomain.Tenancy;
using SemBackDotNet10.service.impl;
using QueryDirection = SemBackDotNet10.Common.enums.SortDirection;
using PageDirection = SemBackDotNet10.service.impl.SortDirection;

namespace SemBackDotNet10.actionDomain.controller;

[ApiController]
[Route("api/v1/actionPlanCatalog/page")]
public sealed class ActionCatalogPageController(ActionCatalogPageService service, TenantContext tenant) : ControllerBase
{
    public sealed record GridInput(string? Filter, long? LevelId, long? Status);
    public sealed record PlanGridInput(string? Filter, long? LevelId, long? Status, long? PlanId);

    [HttpPost("by_planId")]
    [CheckAccess]
    public async Task<IActionResult> ReadByPlan([FromBody] PlanGridInput body, [FromQuery] int? page,
        [FromQuery] int? size, [FromQuery] string? orderBy, [FromQuery] QueryDirection? direction, CancellationToken ct)
    {
        if (!tenant.HasTenant) return Forbid();
        if (body.PlanId is not > 0 || body.LevelId is not > 0 || page is null || size is null)
            return BadRequest(new { message = "planId, levelId, page and size are required." });
        var year = (body.PlanId.Value / 100L).ToString(System.Globalization.CultureInfo.InvariantCulture);
        return await ReadCore(new(body.Filter, body.LevelId, body.Status), page, size, orderBy, direction, ct, year, tenant.TenantId);
    }

    [HttpPost]
    [CheckAccess]
    public async Task<IActionResult> Read([FromBody] GridInput body, [FromQuery] int? page,
        [FromQuery] int? size, [FromQuery] string? orderBy, [FromQuery] QueryDirection? direction, CancellationToken ct)
        => !tenant.HasTenant ? Forbid() : await ReadCore(body, page, size, orderBy, direction, ct, tenantId: tenant.TenantId);

    private async Task<IActionResult> ReadCore(GridInput body, int? page, int? size, string? orderBy,
        QueryDirection? direction, CancellationToken ct, string? year = null, long? tenantId = null)
    {
        if (body.Filter is null) return BadRequest(new { message = "filter is required." });
        ServicePageRequest? paging = null;
        if (page is int index && size is int count)
        {
            var effectiveSize = year is null ? count : Math.Min(count, 200);
            if (index < 0 || count <= 0 || (long)index * effectiveSize > int.MaxValue)
                return BadRequest(new { message = "Invalid page or size." });
            var sorted = direction.HasValue && !string.IsNullOrWhiteSpace(orderBy);
            var sort = sorted ? orderBy! : "Id";
            if (string.Equals(sort, "confirmstatus", StringComparison.OrdinalIgnoreCase)) sort = "ActionConfirmStatus";
            if (string.Equals(sort, "createddate", StringComparison.OrdinalIgnoreCase)) sort = "CreateDate";
            if (typeof(SemBackDotNet10.actionDomain.model.dto.ActionPlanCatalogDto).GetProperty(sort,
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase) is null)
                return BadRequest(new { message = "Unknown sort property." });
            paging = new(index, effectiveSize, sort, sorted && direction == QueryDirection.Desc ? PageDirection.Descending : PageDirection.Ascending);
        }
        var result = await service.ReadAsync(body.Filter, body.LevelId, body.Status, paging, ct, year, tenantId);
        if (year is not null && result is ServicePage<SemBackDotNet10.actionDomain.model.dto.ActionPlanCatalogDto> grid)
            return Ok(new { grid.Content, grid.TotalElements, grid.TotalPages, grid.CurrentPage, pageSize = size });
        return Ok(result);
    }
}
