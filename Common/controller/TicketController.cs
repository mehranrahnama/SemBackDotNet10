using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Common.model.entity;
using Common.service.interf;
namespace Common.controller;
[ApiController]
[Route("api/v1")]
[Authorize(Policy="SemRequestUriAccess")]
public sealed class TicketController(TicketService service):ControllerBase
{
    // Preserve the duplicated v1 segment in the original Java endpoint.
    [HttpPost("v1/ticketCreate")]
    public IActionResult Create([FromBody] Ticket ticket)
    {
        ArgumentNullException.ThrowIfNull(ticket);
        service.Create(ticket);
        return Ok();
    }
}
