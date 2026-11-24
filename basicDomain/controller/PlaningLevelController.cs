using Microsoft.AspNetCore.Mvc;
using SemBackDotNet10.service.impl;
using global::Common.webServices;
using SortDirection = SemBackDotNet10.Common.enums.SortDirection;
using ServiceSortDirection = SemBackDotNet10.service.impl.SortDirection;

namespace SemBackDotNet10.Controllers;

[ApiController]
[Route("api/v1")]
[CheckAccess]
public class PlaningLevelController(PlaningLevelServiceImpl service) : ControllerBase
{
    public sealed record CurrentLevelFilter(string? Filter, string? SrcType, long? ParentLevelId);

    private static ServicePageRequest PageRequest(int page, int size, string? orderBy, SortDirection? direction)
        => new(page, Math.Min(size, 200), direction is null || string.IsNullOrWhiteSpace(orderBy) ? "Id" : orderBy,
            direction == SortDirection.Desc && !string.IsNullOrWhiteSpace(orderBy) ? ServiceSortDirection.Descending : ServiceSortDirection.Ascending);

    private IActionResult PageResponse(ServicePage<PlaningLevelDto> result, int page, int size)
        => Ok(new { result.Content, result.TotalElements, result.TotalPages, CurrentPage = page, result.PageSize });

    [HttpPost("planingLevel/page")]
    public async Task<IActionResult> GetAllPlaningLevel([FromQuery] int page, [FromQuery] int size,
        [FromQuery] string? orderBy, [FromQuery] SortDirection? direction, [FromBody] string? filter, CancellationToken ct)
        => PageResponse(await service.FindAllAsync(filter, PageRequest(page, size, orderBy, direction), ct), page, size);

    [HttpPost("planingLevel/by-user/page")]
    [HttpPost("planingLevel/by-user/page/forMenu")]
    public async Task<IActionResult> GetPlaningLevelByUser([FromQuery] int page, [FromQuery] int size,
        [FromQuery] string? orderBy, [FromQuery] SortDirection? direction, [FromBody] CurrentLevelFilter body, CancellationToken ct)
        => PageResponse(await service.GetCurrentUserLevelsAsync(body.Filter, body.SrcType ?? "", PageRequest(page, size, orderBy, direction), ct), page, size);

    [HttpPost("planingLevel/by-user-parentLevelId/page")]
    public async Task<IActionResult> GetPlaningLevelByUserAndParent([FromQuery] int page, [FromQuery] int size,
        [FromQuery] string? orderBy, [FromQuery] SortDirection? direction, [FromBody] CurrentLevelFilter body, CancellationToken ct)
        => PageResponse(await service.GetCurrentUserLevelsByParentLevelIdAsync(body.Filter, body.SrcType ?? "", body.ParentLevelId,
            PageRequest(page, size, orderBy, direction), ct), page, size);

    [HttpGet("planingLevel/by-selectedUser/{userId:long}")]
    public async Task<IActionResult> GetBySelectedUser(long userId, CancellationToken ct)
        => Ok(await service.GetPlaningLevelsBySelectedUserTreeAsync(userId, ct));

    [HttpGet("planingLevel/planingLevelTree")]
    public async Task<IActionResult> GetPlaningLevelTree(CancellationToken ct)
        => Ok(await service.GetPlaningLevelTreeAsync(ct));

    [HttpPut("planingLevel")]
    public async Task<IActionResult> UpdatePlaningLevel([FromBody] PlaningLevelDto dto, CancellationToken ct)
        => Ok(service.MapToDto(await service.EditAsync(dto, ct)));

    [HttpDelete("planingLevel/{id:long}")]
    public async Task<IActionResult> DeletePlaningLevel(long id, CancellationToken ct)
    {
        await service.DeleteAsync(id, ct);
        return Ok();
    }

    [HttpPost("planingLevel")]
    public async Task<IActionResult> CreatePlaningLevel([FromBody] PlaningLevelDto dto, CancellationToken ct)
        => Ok(service.MapToDto(await service.CreateAsync(dto, ct)));

    [HttpGet("planingLevel/SupremeCommittee/page")]
    public async Task<IActionResult> GetSupremeCommittee([FromQuery] int page, [FromQuery] int size,
        [FromQuery] string? orderBy, [FromQuery] SortDirection? direction, CancellationToken ct)
        => PageResponse(await service.GetSupremeCommitteeAsync(PageRequest(page, size, orderBy, direction), ct), page, size);

    [HttpGet("planingLevel/CommitteeTahavol")]
    public async Task<IActionResult> GetCommitteeTahavol([FromServices] CommitteeTahavolService committees, CancellationToken ct)
        => Content((await committees.GetCommitteeTahavol(ct)).ToJsonString(), "application/json");

    [HttpPost("planingLevel/insertNewCommitteeTahavol")]
    public async Task<IActionResult> InsertNewCommitteeTahavol(CancellationToken ct)
    {
        await service.InsertNewCommitteeTahavolAsync(ct);
        return Ok();
    }
}
