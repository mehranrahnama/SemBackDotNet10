using global::SemBackDotNet10.webserviceDomain.service.interf;
using SortDirection = global::SemBackDotNet10.Common.enums.SortDirection;
using SemBackDotNet10.Common;
using Microsoft.AspNetCore.Mvc;
using SemBackDotNet10.service.impl;
using StrategyObjectDto = SemBackDotNet10.basicDomain.Model.Dto.StrategyObjectDto;
using ServiceSortDirection = SemBackDotNet10.service.impl.SortDirection;
using System.Text.Json.Serialization;

namespace SemBackDotNet10.Controllers;

/// <summary>Port of StrategyObjectController (base route /api/v1).</summary>
[ApiController]
[Route("api/v1")]
public class StrategyObjectController : ControllerBase
{
    private readonly IServiceProvider _services;
    private readonly StrategyObjectReadService _readService;
    private StrategyObjectImpl _service => _services.GetRequiredService<StrategyObjectImpl>();
    private IWebServicesService _webServicesService => _services.GetRequiredService<IWebServicesService>();
    public StrategyObjectController(
        IServiceProvider services,
        StrategyObjectReadService readService)
    {
        _services = services;
        _readService = readService;
    }

    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public sealed record PerspectiveFilter(long LevelId, long PlanId, long PerspectId, string? Filter);
    public sealed record PlanFilter(long LevelId, long PlanId);

    private static ServicePageRequest Page(int page, int size, string? orderBy, SortDirection? direction)
        => new(page, Math.Min(size, 200), direction is null || string.IsNullOrWhiteSpace(orderBy) ? "Id" : orderBy,
            direction == SortDirection.Desc && !string.IsNullOrWhiteSpace(orderBy) ? ServiceSortDirection.Descending : ServiceSortDirection.Ascending);

    [HttpPost("strategyobject/page")]
    [HttpPost("strategyobject/page/forMenu")]
    [CheckAccess]
    public async Task<IActionResult> GetStrategyObjectByLevelId([FromBody] LoadStrategyObjectListDto body,
        [FromQuery] int page, [FromQuery] int size, [FromQuery] string? orderBy,
        [FromQuery] SortDirection? direction, CancellationToken ct)
        => Ok(await _readService.FindAsync(body.LevelId, body.PlanId, null, body.Filter, Page(page, size, orderBy, direction), ct));

    [HttpPost("strategyobject/list-with-perspective")]
    [CheckAccess]
    public async Task<IActionResult> GetWithPerspective([FromBody] LoadStrategyObjectListDto body, CancellationToken ct)
        => Ok(await _service.FindAllWithPerspectiveAndThemeAsync(body.LevelId, body.PlanId, body.Filter, ct));

    [HttpPost("strategyobjectByPerspectId/page")]
    [CheckAccess]
    public async Task<IActionResult> GetStrategyObjectByPerspectiveId([FromBody] PerspectiveFilter body,
        [FromQuery] int page, [FromQuery] int size, [FromQuery] string? orderBy,
        [FromQuery] SortDirection? direction, CancellationToken ct)
        => Ok(await _readService.FindAsync(body.LevelId, body.PlanId, body.PerspectId,
            body.Filter, Page(page, size, orderBy, direction), ct));

    [HttpPost("strategyobjectByLevelIdAndPlanId/list")]
    [CheckAccess]
    public async Task<IActionResult> GetByLevelIdAndPlanId([FromBody] PlanFilter body, CancellationToken ct)
        => Ok(await _service.FindAllByLevelIdAndPlanIdAsync(body.LevelId, body.PlanId, ct));

    [HttpPost("strategyobject/list")]
    [CheckAccess]
    public async Task<IActionResult> GetList([FromQuery] int page, [FromQuery] int size, [FromQuery] string? orderBy,
        [FromQuery] SortDirection? direction, [FromBody] string? filter, CancellationToken ct)
        => Ok(await _service.FindAllAsync(filter, Page(page, size, orderBy, direction), ct));

    [HttpPost("strategyobject")]
    [CheckAccess]
    public async Task<IActionResult> Create([FromBody] StrategyObjectDto dto, CancellationToken ct)
        => Ok(await _service.MapToDtoAsync(await _service.StrategyObjectCreateAsync(dto, ct), ct));

    [HttpPut("strategyobject")]
    [CheckAccess]
    public async Task<IActionResult> Update([FromBody] StrategyObjectDto dto, CancellationToken ct)
        => Ok(await _service.MapToDtoAsync(await _service.StrategyObjectEditAsync(dto, ct), ct));

    [HttpDelete("strategyobject/{id:long}")]
    [CheckAccess]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
    {
        await _service.DeleteByIdAsync(id, ct);
        return Ok();
    }

    [HttpGet("strategyObjectCount/{planId:long}")]
    [CheckAccess]
    public async Task<IActionResult> GetStrategyObjectCount(long planId, CancellationToken ct)
        => Ok(await _service.StrategyObjectCountAsync(planId, ct));

    [HttpGet("parentStrategyObject/{levelId:long}")]
    [CheckAccess]
    public async Task<IActionResult> GetParentStrategyObject(long levelId, CancellationToken ct)
        => Ok(await _service.GetParentStrategyObjAsync(levelId, ct));

    [HttpGet("strategyObject/createJson/{levelId:long}/{planId:long}")]
    [CheckAccess]
    public async Task<IActionResult> CreateJson(long levelId, long planId,
        [FromServices] StrategyObjectConvertService converter, CancellationToken ct)
        => Ok(await converter.CreateJsonFileAsync(levelId, planId, ct));

    [HttpPost("strategyObject/insertJsonToTable/{jsonFileName}")]
    [CheckAccess]
    public async Task<IActionResult> InsertJsonToTable(string jsonFileName,
        [FromServices] StrategyObjectConvertService converter, CancellationToken ct)
    {
        await converter.ImportJsonFileAsync(jsonFileName, ct);
        return Ok();
    }

    [HttpGet("strategyobject/objectFunctionality")]
    [CheckAccess]
    public async Task<IActionResult> GetObjectFunctionality(CancellationToken ct)
    {
        var result = await _webServicesService.GetObjectFunctionalityAsync(ct);
        return Ok(new { objectFunctionality = result, kpi = result, subtitle = "میزان تحقق اهداف ماه جاری" });
    }
}
