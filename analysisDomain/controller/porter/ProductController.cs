using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SemBackDotNet10.analysisDomain.model.dto.porter;
using SemBackDotNet10.analysisDomain.service.interf.porter;
using SemBackDotNet10.Common.enums;
using SemBackDotNet10.Common.utils;

namespace SemBackDotNet10.analysisDomain.controller.porter;

[ApiController]
[Route("api/v1")]
[Authorize]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpPost("product/create")]
    public async Task<IActionResult> CreateProduct([FromBody] ProductDto productDto)
    {
        var result = await _productService.CreateNewProductAsync(productDto);
        return result != null ? StatusCode(StatusCodes.Status201Created, result) : StatusCode(StatusCodes.Status304NotModified);
    }

    [HttpPost("product/getAll/page")]
    public async Task<IActionResult> GetProductPage([FromQuery] int page, [FromQuery] int size,
        [FromQuery] string? orderBy, [FromQuery] SortDirection? direction, [FromBody] JsonElement? body) =>
        Ok(await _productService.GetAllProductPageByProductGroupAsync(page, size, orderBy, direction, body?.GetRawText() ?? "{}"));

    [HttpPost("product/getAllByPlanId")]
    public async Task<IActionResult> GetAllProductByPlanId([FromQuery] int page, [FromQuery] int size,
        [FromQuery] string? orderBy, [FromQuery] SortDirection? direction, [FromBody] JsonElement? body) =>
        Ok(await _productService.FindAllProductByPlanIdAsync(page, size, orderBy, direction, body?.GetRawText() ?? "{}"));

    [HttpPut("product/edit")]
    public async Task<IActionResult> EditProduct([FromBody] ProductDto productDto)
    {
        await _productService.UpdateProductAsync(productDto);
        return Ok();
    }

    [HttpDelete("product/delete/{productId:long}")]
    public async Task<IActionResult> DeleteProduct(long productId)
    {
        await _productService.DeleteProductByIdAsync(productId);
        return Ok();
    }
}
