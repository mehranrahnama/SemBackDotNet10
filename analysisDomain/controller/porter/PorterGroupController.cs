using SortDirection = global::SemBackDotNet10.Common.enums.SortDirection;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SemBackDotNet10.analysisDomain.model.dto.porter;
using SemBackDotNet10.analysisDomain.service.interf.porter;
using SemBackDotNet10.Common.utils;

namespace SemBackDotNet10.analysisDomain.controller.porter;

[ApiController]
[Route("api/v1")]
[Authorize]
public class PorterGroupController : ControllerBase
{
    private readonly IPorterGroupService _porterGroupService;

    public PorterGroupController(IPorterGroupService porterGroupService)
    {
        _porterGroupService = porterGroupService;
    }

    [HttpPost("porterGroup/create")]
    public async Task<IActionResult> CreatePorterGroup([FromBody] PorterGroupDto porterGroupDto)
    {
        var result = await _porterGroupService.CreatePorterGroupAsync(porterGroupDto);
        return result != null ? StatusCode(StatusCodes.Status201Created, result) : StatusCode(StatusCodes.Status304NotModified);
    }

    [HttpPost("porterGroup/getAll/page")]
    public async Task<IActionResult> GetPorterGroupPage([FromQuery] int page, [FromQuery] int size,
        [FromQuery] string? orderBy, [FromQuery] SortDirection? direction, [FromBody] JsonElement? body) =>
        Ok(await _porterGroupService.FindAllPageAsync(page, size, orderBy, direction, body?.GetRawText() ?? "{}"));

    [HttpGet("porterGroup/getAll/{planId:long}")]
    public async Task<IActionResult> GetAllPorterGroup(long planId) =>
        Ok(await _porterGroupService.FindAllPorterGroupsAsync(planId));

    [HttpPut("porterGroup/edit")]
    public async Task<IActionResult> EditPorterGroup([FromBody] PorterGroupDto porterGroupDto) =>
        Ok(await _porterGroupService.EditPorterGroupAsync(porterGroupDto));

    [HttpDelete("porterGroup/delete/{porterGroupId:long}")]
    public async Task<IActionResult> DeletePorterGroup(long porterGroupId)
    {
        await _porterGroupService.DeletePorterGroupAsync(porterGroupId);
        return Ok();
    }
}
