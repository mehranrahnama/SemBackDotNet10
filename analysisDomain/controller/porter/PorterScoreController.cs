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
public class PorterScoreController : ControllerBase
{
    private readonly IPorterScoreService _porterScoreService;

    public PorterScoreController(IPorterScoreService porterScoreService)
    {
        _porterScoreService = porterScoreService;
    }

    [HttpPost("porterScore/create")]
    public async Task<IActionResult> CreateNewPorterScore([FromBody] List<PorterScoreDto> porterScoreDto)
    {
        await _porterScoreService.CreateNewPorterScoreAsync(porterScoreDto);
        return Ok();
    }

    [HttpDelete("porterScore/delete/{porterScoreId:long}")]
    public async Task<IActionResult> DeletePorterScore(long porterScoreId)
    {
        await _porterScoreService.DeletePorterScoreAsync(porterScoreId);
        return Ok();
    }

    [HttpPut("porterScore/edit")]
    public async Task<IActionResult> EditPorterScore([FromBody] List<PorterScoreDto> porterScoreDto)
    {
        await _porterScoreService.EditPorterScoreAsync(porterScoreDto);
        return Ok();
    }

    [HttpPut("porterScore/editScore")]
    public async Task<IActionResult> EditScore([FromBody] List<PorterScoreDto> porterScoreDto)
    {
        await _porterScoreService.EditScoreAsync(porterScoreDto);
        return Ok();
    }

    [HttpGet("porterScore/getAllPage/{porterGroupId:long}")]
    public async Task<IActionResult> GetAllPageByPorterGroupId(long porterGroupId, [FromQuery] int page,
        [FromQuery] int size, [FromQuery] SortDirection? direction, [FromQuery] string? orderBy) =>
        Ok(await _porterScoreService.GetAllByPorterGroupIdAsync(porterGroupId, page, size, orderBy, direction));

    [HttpGet("porterScore/getAll/{porterGroupId:long}")]
    public async Task<IActionResult> GetAllByPorterGroupId(long porterGroupId) =>
        Ok(await _porterScoreService.GetAllByPorterGroupIdAsync(porterGroupId));

    [HttpPost("porterScore/getAll")]
    public async Task<IActionResult> GetAllByPorterGroupAndProductAndMarket([FromBody] JsonElement? body) =>
        Ok(await _porterScoreService.GetAllByPorterGroupAndProductAndMarketAsync(body?.GetRawText() ?? "{}"));

    [HttpPost("porterScore/getKeyOpportunityByPorterGroup")]
    public async Task<IActionResult> GetKeyOpportunityByPorterGroup([FromBody] JsonElement? body) =>
        Ok(await _porterScoreService.GetKeyOpportunityByPorterGroupAsync(body?.GetRawText() ?? "{}"));

    [HttpPost("porterScore/getKeyThreatByPorterGroup")]
    public async Task<IActionResult> GetKeyThreatByPorterGroup([FromBody] JsonElement? body) =>
        Ok(await _porterScoreService.GetKeyThreatByPorterGroupAsync(body?.GetRawText() ?? "{}"));
}
