using Microsoft.AspNetCore.Mvc;
using SemBackDotNet10.Base.model.dto;
using SortDirection = SemBackDotNet10.Common.enums.SortDirection;

namespace SemBackDotNet10.Controllers;

[ApiController]
[Route("api")]
public class DelegationController(DelegationEndpointService service) : ControllerBase
{
    [HttpPost("v1/delegation")]
    [CheckAccess]
    public async Task<IActionResult> SaveDelegation([FromBody] DelegationDto dto, CancellationToken ct)
        => Ok(await service.SaveAsync(dto, false, ct));

    [HttpPut("v1/delegation")]
    [CheckAccess]
    public async Task<IActionResult> UpdateDelegation([FromBody] DelegationDto dto, CancellationToken ct)
        => Ok(await service.SaveAsync(dto, true, ct));

    [HttpDelete("v1/delegation/{delegationId:long}")]
    [CheckAccess]
    public async Task<IActionResult> DeleteDelegation(long delegationId, CancellationToken ct)
        => await service.DeactivateAsync(delegationId, ct) ? Ok() : StatusCode(417);

    [HttpPost("v1/delegation/page")]
    [CheckAccess]
    public async Task<IActionResult> GetDelegationPage([FromQuery] int page, [FromQuery] int size,
        [FromQuery] string? orderBy, CancellationToken ct, [FromQuery] SortDirection direction = SortDirection.Asc,
        [FromBody] string? filter = null)
    {
        var result = await service.PageAsync(page, size, filter, ct);
        return Ok(new SearchDto.Builder().content(result.Content.ToList()).totalElements(result.TotalElements)
            .totalPages(checked((int)result.TotalPages)).currentPage(page).pageSize(size).build());
    }

    [HttpGet("v1/passiveExpiredDelegation")]
    [CheckAccess]
    public async Task<IActionResult> GetPassiveExpiredDelegation(CancellationToken ct)
        => Ok(await service.DeactivateExpiredAsync(ct));
}
