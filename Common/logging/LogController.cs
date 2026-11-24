using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc;
namespace Common.logging;
// Register only outside the Java 'dev' profile using SemLoggingProfile below.
[ApiController,Route("api/v1")]
public sealed class LogController(LogService logs,IMeasureLogControllerService measures,IActionLogControllerService actions):ControllerBase
{
    [HttpPost("logs")]
    public Task<List<JsonObject?>> GetFilteredLogs([FromBody]string body,[FromQuery]int page,[FromQuery]int size,[FromQuery]string? direction=null,[FromQuery]string? orderBy=null,CancellationToken cancellationToken=default)
        =>logs.GetFilteredLogs(body,page,size,orderBy,direction,cancellationToken);
    [HttpPost("log/createMeasureCatalog")]
    public Task<List<JsonObject?>> GetMeasureCatalogCreateLog([FromBody]string body,CancellationToken cancellationToken)
        =>measures.GetMeasureCatalogCreateLog(body,cancellationToken);
    [HttpPost("log/editMeasureCatalog")]
    public Task<List<JsonObject?>> GetMeasureCatalogEditLog([FromBody]string body,CancellationToken cancellationToken)
        =>measures.GetMeasureCatalogEditLog(body,cancellationToken);
    [HttpPost("log/deleteMeasureCatalog")]
    public Task<List<JsonObject?>> GetMeasureCatalogDeleteLog([FromBody]string body,CancellationToken cancellationToken)
        =>measures.GetMeasureCatalogDeleteLog(body,cancellationToken);
    [HttpPost("log/createMeasure")]
    public Task<List<JsonObject?>> GetMeasureCreateLog([FromBody]string body,CancellationToken cancellationToken)
        =>measures.GetMeasureCreateLog(body,cancellationToken);
    [HttpPost("log/editMeasure")]
    public Task<List<JsonObject?>> GetMeasureEditLog([FromBody]string body,CancellationToken cancellationToken)
        =>measures.GetMeasureEditLog(body,cancellationToken);
    [HttpPost("log/deleteMeasure")]
    public Task<List<JsonObject?>> GetMeasureDeleteLog([FromBody]string body,CancellationToken cancellationToken)
        =>measures.GetMeasureDeleteLog(body,cancellationToken);
    [HttpPost("log/createActionPlanCatalog")]
    public Task<List<JsonObject?>> GetActionPlanCatalogCreateLog([FromBody]string body,CancellationToken cancellationToken)
        =>actions.GetActionPlanCatalogCreateLog(body,cancellationToken);
    [HttpPost("log/editActionPlanCatalog")]
    public Task<List<JsonObject?>> GetActionPlanCatalogEditLog([FromBody]string body,CancellationToken cancellationToken)
        =>actions.GetActionPlanCatalogEditLog(body,cancellationToken);
    [HttpPost("log/deleteActionPlanCatalog")]
    public Task<List<JsonObject?>> GetActionPlanCatalogDeleteLog([FromBody]string body,CancellationToken cancellationToken)
        =>actions.GetActionPlanCatalogDeleteLog(body,cancellationToken);
    [HttpPost("log/createActionPlan")]
    public Task<List<JsonObject?>> GetActionPlanCreateLog([FromBody]string body,CancellationToken cancellationToken)
        =>actions.GetActionPlanCreateLog(body,cancellationToken);
    [HttpPost("log/editActionPlan")]
    public Task<List<JsonObject?>> GetActionPlanEditLog([FromBody]string body,CancellationToken cancellationToken)
        =>actions.GetActionPlanEditLog(body,cancellationToken);
    [HttpPost("log/deleteActionPlan")]
    public Task<List<JsonObject?>> GetActionPlanDeleteLog([FromBody]string body,CancellationToken cancellationToken)
        =>actions.GetActionPlanDeleteLog(body,cancellationToken);
}
public interface IMeasureLogControllerService
{
    Task<List<JsonObject?>> GetMeasureCatalogCreateLog(string body,CancellationToken cancellationToken);
    Task<List<JsonObject?>> GetMeasureCatalogEditLog(string body,CancellationToken cancellationToken);
    Task<List<JsonObject?>> GetMeasureCatalogDeleteLog(string body,CancellationToken cancellationToken);
    Task<List<JsonObject?>> GetMeasureCreateLog(string body,CancellationToken cancellationToken);
    Task<List<JsonObject?>> GetMeasureEditLog(string body,CancellationToken cancellationToken);
    Task<List<JsonObject?>> GetMeasureDeleteLog(string body,CancellationToken cancellationToken);
}
public interface IActionLogControllerService
{
    Task<List<JsonObject?>> GetActionPlanCatalogCreateLog(string body,CancellationToken cancellationToken);
    Task<List<JsonObject?>> GetActionPlanCatalogEditLog(string body,CancellationToken cancellationToken);
    Task<List<JsonObject?>> GetActionPlanCatalogDeleteLog(string body,CancellationToken cancellationToken);
    Task<List<JsonObject?>> GetActionPlanCreateLog(string body,CancellationToken cancellationToken);
    Task<List<JsonObject?>> GetActionPlanEditLog(string body,CancellationToken cancellationToken);
    Task<List<JsonObject?>> GetActionPlanDeleteLog(string body,CancellationToken cancellationToken);
}
