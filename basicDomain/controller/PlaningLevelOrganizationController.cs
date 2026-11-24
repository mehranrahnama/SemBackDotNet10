using SemBackDotNet10.service.impl;
using Microsoft.AspNetCore.Mvc;
using SortDirection = SemBackDotNet10.Common.enums.SortDirection;

namespace SemBackDotNet10.Controllers;

/// <summary>Port of PlaningLevelOrganizationController (base route /api/v1).</summary>
[ApiController]
[Route("api/v1")]
public class PlaningLevelOrganizationController : ControllerBase
{
    private readonly PlaningLevelOrganizationServiceImpl _service;

    public PlaningLevelOrganizationController(PlaningLevelOrganizationServiceImpl service) => _service = service;

    [HttpPost("PlaningLevelOrganization/page")]
    [CheckAccess]
    public async Task<IActionResult> GetAllPlaningLevelOrganization([FromQuery] int page, [FromQuery] int size,
        [FromQuery] string? orderBy, [FromQuery] SortDirection direction = SortDirection.Asc,
        [FromBody] string? filter = null)
        => Ok(await _service.FindAllAsync(System.Text.Json.JsonSerializer.Serialize(new { filter = filter ?? "" }),
            new ServicePageRequest(page, size, orderBy ?? "Id", direction == SortDirection.Desc
                ? SemBackDotNet10.service.impl.SortDirection.Descending : SemBackDotNet10.service.impl.SortDirection.Ascending)));

    [HttpPut("PlaningLevelOrganization")]
    [CheckAccess]
    public async Task<IActionResult> Update([FromBody] PlaningLevelOrganizationDto dto)
        => Ok(_service.MapToDto(await _service.EditAsync(dto)));

    [HttpDelete("PlaningLevelOrganization/{id:long}")]
    [CheckAccess]
    public async Task<IActionResult> Delete(long id) { await _service.DeleteAsync(id); return Ok(); }

    [HttpPost("PlaningLevelOrganization")]
    [CheckAccess]
    public async Task<IActionResult> Create([FromBody] PlaningLevelOrganizationDto dto)
        => Ok(_service.MapToDto(await _service.CreateAsync(dto)));

    [HttpPost("PlaningLevelOrganization/planingLevelOrganizationsByLevelID")]
    [CheckAccess]
    public async Task<IActionResult> GetByLevelId([FromQuery] int page, [FromQuery] int size,
        [FromQuery] long levelId, [FromBody] string? filter = null)
        => Ok(await _service.FindPlaningLevelOrganizationsByLevelIDAsync(System.Text.Json.JsonSerializer.Serialize(new { levelId, filter = filter ?? "" }),
            new ServicePageRequest(page, size)));
}
