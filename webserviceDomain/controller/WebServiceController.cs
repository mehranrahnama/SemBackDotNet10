using SortDirection = global::SemBackDotNet10.Common.enums.SortDirection;
using Common.utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SemBackDotNet10.webserviceDomain.service.interf;

namespace SemBackDotNet10.webserviceDomain.controller;

[ApiController]
[Route("api/v1/webservices")]
[AllowAnonymous]
public class WebServiceController : ControllerBase
{
    private readonly IWebServicesService _webServicesService;

    public WebServiceController(IWebServicesService webServicesService)
    {
        _webServicesService = webServicesService;
    }

    [HttpPost("strategyObjectList")]
    public async Task<IActionResult> GetAllStrategyObjects(
        [FromQuery] int page,
        [FromQuery] int size,
        [FromQuery] string? orderBy,
        [FromQuery] SortDirection? direction,
        CancellationToken ct = default)
    {
        var response = await _webServicesService.FindAllStrategyServicesAsync(page, size, orderBy, direction, ct);
        return Ok(response);
    }

    [HttpPost("actionPlanList")]
    public async Task<IActionResult> GetAllActionPlans(
        CancellationToken ct = default)
    {
        var response = await _webServicesService.FindAllActionPlansAsync(ct);
        return Ok(response);
    }
}
