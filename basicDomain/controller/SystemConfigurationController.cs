using SemBackDotNet10.service.impl;
using SortDirection = global::SemBackDotNet10.Common.enums.SortDirection;
using SemBackDotNet10.Common;
using Microsoft.AspNetCore.Mvc;
using SemBackDotNet10.basicDomain.service;

namespace SemBackDotNet10.Controllers;

/// <summary>Port of SystemConfigurationController (base route /api/v1).</summary>
[ApiController]
[Route("api/v1")]
public class SystemConfigurationController : ControllerBase
{
    private readonly SystemConfigurationServiceImpl _service;

    public SystemConfigurationController(SystemConfigurationServiceImpl service) => _service = service;

    [HttpPost("systemConfiguration/page")]
    [CheckAccess]
    public async Task<IActionResult> GetAllSystemConfiguration([FromQuery] int page, [FromQuery] int size,
        [FromQuery] string? orderBy, [FromQuery] SortDirection direction = SortDirection.Asc,
        [FromBody] string? filter = null)
        => Ok(await _service.FindAllAsync(filter ?? "{}", new ServicePageRequest(page, size, orderBy ?? "Id",
            direction == SortDirection.Desc ? SemBackDotNet10.service.impl.SortDirection.Descending : SemBackDotNet10.service.impl.SortDirection.Ascending)));

    [HttpPut("systemConfiguration")]
    [CheckAccess]
    public async Task<IActionResult> Update([FromBody] SystemConfigurationDto dto)
        => Ok(_service.MapToDto(await _service.EditAsync(dto)));

    [HttpDelete("systemConfiguration/{id:long}")]
    [CheckAccess]
    public async Task<IActionResult> Delete(long id) {
        await _service.DeleteAsync(id);
        return Ok();
    }

    [HttpPost("systemConfiguration")]
    [CheckAccess]
    public async Task<IActionResult> Create([FromBody] SystemConfigurationDto dto)
        => Ok(_service.MapToDto(await _service.CreateAsync(dto)));

    [HttpPost("maxExpiredDate")]
    [CheckAccess]
    public IActionResult GetMaxExpiredDate()
        => Ok(new { MaxExpiredDate = DateTime.UtcNow.AddYears(1) });
}
