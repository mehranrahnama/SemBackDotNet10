using SemBackDotNet10.service.impl;
using Microsoft.AspNetCore.Mvc;
using SortDirection = SemBackDotNet10.Common.enums.SortDirection;

namespace SemBackDotNet10.Controllers;

/// <summary>Port of PlaningLevelTeamController (base route /api/v1).</summary>
[ApiController]
[Route("api/v1")]
public class PlaningLevelTeamController : ControllerBase
{
    private readonly PlaningLevelTeamServiceImpl _service;

    public PlaningLevelTeamController(PlaningLevelTeamServiceImpl service) => _service = service;

    [HttpPost("planingLevelTeam/page")]
    [CheckAccess]
    public async Task<IActionResult> GetAllPlaningLevelTeam([FromQuery] int page, [FromQuery] int size,
        [FromQuery] string? orderBy, [FromQuery] SortDirection direction = SortDirection.Asc,
        [FromBody] string? filter = null)
        => Ok(await _service.FindAllAsync(System.Text.Json.JsonSerializer.Serialize(new { filter = filter ?? "" }),
            new ServicePageRequest(page, size, orderBy ?? "Id", direction == SortDirection.Desc
                ? SemBackDotNet10.service.impl.SortDirection.Descending : SemBackDotNet10.service.impl.SortDirection.Ascending)));

    [HttpPost("planingLevelTeam/by-level/page")]
    [CheckAccess]
    public async Task<IActionResult> GetPlaningLevelTeamByLevel([FromQuery] int page, [FromQuery] int size,
        [FromQuery] long levelId, [FromBody] string? filter = null)
        => Ok(await _service.FindAllByLevelIdAsync(System.Text.Json.JsonSerializer.Serialize(new { levelId, filter = filter ?? "" }),
            new ServicePageRequest(page, size)));

    [HttpPut("planingLevelTeam")]
    [CheckAccess]
    public async Task<IActionResult> Update([FromBody] PlaningLevelTeamDto dto)
        => Ok(_service.MapToDto(await _service.EditAsync(dto)));

    [HttpDelete("planingLevelTeam/{id:long}")]
    [CheckAccess]
    public async Task<IActionResult> Delete(long id) { await _service.DeleteAsync(id); return Ok(); }

    [HttpPost("planingLevelTeam")]
    [CheckAccess]
    public async Task<IActionResult> Create([FromBody] PlaningLevelTeamDto dto)
        => Ok(_service.MapToDto(await _service.CreateAsync(dto)));
}
