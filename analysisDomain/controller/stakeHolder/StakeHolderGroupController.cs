using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SemBackDotNet10.analysisDomain.model.dto.stakeHolder;
using SemBackDotNet10.analysisDomain.service.interf.stakeHolder;
using SemBackDotNet10.Common.enums;
using SemBackDotNet10.Common.utils;

namespace SemBackDotNet10.analysisDomain.controller.stakeHolder;

[ApiController]
[Route("api/v1/stakeholdergroup")]
[Authorize]
public class StakeHolderGroupController : ControllerBase
{
    private readonly IStakeHolderGroupService _stakeHolderGroupService;

    public StakeHolderGroupController(IStakeHolderGroupService stakeHolderGroupService)
    {
        _stakeHolderGroupService = stakeHolderGroupService;
    }

    [HttpPost("page")]
    public async Task<IActionResult> GetAllStakeHolderGroups([FromQuery] int page, [FromQuery] int size,
        [FromQuery] SortDirection? direction, [FromQuery] string? orderBy, [FromQuery] string? filter,
        [FromBody] JsonElement? body, CancellationToken ct)
    {
        var result = await _stakeHolderGroupService.GetAllStakeHolderGroupsAsync(filter, body?.GetRawText() ?? "{}", page, size, orderBy, direction, ct);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetStakeHolderById([FromQuery] long id, CancellationToken ct)
    {
        var group = await _stakeHolderGroupService.FindByIdOrElseThrowAsync(id, ct);
        return Ok(group);
    }

    [HttpPost]
    public async Task<IActionResult> CreateStakeHolderGroup([FromBody] List<StakeHolderGroupDto> dtoList, CancellationToken ct)
    {
        await _stakeHolderGroupService.CreateStakeHolderGroupsAsync(dtoList, ct);
        return StatusCode(StatusCodes.Status201Created);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteStakeHolderGroupById([FromQuery] long id, CancellationToken ct)
    {
        await _stakeHolderGroupService.DeleteStakeHolderGroupByIdAsync(id, ct);
        return Ok();
    }

    [HttpPut]
    public async Task<IActionResult> UpdateStakeHolderGroupById([FromQuery] long id, [FromBody] StakeHolderGroupDto dto, CancellationToken ct)
    {
        var updated = await _stakeHolderGroupService.UpdateStakeHolderGroupByIdAsync(id, dto, ct);
        return Ok(updated);
    }

    [HttpGet("getAll/{planId}")]
    public async Task<IActionResult> GetAllStakeHolderGroup([FromRoute] long planId, CancellationToken ct)
    {
        var result = await _stakeHolderGroupService.GetAllStakeHolderGroupAsync(planId, ct);
        return Ok(result);
    }
}
