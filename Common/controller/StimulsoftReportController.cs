using Microsoft.AspNetCore.Mvc;
namespace Common.controller;
[Route("api/v1/stimulsoftReport")]
public sealed class StimulsoftReportController(IStimulsoftReportRenderer renderer):Controller
{
    [HttpPost("report")]
    public Task<IActionResult> IndexAction(CancellationToken cancellationToken)=>renderer.RenderIndex(cancellationToken);
}
