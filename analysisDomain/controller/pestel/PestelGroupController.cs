using SortDirection = global::SemBackDotNet10.Common.enums.SortDirection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SemBackDotNet10.analysisDomain.model.dto.pestel;
using SemBackDotNet10.analysisDomain.service.interf.pestel;
using SemBackDotNet10.Common.utils;

namespace SemBackDotNet10.analysisDomain.controller.pestel;

[ApiController]
[Route("api/v1/pestelGroup")]
[Authorize]
public class PestelGroupController : ControllerBase
{
    private readonly IPestelGroupService _pestelGroupService;

    public PestelGroupController(IPestelGroupService pestelGroupService)
    {
        _pestelGroupService = pestelGroupService;
    }

    [HttpGet("getById/{id}")]
    public async Task<IActionResult> GetPestelGroupById(long id, CancellationToken ct)
    {
        var res = await _pestelGroupService.FindByIdOrElseThrowAsync(id, ct);
        return Ok(res);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePestelGroupById(long id, CancellationToken ct)
    {
        await _pestelGroupService.DeletePestelGroupByIdAsync(id, ct);
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> CreatePestelGroup([FromBody] PestelGroupDto pestelGroup, CancellationToken ct)
    {
        var res = await _pestelGroupService.CreateNewPestelGroupAsync(pestelGroup, ct);
        return res != null ? StatusCode(StatusCodes.Status201Created, res) : StatusCode(StatusCodes.Status304NotModified);
    }

    [HttpPut]
    public async Task<IActionResult> EditPestelGroup([FromBody] PestelGroupDto pestelGroup, CancellationToken ct)
    {
        var res = await _pestelGroupService.EditPestelGroupAsync(pestelGroup, ct);
        return Ok(res);
    }

    [HttpPost("all/page")]
    public async Task<IActionResult> GetPestelGroupPage([FromQuery] int page, [FromQuery] int size,
        [FromQuery] string? orderBy, [FromQuery] SortDirection? direction, [FromBody] PestelGroupDto? dto, CancellationToken ct)
    {
        var res = await _pestelGroupService.FindAllPageAsync(page, size, orderBy, direction, dto, ct);
        return Ok(res);
    }

    [HttpGet("all/{planId}")]
    public async Task<IActionResult> GetAllPestelGroup(long planId, CancellationToken ct)
    {
        var res = await _pestelGroupService.FindAllPestelGroupsAsync(planId, ct);
        return Ok(res);
    }
}
