using Common.model.entity;
namespace Common.service.interf;
public sealed record ReportTaskStatus(long? PlanId,long? LevelId,string? StoredProcedureName,DateTime? LastTimeUpdated);
public sealed record TaskStreamEvent(string Name,string Id,IReadOnlyDictionary<string,object> Body);
public interface ITaskEventStream
{
    // Host supplies real SSE framing, 300-second timeout and disconnect callbacks.
    event Action? Closed;
    Task Send(TaskStreamEvent value,CancellationToken cancellationToken);
    void Complete(Exception? error=null);
}
public interface ITaskEventStreamFactory { ITaskEventStream Create(TimeSpan timeout); }
public interface TaskExecutorService
{
    ReportTaskStatus GetTaskStatus(long? planId,long? levelId,long? month,string storedProcedure);
    ITaskEventStream Subscribe(string emitterKey);
    bool IsEmitterBusy(string emitterKey);
    Task RunTaskAsync(string emitterKey,Func<CancellationToken,Task> operation,CancellationToken cancellationToken=default);
    IReadOnlyDictionary<string,object> CreatingSseBody(string status);
}
