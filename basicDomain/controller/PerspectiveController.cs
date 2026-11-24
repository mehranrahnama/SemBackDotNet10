using System.Text.Json.Serialization;
using Common.exception;
using Microsoft.AspNetCore.Mvc;
using SemBackDotNet10.basicDomain.Model.Dto;
using SortDirection = SemBackDotNet10.Common.enums.SortDirection;
using ServiceSortDirection = SemBackDotNet10.service.impl.SortDirection;
using SemBackDotNet10.service.impl;

namespace SemBackDotNet10.Controllers;

[ApiController]
[Route("api/v1")]
public class PerspectiveController(PerspectiveServiceImpl service) : ControllerBase
{
    [HttpPost("perspective")]
    [CheckAccess]
    public async Task<IActionResult> SaveGapSrv([FromBody] PerspectiveDto dto, CancellationToken ct)
        => Ok(service.MapToDto(await service.SaveAsync(dto, ct)));

    [HttpPut("perspective")]
    [CheckAccess]
    public async Task<IActionResult> Edit([FromBody] PerspectiveDto dto, CancellationToken ct)
        => Ok(service.MapToDto(await service.EditAsync(dto, ct)));

    [HttpDelete("perspective/{id:long}")]
    [CheckAccess]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
    {
        await service.DeletePerspectiveAsync(id, ct);
        return Ok();
    }

    [HttpPost("perspective/page")]
    [HttpPost("v1/cartableRoyalVision/page")]
    [CheckAccess]
    public async Task<IActionResult> GetPerspectivePage([FromQuery] int page, [FromQuery] int size,
        [FromQuery] string? orderBy, [FromBody] PageInput? body, CancellationToken ct,
        [FromQuery] SortDirection direction = SortDirection.Asc)
    {
        if (body?.LevelId is not long levelId || body.PlanId is not long planId)
            throw new BusinessException("error.perspective.find");

        var request = new ServicePageRequest(page, size, orderBy ?? "Id",
            direction == SortDirection.Desc ? ServiceSortDirection.Descending : ServiceSortDirection.Ascending);
        return Ok(await service.FindAllAsync(levelId, planId, body.Filter, request, ct));
    }

    [HttpPost("perspective/StrategyMapData")]
    [CheckAccess]
    public async Task<IActionResult> GetStrategyMapData([FromBody] ReportInput body, CancellationToken ct)
        => Ok(await service.StrategyMapReportServiceAsync(body.PlanId, body.LevelId, body.Month, ct));

    [HttpPost("perspective/StrategyMapDashboardData")]
    [CheckAccess]
    public async Task<IActionResult> GetStrategyMapDashboardData([FromBody] ReportInput body, CancellationToken ct)
        => Ok(await service.StrategyMapDashboardReportServiceAsync(body.PlanId, body.LevelId, body.Month, ct));

    [HttpPost("perspective/ReportMeasureDataByStrategyObjectIdData")]
    [CheckAccess]
    public async Task<IActionResult> GetReportMeasureData([FromBody] ReportInput body, CancellationToken ct)
        => Ok(await service.ReportMeasureDataByStrategyObjectIdControllerAsync(body.StrategyObjectId, body.PlanId, body.LevelId, body.Month, ct));

    [HttpPost("perspective/ReportMeasureDataByStrategyObjectIdAllMonthData")]
    [CheckAccess]
    public async Task<IActionResult> GetReportMeasureDataAllMonth([FromBody] ReportInput body, CancellationToken ct)
        => Ok(await service.ReportMeasureDataByStrategyObjectIdAllMonthControllerAsync(body.StrategyObjectId, body.PlanId, body.LevelId, ct));

    [HttpPost("perspective/ReportActionDataByStrategyObjectIdData")]
    [CheckAccess]
    public async Task<IActionResult> GetReportActionData([FromBody] ReportInput body, CancellationToken ct)
        => Ok(await service.ReportActionDataByStrategyObjectIdControllerAsync(body.StrategyObjectId, body.PlanId, body.LevelId, body.Month, ct));

    [HttpPost("perspective/ReportActionDataByStrategyObjectIdAllMonthData")]
    [CheckAccess]
    public async Task<IActionResult> GetReportActionDataAllMonth([FromBody] ReportInput body, CancellationToken ct)
        => Ok(await service.ReportActionDataByStrategyObjectIdAllMonthControllerAsync(body.StrategyObjectId, body.PlanId, body.LevelId, ct));

    [HttpPut("perspective/saveWeights")]
    [CheckAccess]
    public async Task<IActionResult> SaveWeights([FromBody] List<PerspectiveDto> weights, CancellationToken ct)
    {
        await service.EditAllWeightsAsync(weights, ct);
        return Ok();
    }

    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public sealed record PageInput(long? LevelId, long? PlanId, string? Filter);

    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public sealed record ReportInput(long? StrategyObjectId, long? PlanId, long? LevelId, long? Month);
}
