using Common.model.entity;
using Common.enums;
using Common.controller.dto.response;
namespace Common.repository;
public interface IStoredAttachmentQueries
{
    // Requires an active host transaction and a real pessimistic write lock, NOT a LINQ MAX fallback.
    int FindMaxRowNoForUpdate(EntityNames resource,string entityId);
    // Java signature uses long against a string column; host must resolve this explicitly.
    List<StoredAttachment> FindByEntityIdAndResource(long entityId,EntityNames resource);
    List<StoredAttachmentResDto> FindByEntityIdIn(EntityNames resource,List<string> entityIds);
    void DeleteByResourceAndEntityIdIn(EntityNames resource,List<string> entityIds);
}
public sealed class StoredAttachmentRepository(IPersistenceSession<StoredAttachment> session,IStoredAttachmentQueries queries):EntityRepository<StoredAttachment,long?>(session,a=>a.Id)
{
    public int FindMaxRowNoForUpdate(EntityNames resource,string entityId)=>queries.FindMaxRowNoForUpdate(resource,entityId);
    public List<StoredAttachment> FindByEntityIdAndResource(long entityId,EntityNames resource)=>queries.FindByEntityIdAndResource(entityId,resource);
    public List<StoredAttachment> FindAllFilesByResourceAndEntityId(EntityNames resource,string entityId)=>Session.Query.Where(a=>a.Resource==resource && a.EntityId==entityId).ToList();
    public bool ExistsUploadedFileByResourceAndEntityId(EntityNames resource,string entityId)=>Session.Query.Any(a=>a.Resource==resource && a.EntityId==entityId && a.StoredFile!=null);
    public List<StoredAttachmentResDto> FindByEntityIdIn(EntityNames resource,List<string> entityIds)=>queries.FindByEntityIdIn(resource,entityIds);
    public List<string> FindEntityIdsWithFile(EntityNames resource,List<string> entityIds)=>Session.Query.Where(a=>a.Resource==resource && entityIds.Contains(a.EntityId!) && a.StoredFile!=null).Select(a=>a.EntityId!).ToList();
    public List<long> FindFileIdsByResourceAndEntityIdIn(EntityNames resource,List<string> entityIds)=>Session.Query.Where(a=>a.Resource==resource && entityIds.Contains(a.EntityId!) && a.StoredFile!=null).Select(a=>a.StoredFile!.FileId).ToList();
    public void DeleteByResourceAndEntityIdIn(EntityNames resource,List<string> entityIds)=>queries.DeleteByResourceAndEntityIdIn(resource,entityIds);
}
