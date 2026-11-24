using SemBackDotNet10.Common;
using Microsoft.AspNetCore.Mvc;
using SemBackDotNet10.basicDomain.service;

namespace SemBackDotNet10.Controllers;

/// <summary>Port of PoopakController (base route /api/v1).</summary>
[ApiController]
[Route("api/v1")]
public class PoopakController : ControllerBase
{
    private readonly IPoopakService _poopakService;
    public PoopakController(IPoopakService poopakService)
    {
        _poopakService = poopakService;
    }

    [HttpPost("sendMessageToUsers")]
    [CheckAccess]
    public IActionResult SendMessageToPoopak([FromBody] ERPSendMessageDto message)
    {
        _poopakService.SendSMSMessage(message);
        return Ok();
    }
}
