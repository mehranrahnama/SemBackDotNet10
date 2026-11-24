using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SemBackDotNet10.analysisDomain.model.dto.porter;
using SemBackDotNet10.analysisDomain.service.interf.porter;

namespace SemBackDotNet10.analysisDomain.controller.porter;

[ApiController]
[Route("api/v1")]
[Authorize]
public class PorterResultController : ControllerBase
{
    private readonly IPorterResultService _porterResultService;

    public PorterResultController(IPorterResultService porterResultService)
    {
        _porterResultService = porterResultService;
    }

    [HttpPost("porterResult/create")]
    public async Task<IActionResult> CreatePorterResult([FromBody] PorterResultDto porterResultDto)
    {
        await _porterResultService.CreateNewPorterResultAsync(porterResultDto);
        return StatusCode(StatusCodes.Status201Created);
    }

    [HttpPost("porterResult/getAllByProductMarket")]
    public async Task<IActionResult> GetAllByProductMarket([FromBody] JsonElement? body) =>
        Ok(await _porterResultService.GetAllByProductMarketAsync(body?.GetRawText() ?? "{}"));
}
