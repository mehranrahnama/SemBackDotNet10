using SortDirection = global::SemBackDotNet10.Common.enums.SortDirection;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SemBackDotNet10.analysisDomain.model.dto.pestel;
using SemBackDotNet10.analysisDomain.service.interf.pestel;
using SemBackDotNet10.Common.utils;

namespace SemBackDotNet10.analysisDomain.controller.pestel;

[ApiController]
[Route("api/v1")]
[Authorize]
public class PestelScoreController : ControllerBase
{
    private readonly IPestelScoreService _pestelScoreService;

    public PestelScoreController(IPestelScoreService pestelScoreService)
    {
        _pestelScoreService = pestelScoreService;
    }

    [HttpPost("pestelScore/create")]
    public async Task<IActionResult> CreateNewPestelScore([FromBody] PestelScoreDto pestelScoreDto, CancellationToken ct)
    {
        await _pestelScoreService.CreateNewPestelScoreAsync(pestelScoreDto, ct);
        return Ok();
    }

    [HttpDelete("pestelScore/delete/{pestelScoreId}")]
    public async Task<IActionResult> DeletePestelScore(long pestelScoreId, CancellationToken ct)
    {
        await _pestelScoreService.DeletePestelScoreAsync(pestelScoreId, ct);
        return Ok();
    }

    [HttpPut("pestelScore/edit")]
    public async Task<IActionResult> EditPestelScore([FromBody] PestelScoreDto pestelScoreDto, CancellationToken ct)
    {
        await _pestelScoreService.EditPestelScoreAsync(pestelScoreDto, ct);
        return Ok();
    }

    [HttpGet("pestelScore/getAll/{pestelGroupId}")]
    public async Task<IActionResult> GetAllByPestelGroupId(long pestelGroupId, [FromQuery] int page, [FromQuery] int size,
        [FromQuery] SortDirection? direction, [FromQuery] string? orderBy, CancellationToken ct)
    {
        var result = await _pestelScoreService.GetAllByPestelGroupIdAsync(pestelGroupId, page, size, orderBy, direction, ct);
        return Ok(result);
    }

    [HttpPost("pestelScore/getKeyOpportunityByPestelGroup")]
    public async Task<IActionResult> GetKeyOpportunityByPestelGroup([FromBody] JsonElement? body, CancellationToken ct)
    {
        var result = await _pestelScoreService.GetKeyOpportunityByPestelGroupAsync(body?.GetRawText() ?? "{}", ct);
        return Ok(result);
    }

    [HttpPost("pestelScore/getKeyThreatByPestelGroup")]
    public async Task<IActionResult> GetKeyThreatByPestelGroup([FromBody] JsonElement? body, CancellationToken ct)
    {
        var result = await _pestelScoreService.GetKeyThreatByPestelGroupAsync(body?.GetRawText() ?? "{}", ct);
        return Ok(result);
    }
}
