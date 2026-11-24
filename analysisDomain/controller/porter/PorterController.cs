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
public class PorterController : ControllerBase
{
    private readonly IPorterService _porterService;

    public PorterController(IPorterService porterService)
    {
        _porterService = porterService;
    }

    [HttpPost("porter/create")]
    public async Task<IActionResult> CreatePorter([FromBody] PorterDto porterDto)
    {
        var result = await _porterService.CreateNewPorterAsync(porterDto);
        return result != null ? StatusCode(StatusCodes.Status201Created, result) : StatusCode(StatusCodes.Status304NotModified);
    }

    [HttpPost("porter/getAll/page")]
    public async Task<IActionResult> GetPorterPage([FromQuery] int page, [FromQuery] int size,
        [FromQuery] string? orderBy, [FromQuery] SortDirection? direction, [FromBody] JsonElement? body) =>
        Ok(await _porterService.GetAllPorterPageByPorterGroupAsync(page, size, orderBy, direction, body?.GetRawText() ?? "{}"));

    [HttpPost("porter/getAll")]
    public async Task<IActionResult> GetPorter([FromBody] JsonElement? body) =>
        Ok(await _porterService.GetAllPorterByPorterGroupAsync(body?.GetRawText() ?? "{}"));

    [HttpPut("porter/edit")]
    public async Task<IActionResult> EditPorter([FromBody] PorterDto porterDto)
    {
        await _porterService.UpdatePorterAsync(porterDto);
        return Ok();
    }

    [HttpDelete("porter/delete/{porterId:long}")]
    public async Task<IActionResult> DeletePorter(long porterId)
    {
        await _porterService.DeletePorterByIdAsync(porterId);
        return Ok();
    }
}
