using SemBackDotNet10.service.impl;
using SemBackDotNet10.Common;
using Microsoft.AspNetCore.Mvc;
using SemBackDotNet10.basicDomain.service;

namespace SemBackDotNet10.Controllers;

/// <summary>Port of StrategyPillarsController (base route /api/v1).</summary>
[ApiController]
[Route("api/v1")]
public class StrategyPillarsController : ControllerBase
{
    private readonly StrategyPillarServiceImpl _service;

    public StrategyPillarsController(StrategyPillarServiceImpl service) => _service = service;

    [HttpGet("getStrategyPillarDataaByPlanIdAndLevelId/{planId:long}/{levelId:long}")]
    [CheckAccess]
    public async Task<IActionResult> GetByPlanIdAndLevelId(long planId, long levelId)
        => Ok(_service.MapToDto(await _service.FindStrategyPillarDataByPlanIdAndLevelIdAsync(planId, levelId)));

    [HttpGet("getStrategyPillarDataaByPlanIdAndLevelIdForRoadMap/{planId:long}/{levelId:long}")]
    [CheckAccess]
    public async Task<IActionResult> GetByPlanIdAndLevelIdForRoadMap(long planId, long levelId)
        => Ok(_service.MapToDto(await _service.FindStrategyPillarDataByPlanIdAndLevelIdForRoadMapAsync(planId, levelId)));

    [HttpGet("getStrategyPillarDataaByPlanId/{planId:long}")]
    [CheckAccess]
    public async Task<IActionResult> GetByPlanId(long planId)
        => Ok(await _service.FindStrategyPillarDataByPlanIdAsync(planId));

    [HttpPost("StrategyPillarCreate")]
    [CheckAccess]
    public async Task<IActionResult> Create([FromBody] StrategyPillarDto dto)
        => Ok(await _service.StrategyPillarCreateAsync(dto));
}
