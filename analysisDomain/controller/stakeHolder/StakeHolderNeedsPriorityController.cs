using SortDirection = global::SemBackDotNet10.Common.enums.SortDirection;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SemBackDotNet10.analysisDomain.model.dto.stakeHolder;
using SemBackDotNet10.analysisDomain.service.interf.stakeHolder;
using SemBackDotNet10.Common.utils;

namespace SemBackDotNet10.analysisDomain.controller.stakeHolder;

[ApiController]
[Route("api/v1/stakeholderneedsPriority")]
[Authorize]
public class StakeHolderNeedsPriorityController : ControllerBase
{
    private readonly IStakeHolderNeedsPriorityService _stakeHolderNeedsPriorityService;

    public StakeHolderNeedsPriorityController(IStakeHolderNeedsPriorityService stakeHolderNeedsPriorityService)
    {
        _stakeHolderNeedsPriorityService = stakeHolderNeedsPriorityService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateNewStakeHolderNeeds([FromBody] List<StakeHolderNeedsPriorityDto> dtoList, CancellationToken ct)
    {
        var result = await _stakeHolderNeedsPriorityService.CreateNewStakeHolderNeedsPriorityAsync(dtoList, ct);
        return Ok(result);
    }

    [HttpPut]
    public async Task<IActionResult> EditStakeHolderNeedsById([FromBody] List<StakeHolderNeedsPriorityDto> dtoList, CancellationToken ct)
    {
        var result = await _stakeHolderNeedsPriorityService.EditStakeHolderNeedsPriorityByIdAsync(dtoList, ct);
        if (result == null)
            return NotFound();
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetStakeHolderNeedsById([FromQuery] long id, CancellationToken ct)
    {
        var result = await _stakeHolderNeedsPriorityService.GetStakeHolderNeedsPriorityByIdAsync(id, ct);
        return Ok(result);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteStakeHolderNeedsById([FromBody] List<long> ids, CancellationToken ct)
    {
        await _stakeHolderNeedsPriorityService.DeleteStakeHolderNeedsPriorityByIdAsync(ids, ct);
        return Ok();
    }

    [HttpPost("list")]
    public async Task<IActionResult> GetStakeHolderNeedsListById([FromBody] JsonElement? body, CancellationToken ct)
    {
        var result = await _stakeHolderNeedsPriorityService.GetStakeHolderNeedsPriorityListAsync(body?.GetRawText() ?? "{}", ct);
        if (result != null)
            return Ok(result);
        return Ok();
    }

    [HttpPost("page")]
    public async Task<IActionResult> GetStakeHolderNeedsPage([FromBody] JsonElement? body,
        [FromQuery] int page, [FromQuery] int size,
        [FromQuery] string? orderBy, [FromQuery] SortDirection? direction, CancellationToken ct)
    {
        var result = await _stakeHolderNeedsPriorityService.GetStakeHolderNeedsPriorityAsync(body?.GetRawText() ?? "{}", page, size, orderBy, direction, ct);
        return Ok(result);
    }

    [HttpPost("answerByStakeHolderGroupId/list")]
    public async Task<IActionResult> GetStakeHolderNeedsAnswerByStakeHolderGroupId([FromBody] JsonElement? body, CancellationToken ct)
    {
        var result = await _stakeHolderNeedsPriorityService.GetStakeHolderNeedsAnswerByStakeHolderGroupIdAsync(body?.GetRawText() ?? "{}", ct);
        return Ok(result);
    }
}
