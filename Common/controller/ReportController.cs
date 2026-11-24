using Microsoft.AspNetCore.Mvc;
namespace Common.controller;
[ApiController,Route("api/v1/jasperReport")]
public sealed class ReportController(IReportControllerService service,IReportFileTimestamp timestamps):ControllerBase
{
    [HttpPost("report")]
    public ContentResult GenerateReport([FromQuery]string fileFormat,[FromQuery]string reportName,[FromBody]string? filter=null)
        // Java @RestController returns literal text, not an HTTP redirect.
        =>Content("redirect:/"+service.GenerateReport(filter,fileFormat,reportName),"text/plain");
    [HttpPost("reportAsByteArray")]
    public IActionResult GenerateReportAsByteArray([FromQuery]string fileFormat,[FromQuery]string reportName,[FromBody]string? filter=null)
    {
        var content=service.GenerateReportAsByteArray(filter,fileFormat,reportName);
        // Preserve Java's PDF content-type even for a non-PDF format.
        return File(content,"application/pdf",reportName+"_"+timestamps.GetPersianTimestamp()+"."+fileFormat);
    }
}
