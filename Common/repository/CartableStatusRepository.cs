using Common.model.entity;
using Common.model.dto;
namespace Common.repository;
public interface ICartableStatusQueries
{
    CartableStatus? FindByEntityNameIgnoreCaseAndOutComeIgnoreCaseAndOldStatus(string entityName,string outCome,long? oldStatus);
    List<CartableStatusDto> FindListByEntityNameAndOperationTypeAndOutCome(string entityName,string operationType,string outCome);
    List<CartableStatusDto> FindListByEntityName(string entityName);
    List<CartableStatusDto> FindAllByEntityNameAndOperationTypeAndOutCome(string entityName,string? operationType,string outCome);
    // Preserve LEFT JOIN, null status semantics and non-unique Optional failure for tied maxima.
    CartableStatusPair? FindMaxNewStatusByEntityAndOperationAndOutcomeAndRoles(string entityName,string? operationType,string outCome,ISet<string> userRoles,long? confirmStatus);
}
public sealed class CartableStatusRepository(IPersistenceSession<CartableStatus> session,ICartableStatusQueries queries):EntityRepository<CartableStatus,long?>(session,c=>c.Id)
{
    public CartableStatus? FindByEntityNameIgnoreCaseAndOutComeIgnoreCaseAndOldStatus(string e,string o,long? s)=>queries.FindByEntityNameIgnoreCaseAndOutComeIgnoreCaseAndOldStatus(e,o,s);
    public List<CartableStatus> FindByEntityNameAndOperationTypeAndOutCome(string e,string? op,string o)=>Session.Query.Where(c=>c.EntityName==e && c.OperationType==op && c.OutCome==o).ToList();
    public List<CartableStatusDto> FindListByEntityNameAndOperationTypeAndOutCome(string e,string op,string o)=>queries.FindListByEntityNameAndOperationTypeAndOutCome(e,op,o);
    public List<CartableStatusDto> FindListByEntityName(string e)=>queries.FindListByEntityName(e);
    public List<string> GetAllEntityNames()=>Session.Query.Select(c=>c.EntityName!).Distinct().ToList();
    public List<CartableStatusDto> FindAllByEntityNameAndOperationTypeAndOutCome(string e,string? op,string o)=>queries.FindAllByEntityNameAndOperationTypeAndOutCome(e,op,o);
    public CartableStatus? FindByEntityNameAndOutCome(string e,string o)=>Session.Query.SingleOrDefault(c=>c.EntityName==e && c.OutCome==o);
    public List<CartableStatus> FindByEntityNameAndCreateOperationTypeAndConfirm(string e,string? op)=>Session.Query.Where(c=>c.EntityName==e && (c.OperationType==op || c.OperationType==null) && (c.OutCome=="preConfirm" || c.OutCome=="confirm")).ToList();
    public List<CartableStatus> FindByEntityNameAnDeleteOperationTypeAndConfirm(string e,string op)=>FindByEntityNameAndOperationTypeAndOutCome(e,op,"confirm");
    public List<CartableStatus> FindByEntityNameAndOperationTypeAnReject(string e,string op)=>FindByEntityNameAndOperationTypeAndOutCome(e,op,"reject");
    public List<CartableStatus> FindByEntityNameAndCreateOperationTypeAndReplan(string e,string op,string o)=>FindByEntityNameAndOperationTypeAndOutCome(e,op,o);
    public CartableStatusPair? FindMaxNewStatusByEntityAndOperationAndOutcomeAndRoles(string e,string? op,string o,ISet<string> roles,long? status)=>queries.FindMaxNewStatusByEntityAndOperationAndOutcomeAndRoles(e,op,o,roles,status);
}
