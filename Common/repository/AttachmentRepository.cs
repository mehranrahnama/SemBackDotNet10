using Common.model.entity;
using Common.model.dto;
using Common.utils;
namespace Common.repository;
public interface IAttachmentGridQuery
{
    // Must preserve Java createdBy projection, count query and DB sort/collation semantics.
    RepositoryPage<AttachmentDto> LoadGridById(PageRequest page,string? entityName,long? entityId);
}
public sealed class AttachmentRepository(IPersistenceSession<Attachment> session,IAttachmentGridQuery grid):EntityRepository<Attachment,long?>(session,a=>a.Id)
{
    public RepositoryPage<AttachmentDto> LoadGridById(PageRequest page,string? entityName,long? entityId)=>grid.LoadGridById(page,entityName,entityId);
    public Attachment? GetLastFileByEntityNameAndEntityId(string entityName,long? entityId)=>Session.Query.Where(a=>a.EntityName==entityName && a.EntityId==entityId).OrderByDescending(a=>a.Id).FirstOrDefault();
    public Attachment? FindByFileName(string fileName)=>Session.Query.SingleOrDefault(a=>a.FileName==fileName);
}
