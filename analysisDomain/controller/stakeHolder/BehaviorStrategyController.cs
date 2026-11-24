using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SemBackDotNet10.analysisDomain.service.impl.stakeHolder;

namespace SemBackDotNet10.analysisDomain.controller.stakeHolder;

[ApiController]
[Route("api/v1/behaviorStrategy")]
[Authorize]
public class BehaviorStrategyController : ControllerBase
{
    private readonly BehaviorStrategyService _behaviorStrategyService;

    public BehaviorStrategyController(BehaviorStrategyService behaviorStrategyService)
    {
        _behaviorStrategyService = behaviorStrategyService;
    }

    [HttpGet("getAll")]
    public IActionResult GetAllBehaviorStrategy()
    {
        var behaviorStrategies = _behaviorStrategyService.GetAllBehaviorStrategy();
        return Ok(behaviorStrategies);
    }
}
