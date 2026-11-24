using Common.model.dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Common.service.impl;
namespace Common.controller;
[ApiController,Route("api/v1/CartableStatus")]
public sealed class CartableStatusController(IServiceProvider services, CartableConfirmationLinkService confirmationLinks):ControllerBase
{
    private ICartableStatusControllerService service => services.GetRequiredService<ICartableStatusControllerService>();
    [HttpPost("createConfirmLink"),Authorize(Policy="SemRequestUriAccess")]
    public IActionResult CreateConfirmLink([FromBody]List<CartableStatusDto> items)
    {service.CreateCartableStatusLink(items);return Ok();}
    [HttpPost("createRejectLink"),Authorize(Policy="SemRequestUriAccess")]
    public IActionResult CreateRejectLink([FromBody]List<CartableStatusDto> items)
    {service.CreateCartableStatusLink(items);return Ok();}
    [HttpPost("getByEntityNameAndOperationTypeAndOutCome")]
    public IActionResult GetByEntityNameAndOperationTypeAndOutCome([FromBody]CartableStatusDto dto)
        =>Ok(service.GetByEntityNameAndOperationTypeAndOutCome(dto));
    [HttpPost("getByEntityName")]
    public IActionResult GetByEntityName([FromBody]CartableStatusDto dto)=>Ok(service.GetByEntityNameAndOperationType(dto));
    [HttpPost("confirmationLink")]
    public async Task<IActionResult> FindConfirmationLink([FromBody] ConfirmationLinkRequest body, CancellationToken cancellationToken)
        => Ok(await confirmationLinks.FindAsync(body, cancellationToken));
}
