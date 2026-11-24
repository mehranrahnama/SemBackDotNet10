using System.Text.Json.Nodes;
namespace Common.logging.service;
public sealed class ActionLogService(IElasticsearchSearchTransport transport)
{
    public Task<List<JsonObject?>> GetActionPlanCatalogCreateLog(string body,CancellationToken cancellationToken=default) => DomainLogQuery.Fetch(transport,DomainLogQuery.Build(body,"actionPlanName","create","ActionPlanCatalogServiceImpl.java",false),cancellationToken);
    public Task<List<JsonObject?>> GetActionPlanCatalogEditLog(string body,CancellationToken cancellationToken=default) => DomainLogQuery.Fetch(transport,DomainLogQuery.Build(body,"actionPlanName","edit","ActionPlanCatalogServiceImpl.java",true),cancellationToken);
    public Task<List<JsonObject?>> GetActionPlanCatalogDeleteLog(string body,CancellationToken cancellationToken=default) => DomainLogQuery.Fetch(transport,DomainLogQuery.Build(body,"actionPlanName","editDeleteStatus","ActionPlanCatalogServiceImpl.java",true),cancellationToken);
    public Task<List<JsonObject?>> GetActionPlanCreateLog(string body,CancellationToken cancellationToken=default) => DomainLogQuery.Fetch(transport,DomainLogQuery.Build(body,"actionPlanName","create_V2","ActionPlanServiceImpl.java",true),cancellationToken);
    public Task<List<JsonObject?>> GetActionPlanEditLog(string body,CancellationToken cancellationToken=default) => DomainLogQuery.Fetch(transport,DomainLogQuery.Build(body,"actionPlanName","editWithActivity","ActionPlanServiceImpl.java",true),cancellationToken);
    public Task<List<JsonObject?>> GetActionPlanDeleteLog(string body,CancellationToken cancellationToken=default) => DomainLogQuery.Fetch(transport,DomainLogQuery.Build(body,"actionPlanName","editDeleteStatus","ActionPlanServiceImpl.java",true),cancellationToken);
}
