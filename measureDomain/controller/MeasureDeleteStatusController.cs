using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using SemBackDotNet10.Controllers;
using SemBackDotNet10.measureDomain.service.impl;
using SemBackDotNet10.service.impl;
using global::Common.service.interf;

namespace SemBackDotNet10.measureDomain.controller;

[ApiController]
[Route("api/v1/measure/delete-status")]
public sealed class MeasureDeleteStatusController(IConfiguration configuration) : ControllerBase
{
    [HttpPut]
    [CheckAccess]
    public async Task<IActionResult> Update([FromBody] JsonElement body, CancellationToken ct)
    {
        // Opt in only after validating this destructive workflow against the target schema.
        if (!configuration.GetValue<bool>("Features:MeasureDeleteStatus"))
            return NotFound();

        var taskBaseUrl = configuration["taskUrl"];
        if (!Uri.TryCreate(taskBaseUrl, UriKind.Absolute, out var uri)
            || uri.Scheme is not ("http" or "https")
            || string.IsNullOrEmpty(uri.Host) || !uri.AbsolutePath.EndsWith('/'))
            return Problem(statusCode: 503, title: "The measure task base URL is not configured correctly.");

        var request = MeasureDeleteStatusRequest.Parse(body);
        var services = HttpContext.RequestServices;
        var workflow = new MeasureDeleteStatusWorkflow(services.GetRequiredService<ServiceRuntime>(),
            services.GetRequiredService<CartableStatusService>());
        await workflow.ExecuteAsync(request, taskBaseUrl!, ct);
        return Ok();
    }
}
