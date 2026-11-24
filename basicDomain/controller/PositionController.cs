using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using PositionDto = SemBackDotNet10.basicDomain.Model.Dto.PositionDto;
using SemBackDotNet10.service.impl;
using ApiDirection = SemBackDotNet10.Common.enums.SortDirection;

namespace SemBackDotNet10.Controllers;

[ApiController]
[Route("api/v1")]
public class PositionController(PositionEndpointService service) : ControllerBase
{
    [HttpPost("position")]
    [CheckAccess]
    public async Task<IActionResult> CreateNewPosition([FromBody] PositionDto dto, CancellationToken ct)
        => Ok(await service.CreateAsync(dto, ct));

    [HttpPut("position")]
    [CheckAccess]
    public async Task<IActionResult> EditPositionById([FromBody] PositionDto dto, CancellationToken ct)
        => Ok(await service.EditAsync(dto, ct));

    [HttpDelete("position")]
    [CheckAccess]
    public async Task<IActionResult> DeletePosition([FromQuery] long id, CancellationToken ct)
    {
        await service.DeleteAsync(id, ct);
        return Ok();
    }

    [HttpGet("position/list")]
    [CheckAccess]
    public async Task<IActionResult> GetPositionList([FromQuery] int page, [FromQuery] int size,
        [FromQuery] string? orderBy, [FromQuery] ApiDirection direction = ApiDirection.Asc,
        [FromQuery] string? filter = null, CancellationToken ct = default)
    {
        if (page < 0 || size < 1) return BadRequest("Invalid page or size.");
        return Ok(await service.FindAsync(filter, page, size, orderBy, direction, ct));
    }

    [HttpPost("position/list")]
    [HttpPost("positionSearch/grid")]
    [CheckAccess]
    public async Task<IActionResult> GetPositionListPost([FromQuery] int page, [FromQuery] int size,
        [FromQuery] string? orderBy, [FromQuery] ApiDirection direction = ApiDirection.Asc,
        [FromBody] JsonElement? body = null, CancellationToken ct = default)
    {
        if (page < 0 || size < 1) return BadRequest("Invalid page or size.");
        string? filter = null;
        if (body is { ValueKind: JsonValueKind.String } text) filter = text.GetString();
        else if (body is { ValueKind: JsonValueKind.Object } obj && obj.TryGetProperty("filter", out var property))
        {
            if (property.ValueKind is not (JsonValueKind.String or JsonValueKind.Null)) return BadRequest("Invalid filter.");
            filter = property.ValueKind == JsonValueKind.String ? property.GetString() : null;
        }
        else if (body is { ValueKind: not (JsonValueKind.Null or JsonValueKind.Object) }) return BadRequest("Invalid request body.");
        return Ok(await service.FindAsync(filter, page, size, orderBy, direction, ct));
    }
}
