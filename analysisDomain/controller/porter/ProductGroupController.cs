using SortDirection = global::SemBackDotNet10.Common.enums.SortDirection;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SemBackDotNet10.analysisDomain.model.dto.porter;
using SemBackDotNet10.analysisDomain.service.interf.porter;
using SemBackDotNet10.Common.utils;

namespace SemBackDotNet10.analysisDomain.controller.porter;

[ApiController]
[Route("api/v1")]
[Authorize]
public class ProductGroupController : ControllerBase
{
    private readonly IProductGroupService _productGroupService;

    public ProductGroupController(IProductGroupService productGroupService)
    {
        _productGroupService = productGroupService;
    }

    [HttpPost("productGroup/create")]
    public async Task<IActionResult> CreateProductGroup([FromBody] ProductGroupDto productGroupDto)
    {
        var result = await _productGroupService.CreateProductGroupAsync(productGroupDto);
        return result != null ? StatusCode(StatusCodes.Status201Created, result) : StatusCode(StatusCodes.Status304NotModified);
    }

    [HttpPost("productGroup/getAll/page")]
    public async Task<IActionResult> GetProductGroupPage([FromQuery] int page, [FromQuery] int size,
        [FromQuery] string? orderBy, [FromQuery] SortDirection? direction, [FromBody] JsonElement? body) =>
        Ok(await _productGroupService.FindAllPageAsync(page, size, orderBy, direction, body?.GetRawText() ?? "{}"));

    [HttpGet("productGroup/getAll/{planId:long}")]
    public async Task<IActionResult> GetAllProductGroup(long planId) =>
        Ok(await _productGroupService.FindAllProductGroupsAsync(planId));

    [HttpPut("productGroup/edit")]
    public async Task<IActionResult> EditProductGroup([FromBody] ProductGroupDto productGroupDto) =>
        Ok(await _productGroupService.EditProductGroupAsync(productGroupDto));

    [HttpDelete("productGroup/delete/{productGroupId:long}")]
    public async Task<IActionResult> DeleteProductGroup(long productGroupId)
    {
        await _productGroupService.DeleteProductGroupAsync(productGroupId);
        return Ok();
    }
}
