using Common.model.entity;
namespace Common.repository;

public sealed class CommentRepository(IPersistenceSession<Comment> session)
    : EntityRepository<Comment,long?>(session,x=>x.Id)
{
    public List<Comment> FindAllByEntityNameAndEntityIdOrderByCreatedDateDesc(string name,long id)
        =>Session.Query.Where(x=>x.EntityName==name && x.EntityId==id).OrderByDescending(x=>x.CreatedDate).ToList();
    public List<Comment> FindAllByEntityIdAndEntityName(long id,string name)
        =>Session.Query.Where(x=>x.EntityId==id && x.EntityName==name).ToList();
    // Java's tuple SELECT declared as List<Comment> is replaced with full entities.
    public List<Comment> FindAllByEntityNameAndEntityId(string name,long id)
        =>FindAllByEntityIdAndEntityName(id,name);
}
