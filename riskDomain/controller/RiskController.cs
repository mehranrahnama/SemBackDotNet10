using SortDirection = global::SemBackDotNet10.Common.enums.SortDirection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SemBackDotNet10.riskDomain.model.dto;
using SemBackDotNet10.riskDomain.service.interf;

namespace SemBackDotNet10.riskDomain.controller;

[ApiController]
[Route("api/v1/risks")]
[Authorize]
public class RiskController : ControllerBase
{
    private readonly IRiskService _riskService;
    private readonly IRiskActionService _riskActionService;
    private readonly IRiskMeasureService _riskMeasureService;

    public RiskController(IRiskService riskService, IRiskActionService riskActionService, IRiskMeasureService riskMeasureService)
    {
        _riskService = riskService;
        _riskActionService = riskActionService;
        _riskMeasureService = riskMeasureService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateRisk([FromBody] RiskDto riskDto, CancellationToken ct)
    {
        if (riskDto == null) return BadRequest(new { message = "اطلاعات ریسک نامعتبر است." });
        await _riskService.CreateRiskAsync(riskDto, ct);
        return StatusCode(StatusCodes.Status201Created);
    }

    [HttpGet("filter-query")]
    public async Task<IActionResult> GetStrategyObjectRiskByPlanIdAndLevelId(
        [FromQuery] int page, [FromQuery] int size, [FromQuery] string? filter,
        [FromQuery] string? orderBy, [FromQuery] SortDirection? direction, CancellationToken ct)
    {
        // خواندن ایمن بدنه با محدودیت و اعتبارسنجی (پیشگیری از DoS)
        using var reader = new StreamReader(Request.Body);
        var body = await reader.ReadToEndAsync(ct);

        var result = await _riskService.GetRiskByPlanIdAndLevelIdAsync(body, page, size, filter, orderBy, direction, ct);
        return Ok(result);
    }

    [HttpPut]
    public async Task<IActionResult> EditRisk([FromBody] RiskDto riskDto, CancellationToken ct)
    {
        if (riskDto == null) return BadRequest(new { message = "اطلاعات ریسک نامعتبر است." });
        await _riskService.EditRiskAsync(riskDto, ct);
        return NoContent();
    }

    [HttpDelete("{riskId:long}")]
    public async Task<IActionResult> DeleteRisk(long riskId, CancellationToken ct)
    {
        await _riskService.DeleteRiskAsync(riskId, ct);
        return NoContent();
    }
}
