using Microsoft.AspNetCore.Mvc;
using SemBackDotNet10.Controllers;
using SemBackDotNet10.service.impl;

namespace SemBackDotNet10.actionDomain.controller;

[ApiController]
[Route("api/v1")]
public sealed class ActionPlanCountsController(ActionPlanCountsService service) : ControllerBase
{
    [HttpPost("actionPlan/counts")]
    [CheckAccess]
    public async Task<IActionResult> GetCounts([FromBody] ActionPlanCountsInput body, CancellationToken ct)
        => Ok(await service.GetAsync(body, ct));
}
