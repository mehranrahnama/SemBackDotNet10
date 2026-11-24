using Microsoft.AspNetCore.Mvc;
using SemBackDotNet10.Controllers;
using SemBackDotNet10.SecurityDomain.Service.Interf;

namespace SemBackDotNet10.SecurityDomain.Controllers;

[ApiController]
[Route("api/v1/role")]
public sealed class RoleListController(IRoleService roles) : ControllerBase
{
    [HttpGet("list")]
    [CheckAccess]
    public IActionResult GetList() => Ok(roles.GetList());

    [HttpPost("page")]
    [CheckAccess]
    public IActionResult GetPage([FromQuery] int? page, [FromQuery] int? size,
        [FromQuery] string? orderBy, [FromQuery] string? direction,
        [FromBody(EmptyBodyBehavior = Microsoft.AspNetCore.Mvc.ModelBinding.EmptyBodyBehavior.Allow)]
        System.Text.Json.JsonElement? body)
    {
        if (page is null or < 0 || size is null or < 1 ||
            (long)page.Value * Math.Min(size.Value, global::Common.utils.PageUtils.MAX_PAGE_SIZE) > int.MaxValue)
            return BadRequest("Invalid page or size.");

        if (direction is not (null or "ASC" or "DESC"))
            return BadRequest("Invalid sort direction.");
        if (direction is not null && !string.IsNullOrWhiteSpace(orderBy) &&
            orderBy is not ("id" or "roleName" or "roleDes"))
            return BadRequest("Invalid role sort field.");

        if (body is { } filter && (filter.ValueKind != System.Text.Json.JsonValueKind.Object ||
            !filter.TryGetProperty("filter", out var keyword) || keyword.ValueKind != System.Text.Json.JsonValueKind.String))
            return BadRequest("Expected a JSON object with a string filter.");

        return Ok(roles.FindAll(body?.GetRawText() ?? "{\"filter\":\"\"}", page.Value, size.Value, orderBy, direction));
    }
}
