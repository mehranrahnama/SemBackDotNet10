using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SemBackDotNet10.analysisDomain.model.dto.porter;
using SemBackDotNet10.analysisDomain.service.interf.porter;

namespace SemBackDotNet10.analysisDomain.controller.porter;

[ApiController]
[Route("api/v1")]
[Authorize]
public class AltProductController : ControllerBase
{
    private readonly IAltProductService _altProductService;

    public AltProductController(IAltProductService altProductService)
    {
        _altProductService = altProductService;
    }

    [HttpPost("altProduct/create")]
    public async Task<IActionResult> CreateAltProduct([FromBody] AltProductDto altProductDto)
    {
        var result = await _altProductService.CreateNewAltProductAsync(altProductDto);
        return result != null ? StatusCode(StatusCodes.Status201Created, result) : StatusCode(StatusCodes.Status304NotModified);
    }

    [HttpPut("altProduct/edit")]
    public async Task<IActionResult> EditAltProduct([FromBody] AltProductDto altProductDto)
    {
        await _altProductService.UpdateAltProductAsync(altProductDto);
        return Ok();
    }

    [HttpDelete("altProduct/delete/{altProductId:long}")]
    public async Task<IActionResult> DeleteAltProduct(long altProductId)
    {
        await _altProductService.DeleteAltProductByIdAsync(altProductId);
        return Ok();
    }

    [HttpGet("altProduct/getAllByProduct/{productId:long}")]
    public async Task<IActionResult> GetAltProduct(long productId) =>
        Ok(await _altProductService.GetAllAltProductByProductIdAsync(productId));
}
