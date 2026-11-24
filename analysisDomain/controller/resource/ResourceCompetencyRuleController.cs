using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SemBackDotNet10.analysisDomain.model.dto.resource;
using SemBackDotNet10.analysisDomain.service.interf.resource;

namespace SemBackDotNet10.analysisDomain.controller.resource;

[ApiController]
[Route("api/v1")]
[Authorize]
public class ResourceCompetencyRuleController : ControllerBase
{
    private readonly IResourceCompetencyRuleService _resourceCompetencyRule;

    public ResourceCompetencyRuleController(IResourceCompetencyRuleService resourceCompetencyRule)
    {
        _resourceCompetencyRule = resourceCompetencyRule;
    }

    [HttpPost("resourceCompetencyRule/create")]
    public async Task<IActionResult> CreateResource([FromBody] ResourceCompetencyRuleDto resourceCompetencyRuleDto, CancellationToken ct)
    {
        var result = await _resourceCompetencyRule.CreateNewResourceCompetencyRuleAsync(resourceCompetencyRuleDto, ct);
        return result != null ? StatusCode(StatusCodes.Status201Created, result) : StatusCode(StatusCodes.Status304NotModified);
    }

    [HttpGet("resourceCompetencyRule/getByPlanId/{planId}")]
    public async Task<IActionResult> GetResourceCompetencyRuleByPlanId([FromRoute] long planId, CancellationToken ct)
    {
        var result = await _resourceCompetencyRule.GetResourceCompetencyRuleByPlanIdAsync(planId, ct);
        return Ok(result);
    }
}
