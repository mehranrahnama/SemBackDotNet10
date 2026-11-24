using System.Text.Json.Nodes;
namespace Common.logging.service;
public sealed class MeasureLogService(IElasticsearchSearchTransport transport)
{
    public Task<List<JsonObject?>> GetMeasureCatalogCreateLog(string body,CancellationToken cancellationToken=default) => DomainLogQuery.Fetch(transport,DomainLogQuery.Build(body,"measureName","create_V2","MeasureRepositoryServiceImpl.java",false),cancellationToken);
    public Task<List<JsonObject?>> GetMeasureCatalogEditLog(string body,CancellationToken cancellationToken=default) => DomainLogQuery.Fetch(transport,DomainLogQuery.Build(body,"measureName","edit","MeasureRepositoryServiceImpl.java",true),cancellationToken);
    public Task<List<JsonObject?>> GetMeasureCatalogDeleteLog(string body,CancellationToken cancellationToken=default) => DomainLogQuery.Fetch(transport,DomainLogQuery.Build(body,"measureName","deleteMeasureCatalog","MeasureCatalogServiceImpl.java",true),cancellationToken);
    public Task<List<JsonObject?>> GetMeasureCreateLog(string body,CancellationToken cancellationToken=default) => DomainLogQuery.Fetch(transport,DomainLogQuery.Build(body,"measureName","create","MeasureServiceImpl.java",true),cancellationToken);
    public Task<List<JsonObject?>> GetMeasureEditLog(string body,CancellationToken cancellationToken=default) => DomainLogQuery.Fetch(transport,DomainLogQuery.Build(body,"measureName","edit","MeasureServiceImpl.java",true),cancellationToken);
    public Task<List<JsonObject?>> GetMeasureDeleteLog(string body,CancellationToken cancellationToken=default) => DomainLogQuery.Fetch(transport,DomainLogQuery.Build(body,"measureName","editDeleteStatus","MeasureServiceImpl.java",true),cancellationToken);
}
