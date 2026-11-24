using Microsoft.AspNetCore.Mvc;
using SemBackDotNet10.Common;
using SemBackDotNet10.Controllers;
using SemBackDotNet10.measureDomain.controller.dto.measureDtos;
using SemBackDotNet10.service.impl;
using ServiceDirection = SemBackDotNet10.service.impl.SortDirection;
using SortDirection = SemBackDotNet10.Common.enums.SortDirection;

namespace SemBackDotNet10.measureDomain.controller;

[ApiController]
[Route("api/v1")]
public sealed class MeasureObjectGridController(MeasureObjectGridService service, MeasureRepositoryGridService repositories, MeasureDetailsService details, MeasureRepositoryEditService editor, MeasureCountsService counts, MeasureByObjectPageService measuresByObject) : ControllerBase
{
    [HttpPost("measure-by-object/page")]
    [CheckAccess]
    public async Task<IActionResult> GetMeasuresByObject([FromBody] MeasureByObjectInput body, [FromQuery] int page,
        [FromQuery] int size, [FromQuery] string? orderBy, [FromQuery] SortDirection? direction, CancellationToken ct)
    {
        if (page < 0 || size <= 0 || body.LevelId is null || (long)page * Math.Min(size, 200) > int.MaxValue)
            return BadRequest(new { message = "LevelId and valid page and size parameters are required." });
        var hasSort = direction.HasValue && !string.IsNullOrWhiteSpace(orderBy);
        var request = new ServicePageRequest(page, Math.Min(size, 200), hasSort ? orderBy! : "Id",
            hasSort && direction == SortDirection.Desc ? ServiceDirection.Descending : ServiceDirection.Ascending);
        return Ok(await measuresByObject.FindAsync(body, request, ct));
    }

    [HttpPost("measure/counts")]
    [CheckAccess]
    public async Task<IActionResult> GetCounts([FromBody] MeasureCountsInput body, CancellationToken ct)
        => Ok(await counts.GetAsync(body, ct));

    [HttpPut("measureRepository")]
    [CheckAccess]
    public async Task<IActionResult> EditRepository([FromBody] SemBackDotNet10.measureDomain.model.dto.MeasureRepositoryDto dto, CancellationToken ct)
    {
        try { return Ok(await editor.EditAsync(dto, ct)); }
        catch (ArgumentException exception) { return BadRequest(new { error = "INVALID_MEASURE_REPOSITORY", message = exception.Message }); }
    }

    [HttpGet("measureByMeasureCatalogId/{planId:long}/{catalogId:long}")]
    [CheckAccess]
    public async Task<IActionResult> GetMeasureByCatalog(long planId, long catalogId, CancellationToken ct)
        => Ok(await details.FindByCatalogAsync(planId, catalogId, ct));

    [HttpPost("measure/object/page")]
    [CheckAccess]
    public async Task<IActionResult> GetPage([FromBody] LoadStrategyObjectDto body, [FromQuery] int page,
        [FromQuery] int size, [FromQuery] string? orderBy, [FromQuery] SortDirection? direction,
        CancellationToken ct)
    {
        var request = new ServicePageRequest(page, Math.Min(size, 200), string.IsNullOrWhiteSpace(orderBy) ? "Id" : orderBy,
            direction == SortDirection.Desc ? ServiceDirection.Descending : ServiceDirection.Ascending);
        return Ok(await service.FindAsync(body, request, ct));
    }

    [HttpPost("measureRepository/page")]
    [CheckAccess]
    public async Task<IActionResult> GetRepositories([FromBody] MeasureRepositoryPageInput body, [FromQuery] int page,
        [FromQuery] int size, [FromQuery] string? orderBy, [FromQuery] SortDirection? direction,
        CancellationToken ct)
    {
        var request = new ServicePageRequest(page, Math.Min(size, 200), string.IsNullOrWhiteSpace(orderBy) ? "Id" : orderBy,
            direction == SortDirection.Desc ? ServiceDirection.Descending : ServiceDirection.Ascending);
        return Ok(await repositories.FindAsync(body, request, ct));
    }
}
