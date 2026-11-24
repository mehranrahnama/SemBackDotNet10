using Microsoft.AspNetCore.Mvc;
using SemBackDotNet10.Controllers;
using SemBackDotNet10.service.impl;
using QueryDirection = SemBackDotNet10.Common.enums.SortDirection;
using PageDirection = SemBackDotNet10.service.impl.SortDirection;

namespace SemBackDotNet10.actionDomain.controller;

[ApiController]
[Route("api/v1/actionPlan/object/page")]
public sealed class ActionObjectPageController(ActionObjectPageService service) : ControllerBase
{
    [HttpPost]
    [CheckAccess]
    public async Task<IActionResult> Read([FromBody] ActionObjectPageService.GridInput body, [FromQuery] int? page,
        [FromQuery] int? size, [FromQuery] string? orderBy, [FromQuery] QueryDirection? direction, CancellationToken ct)
    {
        if (body.LevelId is null || body.PlanId is null || body.IsPlanned is null || page is null || size is null)
            return BadRequest(new { message = "levelId, planId, isPlanned, page and size are required." });
        ServicePageRequest? paging = null;
        if (page is int index && size is int count)
        {
            if (index < 0 || count <= 0 || (long)index * count > int.MaxValue)
                return BadRequest(new { message = "Invalid page or size." });
            var sorted = direction.HasValue && !string.IsNullOrWhiteSpace(orderBy);
            var sort = sorted ? orderBy! : "ObjectId";
            if (string.Equals(sort, "id", StringComparison.OrdinalIgnoreCase)) sort = "ObjectId";
            if (string.Equals(sort, "title", StringComparison.OrdinalIgnoreCase)) sort = "ObjectTitle";
            if (typeof(SemBackDotNet10.actionDomain.model.dto.ActionObjectDto).GetProperty(sort,
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase) is null)
                return BadRequest(new { message = "Unknown sort property." });
            paging = new(index, count, sort, sorted && direction == QueryDirection.Desc ? PageDirection.Descending : PageDirection.Ascending);
        }
        return Ok(await service.ReadAsync(body, paging!, ct));
    }
}
