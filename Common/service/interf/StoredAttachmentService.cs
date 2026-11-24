using Common.controller;
using Common.enums;
namespace Common.service.interf;
public interface StoredAttachmentService : IStoredAttachmentControllerService
{
    bool HasUploadedFile(EntityNames entityName,string entityId);
    void DeleteByResourceAndEntityIds(EntityNames resource,List<long> entityIds);
    HashSet<long> FindEntityIdsWithUploadedFile(EntityNames resource,List<long> entityIds);
}
