using SortDirection = global::SemBackDotNet10.Common.enums.SortDirection;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SemBackDotNet10.analysisDomain.model.dto.resource;
using SemBackDotNet10.analysisDomain.service.interf.resource;
using SemBackDotNet10.Common.utils;

namespace SemBackDotNet10.analysisDomain.controller.resource;

[ApiController]
[Route("api/v1")]
[Authorize]
public class ResourceController : ControllerBase
{
    private readonly IResourceService _resourceService;

    public ResourceController(IResourceService resourceService)
    {
        _resourceService = resourceService;
    }

    [HttpPost("resource/create")]
    public async Task<IActionResult> CreateResource([FromBody] ResourceDto resourceDto, CancellationToken ct)
    {
        var result = await _resourceService.CreateNewResourceAsync(resourceDto, ct);
        return result != null ? StatusCode(StatusCodes.Status201Created, result) : StatusCode(StatusCodes.Status304NotModified);
    }

    [HttpPost("resource/getAll/page")]
    public async Task<IActionResult> GetResourcePage([FromQuery] int page, [FromQuery] int size,
        [FromQuery] string? orderBy, [FromQuery] SortDirection? direction,
        [FromBody] JsonElement? body, CancellationToken ct)
    {
        var result = await _resourceService.GetAllResourcePageByLevelIdAsync(page, size, orderBy, direction, body?.GetRawText() ?? "{}", ct);
        return Ok(result);
    }

    [HttpPost("resource/getAll")]
    public async Task<IActionResult> GetResource([FromBody] JsonElement? body, CancellationToken ct)
    {
        var result = await _resourceService.GetAllResourceByLevelIdAsync(body?.GetRawText() ?? "{}", ct);
        return Ok(result);
    }

    [HttpPut("resource/edit")]
    public async Task<IActionResult> EditResource([FromBody] ResourceDto resourceDto, CancellationToken ct)
    {
        await _resourceService.UpdateResourceAsync(resourceDto, ct);
        return Ok();
    }

    [HttpDelete("resource/delete/{resourceId}")]
    public async Task<IActionResult> DeleteResource([FromRoute] long resourceId, CancellationToken ct)
    {
        await _resourceService.DeleteResourceByIdAsync(resourceId, ct);
        return Ok();
    }

    [HttpPut("resource/setCompetency")]
    public async Task<IActionResult> EditResourceCompetency([FromBody] ResourceDto resourceDto, CancellationToken ct)
    {
        await _resourceService.EditResourceCompetencyAsync(resourceDto, ct);
        return Ok();
    }

    [HttpPost("resource/getKeyResource/page")]
    public async Task<IActionResult> GetKeyResourceByPlanId([FromQuery] int page, [FromQuery] int size,
        [FromQuery] string? orderBy, [FromQuery] SortDirection? direction,
        [FromBody] JsonElement? body, CancellationToken ct)
    {
        var result = await _resourceService.GetKeyResourceByPlanIdAsync(page, size, orderBy, direction, body?.GetRawText() ?? "{}", ct);
        return Ok(result);
    }

    [HttpPost("resource/getKeyResource")]
    public async Task<IActionResult> GetKeyResourceByPlanIdList([FromBody] JsonElement? body, CancellationToken ct)
    {
        var result = await _resourceService.GetKeyResourceByPlanIdListAsync(body?.GetRawText() ?? "{}", ct);
        return Ok(result);
    }
}
