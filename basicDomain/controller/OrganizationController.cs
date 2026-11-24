using SemBackDotNet10.service.impl;
using SemBackDotNet10.Common;
using SemBackDotNet10.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using SortDirection = SemBackDotNet10.Common.enums.SortDirection;

namespace SemBackDotNet10.Controllers;

/// <summary>Port of OrganizationController (base route /api/v1).</summary>
[ApiController]
[Route("api/v1")]
public class OrganizationController : ControllerBase
{
    private readonly OrganizationServiceImpl _service;

    public OrganizationController(OrganizationServiceImpl service) => _service = service;

    [HttpGet("organization/list")]
    [CheckAccess]
    public async Task<IActionResult> GetAllPosition([FromQuery] int page, [FromQuery] int size,
        [FromQuery] string? orderBy, [FromQuery] SortDirection direction = SortDirection.Asc,
        [FromQuery] string? filter = null)
        => Ok(await _service.FindAllAsync(filter, new ServicePageRequest(page, size, orderBy ?? "Id", direction == SortDirection.Desc ? SemBackDotNet10.service.impl.SortDirection.Descending : SemBackDotNet10.service.impl.SortDirection.Ascending)));

    [HttpPost("organization/listWithModirInfo")]
    [CheckAccess]
    public async Task<IActionResult> GetListWithModirInfo([FromQuery] int page, [FromQuery] int size,
        [FromQuery] string? orderBy, [FromQuery] SortDirection direction = SortDirection.Asc,
        [FromBody] string? filter = null)
        => Ok(await _service.FindAllPositionlistWithModirInfoAsync(filter ?? "{}", new ServicePageRequest(page, size, orderBy ?? "Id", direction == SortDirection.Desc ? SemBackDotNet10.service.impl.SortDirection.Descending : SemBackDotNet10.service.impl.SortDirection.Ascending)));

    [HttpPost("organization")]
    [CheckAccess]
    public async Task<IActionResult> CreateOrganization([FromBody] OrganizationDto dto)
        => Ok(await _service.CreateOrganizationAsync(dto));

    [HttpPost("organization/create")]
    [CheckAccess]
    public async Task<IActionResult> CreateOrganizationExplicit([FromBody] OrganizationDto dto)
        => Ok(await _service.CreateOrganizationAsync(dto));

    [HttpPut("organization/edit")]
    [CheckAccess]
    public async Task<IActionResult> EditOrganization([FromBody] OrganizationDto dto)
        { await _service.EditOrganizationAsync(dto); return Ok(); }

    [HttpDelete("organization/delete/{organizationId:long}")]
    [CheckAccess]
    public async Task<IActionResult> DeleteOrganization(long organizationId)
        { await _service.DeleteOrganizationAsync(organizationId); return Ok(); }
}
