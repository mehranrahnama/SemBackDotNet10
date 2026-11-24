using Microsoft.AspNetCore.Mvc;
using SemBackDotNet10.actionDomain.model.dto;
using SemBackDotNet10.actionDomain.service.interf;

namespace SemBackDotNet10.actionDomain.controller;

[ApiController]
[Route("api/v1")]
public class ActionPlanCatalogController : ControllerBase
{
    private readonly IActionPlanCatalogService _actionPlanCatalogService;

    public ActionPlanCatalogController(
        IActionPlanCatalogService actionPlanCatalogService)
    {
        _actionPlanCatalogService = actionPlanCatalogService;
    }

    [HttpPost("actionPlanCatalog")]
    public IActionResult create(
        [FromBody] ActionPlanCatalogDto actionPlanCatalogDto)
    {
        var entity = _actionPlanCatalogService.create(
            actionPlanCatalogDto,
            Request);

        var dto = _actionPlanCatalogService.mapToDto(entity);

        return Ok(dto);
    }

    [HttpDelete("actionPlanCatalog/{id}")]
    public IActionResult delete([FromRoute] long id)
    {
        _actionPlanCatalogService.delete(id);

        return Ok();
    }

    [HttpPut("actionPlanCatalog/delete-status")]
    public IActionResult editDeleteStatus(
        [FromBody] ActionPlanCatalogDto actionPlanCatalogDto)
    {
        var entity =
            _actionPlanCatalogService.editDeleteStatus(actionPlanCatalogDto);

        var dto = _actionPlanCatalogService.mapToDto(entity);

        return Ok(dto);
    }

    [HttpGet("expireActionPlanCatalogTasks")]
    public IActionResult expireActionPlanCatalogTasks(
        [FromHeader(Name = "factoryid")] string? factoryId)
    {
        if (!long.TryParse(factoryId, out var tenantId))
        {
            return BadRequest(new
            {
                message = "هدر factoryid ارسال نشده یا مقدار آن معتبر نیست."
            });
        }

        var result =
            _actionPlanCatalogService.expireActionPlanCatalogTasks(tenantId);

        return result
            ? Ok()
            : StatusCode(417);
    }
}
