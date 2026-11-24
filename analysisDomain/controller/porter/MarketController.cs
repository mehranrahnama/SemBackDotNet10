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
public class MarketController : ControllerBase
{
    private readonly IMarketService _marketService;

    public MarketController(IMarketService marketService)
    {
        _marketService = marketService;
    }

    [HttpPost("market/create")]
    public async Task<IActionResult> CreateMarket([FromBody] MarketDto marketDto)
    {
        var result = await _marketService.CreateMarketAsync(marketDto);
        return result != null ? StatusCode(StatusCodes.Status201Created, result) : StatusCode(StatusCodes.Status304NotModified);
    }

    [HttpPost("market/getAll/page")]
    public async Task<IActionResult> GetMarketPage([FromQuery] int page, [FromQuery] int size,
        [FromQuery] string? orderBy, [FromQuery] SortDirection? direction, [FromBody] JsonElement? body) =>
        Ok(await _marketService.FindAllPageAsync(page, size, orderBy, direction, body?.GetRawText() ?? "{}"));

    [HttpGet("market/getAll")]
    public async Task<IActionResult> GetAllMarket() =>
        Ok(await _marketService.FindAllMarketAsync());

    [HttpPut("market/edit")]
    public async Task<IActionResult> EditMarket([FromBody] MarketDto marketDto) =>
        Ok(await _marketService.EditMarketAsync(marketDto));

    [HttpDelete("market/delete/{marketId:long}")]
    public async Task<IActionResult> DeleteMarket(long marketId)
    {
        await _marketService.DeleteMarketAsync(marketId);
        return Ok();
    }
}
