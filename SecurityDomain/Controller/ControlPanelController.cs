using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using SemBackDotNet10.Common.enums;
using SemBackDotNet10.SecurityDomain.Service.Impl;

namespace SemBackDotNet10.Controllers;

[ApiController]
[Route("api/v1")]
public sealed class ControlPanelController(ControlPanelPageReader reader) : ControllerBase
{
    [HttpPost("controlpanel/page")]
    [HttpPost("controlpanel/page/forMenu")]
    [CheckAccess]
    public async Task<IActionResult> Page([FromQuery] int page, [FromQuery] int size,
        [FromQuery] string? orderBy, [FromQuery] SortDirection direction = SortDirection.Asc,
        [FromBody] JsonElement body = default, CancellationToken ct = default)
    {
        if (page < 0 || size < 1 || body.ValueKind != JsonValueKind.Object) return BadRequest("Invalid page or body.");
        if (!body.TryGetProperty("entitytype", out var type) || type.ValueKind == JsonValueKind.Null) return Ok(null);
        int entityType;
        if (type.ValueKind == JsonValueKind.Number && type.TryGetInt32(out var number)) entityType = number;
        else if (type.ValueKind == JsonValueKind.String && int.TryParse(type.GetString(), out number)) entityType = number;
        else return BadRequest("Invalid entitytype.");
        string? filter = null;
        if (body.TryGetProperty("filter", out var value))
        {
            if (value.ValueKind is not (JsonValueKind.String or JsonValueKind.Null)) return BadRequest("Invalid filter.");
            if (value.ValueKind == JsonValueKind.String) filter = value.GetString();
        }
        return Ok(await reader.FindAsync(entityType, filter, page, size, orderBy, direction, ct));
    }
}
