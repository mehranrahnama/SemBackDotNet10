using System.Linq.Expressions;
namespace Common.repository;

// Host integration boundary, not an implementation of the missing Java SemRepository.
// Save must persist changes before returning. The host owns transactions, validation,
// audit metadata, authorization, connection lifetime and optimistic concurrency.
public interface IPersistenceSession<T> where T:class
{
    IQueryable<T> Query { get; }
    T Save(T entity);
    void Delete(T entity);
}

public class EntityRepository<T,TKey> where T:class
{
    protected readonly IPersistenceSession<T> Session;
    private readonly Expression<Func<T,TKey>> keySelector;
    public EntityRepository(IPersistenceSession<T> session, Expression<Func<T,TKey>> keySelector)
    {
        Session=session ?? throw new ArgumentNullException(nameof(session));
        this.keySelector=keySelector ?? throw new ArgumentNullException(nameof(keySelector));
    }
    public T? FindById(TKey id)
    {
        var body=Expression.Equal(keySelector.Body,Expression.Constant(id,typeof(TKey)));
        return Session.Query.SingleOrDefault(Expression.Lambda<Func<T,bool>>(body,keySelector.Parameters));
    }
    public T GetById(TKey id)=>FindById(id) ?? throw new KeyNotFoundException($"{typeof(T).Name} with id {id} was not found.");
    public T Save(T entity){ArgumentNullException.ThrowIfNull(entity);return Session.Save(entity);}
    public void Delete(T entity){ArgumentNullException.ThrowIfNull(entity);Session.Delete(entity);}
    public void DeleteById(TKey id)=>Delete(GetById(id));
    public List<T> FindAll()=>Session.Query.ToList();
    public List<T> FindAll(Expression<Func<T,bool>> predicate)=>Session.Query.Where(predicate).ToList();
    // No implicit SaveAll: host-specific atomic batch semantics must not be guessed.
}
