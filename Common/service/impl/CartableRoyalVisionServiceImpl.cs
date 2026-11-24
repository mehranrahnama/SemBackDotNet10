using Common.controller;
using Common.exception;
using Common.model.entity;
using Common.repository;
using Common.utils;
using System.Text.Json.Nodes;
namespace Common.service.impl;
public interface ICartableCurrentUser { long GetCurrentUserId(); }
public interface ICartableDelegations
{
    IReadOnlyList<long> FindCurrentDelegations(long userId);
    // Match the real Java entity equality contract; do not infer it from nullable IDs.
    bool SameTask(CartableRoyalVision left,CartableRoyalVision right);
}
public interface ICartableOperationScope { void Execute(Action operation); }
public sealed class CartableRoyalVisionServiceImpl(CartableRoyalVisionRepository repository,ICartableCurrentUser currentUser,ICartableDelegations delegations,ICartableOperationScope operations):Common.service.interf.CartableRoyalVisionService
{
    public CartableRoyalVisionRepository GetRepository()=>repository;
    public object LoadGridByObjectId(string? body,int page,int size,string? orderBy,string? direction)=>repository.LoadGridByObjectId(PageUtils.GetInstance(page,size,orderBy,AttachmentServiceImpl.ParseDirection(direction)),currentUser.GetCurrentUserId());
    public CartableRoyalVision FindByIdOrElseThrow(long? id)=>id is null?throw new BusinessException("error.attachment.idIsNotValid"):repository.FindById(id)??throw new BusinessException("error.attachment.idIsNotValid");
    public void Delete(string measureName)=>operations.Execute(()=>{foreach(var item in repository.FindByMeasureName(measureName.Trim()))repository.Delete(item);});
    public List<CartableRoyalVision> FindByTaskurl(string url)=>repository.FindByTaskurl(url);
    public CartableRoyalVision? FindByTaskrow(long? row)=>repository.FindByTaskrow(row);
    public List<CartableRoyalVision> FindByMeasureName(string name)=>repository.FindByMeasureName(name);
    public List<CartableRoyalVision> FindBySmsSent()=>repository.FindBySmsSent();
    private static JsonObject Body(string? body)=>JsonNode.Parse(body??throw new ArgumentNullException(nameof(body))) as JsonObject ?? throw new FormatException("Expected JSON object.");
    private static long? Plan(JsonObject json)=>json["planId"]?.GetValue<long>();
    private static string Filter(JsonObject json)=>json["filter"]?.GetValue<string>()??"";
    private List<CartableRoyalVision> Tasks(JsonObject json,string? order,string? direction,bool completed=false)=>completed?
        repository.GetTaskListCompleted(Plan(json),currentUser.GetCurrentUserId(),Filter(json),order,direction):
        repository.GetTaskList(Plan(json),currentUser.GetCurrentUserId(),Filter(json),order,direction);
    private List<CartableRoyalVision> Delegated(JsonObject json,string? order,string? direction)
    {
        var items=Tasks(json,order,direction);
        foreach(var id in delegations.FindCurrentDelegations(currentUser.GetCurrentUserId()))
            foreach(var task in repository.GetTaskList(Plan(json),id,Filter(json),order,direction))
                if(!items.Any(existing=>delegations.SameTask(existing,task)))items.Add(task);
        items.RemoveAll(task=>task.Taskurl is null);
        foreach(var task in items)task.Taskurl=task.Taskurl!.Replace("https://bpms.hosco.ir/sem1/Security/SemRouter/?target=","https://sem1.hosco.ir/sem1/Security/SemRouter/?target=",StringComparison.Ordinal);
        return items;
    }
    private static string ProcedureDirection(string? direction)
    {
        _=AttachmentServiceImpl.ParseDirection(direction??throw new ArgumentNullException(nameof(direction)));
        // Deliberate fix: Java sometimes used reference equality (==) for strings.
        return direction=="ASC"?"1":"2";
    }
    public object GetTaskList(string? body,int page,int size,string? orderBy,string? direction)=>RepositoryPaging.Slice(Tasks(Body(body),orderBy,ProcedureDirection(direction)),PageUtils.GetInstance(page,size,orderBy,AttachmentServiceImpl.ParseDirection(direction)));
    public object GetTaskListV2(string? body,int page,int size,string? orderBy,string? direction)=>RepositoryPaging.Slice(Delegated(Body(body),orderBy,ProcedureDirection(direction)),PageUtils.GetInstance(page,size,orderBy,AttachmentServiceImpl.ParseDirection(direction)));
    public object GetTotalTaskCount(string body)
    {
        var json=Body(body);_ = delegations.FindCurrentDelegations(currentUser.GetCurrentUserId());
        return Tasks(json,null,"1").Count;
    }
    public object GetTaskListByFilter(string? body,string? orderBy,string? direction)=>new {content=Delegated(Body(body),orderBy,direction)};
    public object GetTaskListByFilterCompleted(string? body,string? orderBy,string? direction)=>new {content=Tasks(Body(body),orderBy,direction,true)};
    public object GetTaskListCompleted(string? body,int page,int size,string? orderBy,string? direction)=>RepositoryPaging.Slice(Tasks(Body(body),orderBy,ProcedureDirection(direction),true),PageUtils.GetInstance(page,size,orderBy,AttachmentServiceImpl.ParseDirection(direction)));
}
