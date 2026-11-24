using Common.model.entity;
namespace Common.repository;

public sealed class HistoryRepository(IPersistenceSession<History> session):EntityRepository<History,long?>(session,x=>x.Id)
{
    public List<History> FindAllByEntityNameAndEntityIdOrderByCreatedDate(string name,long id)
        =>Session.Query.Where(x=>x.EntityName==name && x.EntityId==id).OrderBy(x=>x.CreatedDate).ToList();
    public List<History> FindByEntityNameAndEntityId(string name,long id)
        =>Session.Query.Where(x=>x.EntityName==name && x.EntityId==id).ToList();
    // Database collation must support the invariant casing used by this predicate.
    private IQueryable<History> Matching(long id,string name,long status,string outcome)
    {
        var upper=name.ToUpperInvariant();
        return Session.Query.Where(x=>x.EntityId==id && x.EntityName!=null && x.EntityName.ToUpper()==upper && x.Status==status && x.OutCome==outcome);
    }
    public History? FindByEntityIdAndEntityNameEqualsIgnoreCaseAndStatusAndOutCome(long id,string name,long status,string outcome)
        =>Matching(id,name,status,outcome).SingleOrDefault();
    public History? FindTopByEntityIdAndEntityNameEqualsIgnoreCaseAndStatusAndOutComeOrderByCreatedDateDesc(long id,string name,long status,string outcome)
        =>Matching(id,name,status,outcome).OrderByDescending(x=>x.CreatedDate).FirstOrDefault();
}
