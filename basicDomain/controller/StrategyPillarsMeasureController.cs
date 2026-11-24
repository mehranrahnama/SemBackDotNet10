using Microsoft.AspNetCore.Mvc;
using SemBackDotNet10.service.impl;

namespace SemBackDotNet10.Controllers;

[ApiController]
[Route("api/v1")]
public class StrategyPillarsMeasureController(StrategyPillarsMeasureServiceImpl service) : ControllerBase
{
    [HttpPost("strategypillarsmeasure")]
    [CheckAccess]
    public async Task<IActionResult> Create([FromBody] List<StrategyPillarsMeasureDto> rows, CancellationToken ct)
    {
        await service.SaveAsync(rows, false, ct);
        return StatusCode(201);
    }

    [HttpGet("strategypillarsmeasure/page/{strategyPillarsId:long}")]
    [CheckAccess]
    public async Task<IActionResult> GetPage(long strategyPillarsId, [FromQuery] int page, [FromQuery] int size,
        [FromQuery] string? filter, CancellationToken ct)
        => Ok(await service.PageAsync(strategyPillarsId, filter, page, size, ct));

    [HttpGet("strategypillarsmeasure/{id:long}")]
    [CheckAccess]
    public async Task<IActionResult> GetById(long id, CancellationToken ct) => Ok(await service.GetAsync(id, ct));

    [HttpPut("strategypillarsmeasure")]
    [CheckAccess]
    public async Task<IActionResult> Update([FromBody] List<StrategyPillarsMeasureDto> rows, CancellationToken ct)
    {
        await service.SaveAsync(rows, true, ct);
        return Ok();
    }

    [HttpDelete("strategypillarsmeasure")]
    [CheckAccess]
    public async Task<IActionResult> Delete([FromQuery] string ids, CancellationToken ct)
    {
        await service.SoftDeleteAsync(ids, ct);
        return Ok();
    }
}
