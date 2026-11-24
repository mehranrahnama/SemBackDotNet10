using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SemBackDotNet10.analysisDomain.model.dto.porter;
using SemBackDotNet10.analysisDomain.service.interf.porter;

namespace SemBackDotNet10.analysisDomain.controller.porter;

[ApiController]
[Route("api/v1")]
[Authorize]
public class ProductThreatController : ControllerBase
{
    private readonly IProductThreatService _productThreatService;

    public ProductThreatController(IProductThreatService productThreatService)
    {
        _productThreatService = productThreatService;
    }

    [HttpPost("productThreat/create")]
    public async Task<IActionResult> CreateProductThreat([FromBody] List<ProductThreatDto> productThreatDtoList)
    {
        await _productThreatService.CreateNewProductThreatAsync(productThreatDtoList);
        return StatusCode(StatusCodes.Status201Created);
    }

    [HttpPost("productThreat/getAllByProductMarket")]
    public async Task<IActionResult> GetAllByProductMarket([FromBody] JsonElement? body) =>
        Ok(await _productThreatService.GetAllByProductMarketAsync(body?.GetRawText() ?? "{}"));

    [HttpPut("productThreat/edit")]
    public async Task<IActionResult> EditProductThreat([FromBody] List<ProductThreatDto> productThreatDtoList) =>
        Accepted(await _productThreatService.EditProductThreatAsync(productThreatDtoList));
}
