using Common.model.entity;
using Common.model.dto;
using Common.utils;
namespace Common.repository;
public interface ICartableRoyalVisionQueries
{
    RepositoryPage<CartableRoyalVisionDto> LoadGridByObjectId(PageRequest page,long userId);
    List<CartableRoyalVision> FindByMeasureName(string name); // SQL LIKE, preserve DB wildcard/collation rules.
    // Parameterized calls only; procedure bodies are owned by the host database.
    List<CartableRoyalVision> GetTaskList(long? planId,long userId,string filter,string? orderBy,string? direction);
    List<CartableRoyalVision> GetTaskListByTaskRow(long? planId,long userId,long? taskrow,string filter,string? orderBy,string? direction);
    List<CartableRoyalVision> GetTaskListCompleted(long? planId,long userId,string filter,string? orderBy,string? direction);
}
public sealed class CartableRoyalVisionRepository(IPersistenceSession<CartableRoyalVision> session,ICartableRoyalVisionQueries queries):EntityRepository<CartableRoyalVision,long?>(session,c=>c.Id)
{
    public RepositoryPage<CartableRoyalVisionDto> LoadGridByObjectId(PageRequest page,long userId)=>queries.LoadGridByObjectId(page,userId);
    public List<CartableRoyalVision> FindBySmsSent()=>Session.Query.Where(c=>!c.Issmssent).ToList();
    public List<CartableRoyalVision> FindByTaskurl(string url)=>Session.Query.Where(c=>c.Taskurl==url).ToList();
    public CartableRoyalVision? FindByTaskrow(long? row)=>Session.Query.SingleOrDefault(c=>c.Taskrow==row);
    public List<CartableRoyalVision> FindByMeasureName(string name)=>queries.FindByMeasureName(name);
    public List<CartableRoyalVision> GetTaskList(long? planId,long userId,string filter,string? orderBy,string? direction)=>queries.GetTaskList(planId,userId,filter,orderBy,direction);
    public List<CartableRoyalVision> GetTaskListByTaskRow(long? planId,long userId,long? taskrow,string filter,string? orderBy,string? direction)=>queries.GetTaskListByTaskRow(planId,userId,taskrow,filter,orderBy,direction);
    public List<CartableRoyalVision> GetTaskListCompleted(long? planId,long userId,string filter,string? orderBy,string? direction)=>queries.GetTaskListCompleted(planId,userId,filter,orderBy,direction);
}
