using SortDirection = global::SemBackDotNet10.Common.enums.SortDirection;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SemBackDotNet10.analysisDomain.model.dto.pestel;
using SemBackDotNet10.analysisDomain.service.interf.pestel;
using SemBackDotNet10.Common.utils;

namespace SemBackDotNet10.analysisDomain.controller.pestel;

[ApiController]
[Route("api/v1/pestel")]
[Authorize]
public class PestelController : ControllerBase
{
    private readonly IPestelService _pestelService;

    public PestelController(IPestelService pestelService)
    {
        _pestelService = pestelService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPestelById([FromQuery] long id, CancellationToken ct)
    {
        var result = await _pestelService.GetPestelByIdAsync(id, ct);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateNewPestel([FromBody] List<PestelDto> pestelDto, CancellationToken ct)
    {
        await _pestelService.CreateNewPestelAsync(pestelDto, ct);
        return Ok();
    }

    [HttpPut]
    public async Task<IActionResult> UpdatePestel([FromBody] List<PestelDto> pestelDto, CancellationToken ct)
    {
        await _pestelService.UpdatePestelAsync(pestelDto, ct);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePestel(long id, CancellationToken ct)
    {
        await _pestelService.DeletePestelByIdAsync(id, ct);
        return Ok();
    }

    [HttpPost("page")]
    public async Task<IActionResult> GetAllPestelPageByPestelGroup([FromQuery] int page, [FromQuery] int size,
        [FromQuery] string? orderBy, [FromQuery] SortDirection? direction, [FromBody] JsonElement? body, CancellationToken ct)
    {
        string? filter = null;
        long? pestelGroupId = null;
        if (body is { ValueKind: JsonValueKind.Object } root)
        {
            if (root.TryGetProperty("filter", out var f) && f.ValueKind == JsonValueKind.String)
                filter = f.GetString();
            if (root.TryGetProperty("pestelGroupId", out var g))
            {
                if (g.ValueKind == JsonValueKind.Number)
                    pestelGroupId = g.GetInt64();
                else if (g.ValueKind == JsonValueKind.String && long.TryParse(g.GetString(), out var parsed))
                    pestelGroupId = parsed;
            }
        }
        var result = await _pestelService.GetAllPestelPageByPestelGroupAsync(page, size, orderBy, direction, filter, pestelGroupId, ct);
        return Ok(result);
    }

    [HttpPost("pestelPriority/list")]
    public async Task<IActionResult> GetPestelPriorityListByPestelGroupId([FromBody] JsonElement? body, CancellationToken ct)
    {
        var result = await _pestelService.GetPestelPriorityListByPestelGroupIdAsync(body?.GetRawText() ?? "{}", ct);
        return Ok(result);
    }

    [HttpPut("pestelPriority")]
    public async Task<IActionResult> UpdatePestelPriority([FromBody] List<PestelDto> dtoList, CancellationToken ct)
    {
        await _pestelService.UpdatePestelPriorityAsync(dtoList, ct);
        return Ok();
    }

    [HttpPut("deletePestelPriority")]
    public async Task<IActionResult> DeletePestelPriority([FromBody] List<PestelDto> dtoList, CancellationToken ct)
    {
        await _pestelService.DeletePestelPriorityAsync(dtoList, ct);
        return Ok();
    }
}
