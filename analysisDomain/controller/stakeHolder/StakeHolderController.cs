using SortDirection = global::SemBackDotNet10.Common.enums.SortDirection;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SemBackDotNet10.analysisDomain.model.dto.stakeHolder;
using SemBackDotNet10.analysisDomain.service.impl.resource;
using SemBackDotNet10.analysisDomain.service.interf.stakeHolder;
using SemBackDotNet10.Common.utils;

namespace SemBackDotNet10.analysisDomain.controller.stakeHolder;

[ApiController]
[Route("api/v1/stakeholder")]
[Authorize]
public class StakeHolderController : ControllerBase
{
    private readonly IStakeHolderService _stakeHolderService;

    public StakeHolderController(IStakeHolderService stakeHolderService)
    {
        _stakeHolderService = stakeHolderService;
    }

    [HttpGet]
    public async Task<IActionResult> GetStakeHolderById([FromQuery] long id, CancellationToken ct)
    {
        var stakeHolder = await _stakeHolderService.FindByIdOrElseThrowAsync(id, ct);
        return Ok(stakeHolder);
    }

    [HttpPost("{groupId}")]
    public async Task<IActionResult> GetStakeHolderByGroupId([FromRoute] long groupId,
        [FromQuery] int page, [FromQuery] int size,
        [FromQuery] SortDirection? direction, [FromQuery] string? orderBy,
        [FromQuery] string? filter, [FromQuery] long? planId, CancellationToken ct)
    {
        var result = await _stakeHolderService.FindAllStakeHoldersByStakeHolderGroupIdAsync(groupId, filter, planId, page, size, orderBy, direction, ct);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateStakeHolder([FromBody] List<StakeHolderDto> dtoList, CancellationToken ct)
    {
        await _stakeHolderService.CreateStakeHolderListAsync(dtoList, ct);
        return StatusCode(StatusCodes.Status201Created);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateStakeHolder([FromBody] StakeHolderDto dto, [FromQuery] long id, CancellationToken ct)
    {
        var updated = await _stakeHolderService.UpdateStakeHolderAsync(dto, id, ct);
        return Ok(updated);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteStakeHolderById([FromQuery] long id, CancellationToken ct)
    {
        await _stakeHolderService.DeleteStakeHolderByIdAsync(id, ct);
        return Ok();
    }

    [HttpPost("page")]
    public async Task<IActionResult> GetStakeHolderPage([FromQuery] int page, [FromQuery] int size,
        [FromQuery] string? orderBy, [FromQuery] SortDirection? direction,
        [FromBody] JsonElement? body, CancellationToken ct)
    {
        var json = AnalysisJsonHelper.Parse(body?.GetRawText() ?? "{}");
        var filter = AnalysisJsonHelper.OptString(json, "filter");
        var planId = AnalysisJsonHelper.OptLong(json, "planId");
        var result = await _stakeHolderService.FindAllStakeHoldersByStakeHolderGroupIdAsync(null, filter, planId, page, size, orderBy, direction, ct);
        return Ok(result);
    }

    [HttpPost("getAllWithPriority")]
    public async Task<IActionResult> GetStakeHolderWithPriority([FromBody] JsonElement? body, CancellationToken ct)
    {
        var result = await _stakeHolderService.FindAllStakeHoldersByPlanIdWithPriorityAsync(body?.GetRawText() ?? "{}", ct);
        return Ok(result);
    }

    [HttpPut("updatePowerInterest")]
    public async Task<IActionResult> UpdatePowerInterest([FromBody] StakeHolderDto dto, CancellationToken ct)
    {
        var updated = await _stakeHolderService.UpdatePowerInterestAsync(dto, ct);
        return Ok(updated);
    }

    [HttpPut("deletePowerInterest")]
    public async Task<IActionResult> DeletePowerInterest([FromBody] StakeHolderDto dto, CancellationToken ct)
    {
        var updated = await _stakeHolderService.DeletePowerInterestAsync(dto, ct);
        return Ok(updated);
    }
}
