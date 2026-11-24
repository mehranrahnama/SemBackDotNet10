using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Common.service.impl;
namespace Common.controller;
[ApiController]
[Authorize]
[Route("api/v1")]
public sealed class CartableController(ICartableControllerService service):ControllerBase
{
    // No per-action authorization in Java: host must protect this endpoint globally.
    [HttpGet("cartable/page")]
    public async Task<IActionResult> GetTaskList([FromQuery]int page,[FromQuery]int size,[FromQuery]string? orderBy=null,[FromQuery]string? direction=null,[FromQuery]bool? completed=null,[FromQuery]long? planId=null,[FromQuery]string? filter=null, CancellationToken cancellationToken = default)
    {
        if (page < 0 || size < 1 || (direction is not null && !direction.Equals("ASC", StringComparison.OrdinalIgnoreCase) && !direction.Equals("DESC", StringComparison.OrdinalIgnoreCase)))
            return BadRequest("Invalid pagination or direction.");
        try { return Ok(await service.GetTaskListForSanganAsync(planId, completed, filter, page, size, orderBy, direction, cancellationToken)); }
        catch (CartableQueriesUnavailableException) { return StatusCode(StatusCodes.Status503ServiceUnavailable, new { error = "CARTABLE_QUERIES_UNAVAILABLE", message = "Cartable task queries have not been configured." }); }
    }
}
