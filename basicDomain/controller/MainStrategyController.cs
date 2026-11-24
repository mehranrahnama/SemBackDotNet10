using SemBackDotNet10.service.impl;
using SemBackDotNet10.Common;
using Microsoft.AspNetCore.Mvc;
using SortDirection = SemBackDotNet10.Common.enums.SortDirection;


namespace SemBackDotNet10.Controllers;

/// <summary>Port of MainStrategyController (base route /api/v1).</summary>
[ApiController]
[Route("api/v1")]
public class MainStrategyController : ControllerBase
{
    private readonly MainStrategyServiceImpl _service;

    public MainStrategyController(MainStrategyServiceImpl service) => _service = service;

    [HttpPost("mainstrategy/list")]
    [CheckAccess]
    public async Task<IActionResult> GetAllMainStrategy([FromQuery] int page, [FromQuery] int size,
        [FromQuery] string? orderBy, [FromQuery] SortDirection direction = SortDirection.Asc,
        [FromQuery] string? filter = null)
        => Ok(await _service.FindAllAsync(filter, new ServicePageRequest(page, size, orderBy ?? "Id", direction == SortDirection.Desc ? SemBackDotNet10.service.impl.SortDirection.Descending : SemBackDotNet10.service.impl.SortDirection.Ascending)));

    [HttpPost("mainstrategyByLevelId/list")]
    [CheckAccess]
    public async Task<IActionResult> GetMainStrategyByLevelId([FromQuery] int page, [FromQuery] int size,
        [FromQuery] long levelId, [FromQuery] string? orderBy,
        [FromQuery] SortDirection direction = SortDirection.Asc)
        => Ok(await _service.FindAllByLevelIdAsync(null, new ServicePageRequest(page, size, orderBy ?? "Id", direction == SortDirection.Desc ? SemBackDotNet10.service.impl.SortDirection.Descending : SemBackDotNet10.service.impl.SortDirection.Ascending), System.Text.Json.JsonSerializer.Serialize(new { levelId })));

    [HttpPost("mainstrategyByLevelIdForRoadMap/list")]
    [CheckAccess]
    public async Task<IActionResult> GetMainStrategyByLevelIdForRoadMap([FromQuery] int page, [FromQuery] int size,
        [FromQuery] long levelId, [FromQuery] string? orderBy,
        [FromQuery] SortDirection direction = SortDirection.Asc)
        => Ok(await _service.FindAllByLevelIdForRoadMapAsync(null, new ServicePageRequest(page, size, orderBy ?? "Id", direction == SortDirection.Desc ? SemBackDotNet10.service.impl.SortDirection.Descending : SemBackDotNet10.service.impl.SortDirection.Ascending), System.Text.Json.JsonSerializer.Serialize(new { levelId })));

    [HttpPost("mainstrategy")]
    [CheckAccess]
    public async Task<IActionResult> CreateMainStrategy([FromBody] MainStrategyDto dto)
        => Ok(await _service.MapToDtoAsync(await _service.MainStrategyCreateAsync(dto)));

    [HttpPut("mainstrategy")]
    [CheckAccess]
    public async Task<IActionResult> UpdateMainStrategy([FromBody] MainStrategyDto dto)
        => Ok(await _service.MapToDtoAsync(await _service.MainStrategyEditAsync(dto)));

    [HttpDelete("mainstrategy/{id:long}")]
    [CheckAccess]
    public async Task<IActionResult> DeleteMainStrategy(long id)
        { await _service.DeleteByIdAsync(id); return Ok(); }
}
