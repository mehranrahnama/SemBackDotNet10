using Common.repository;
using Common.service.interf;
namespace Common.service.impl;
public sealed class TaskExecutorServiceImpl(TaskExecutorRepository repository,ITaskEventStreamFactory streams) : TaskExecutorService
{
    private sealed class Entry(ITaskEventStream stream) { public ITaskEventStream Stream=stream; public bool Busy; }
    private readonly object gate=new();
    private readonly Dictionary<string,Entry> emitters=new();
    public ReportTaskStatus GetTaskStatus(long? planId,long? levelId,long? month,string storedProcedure)
    {
        var value=repository.FindReportsStatusByPlanIdAndLevelId(planId,levelId,month,storedProcedure);
        return new(value?.PlanId??planId,value?.LevelId??levelId,value?.StoredProcedureName??storedProcedure,value?.LastTimeUpdated);
    }
    public ITaskEventStream Subscribe(string emitterKey)
    {
        ArgumentException.ThrowIfNullOrEmpty(emitterKey);
        lock(gate) {
            if(emitters.TryGetValue(emitterKey,out var old)) {
                if(old.Busy) return old.Stream;
                emitters.Remove(emitterKey);
                old.Stream.Complete();
            }
            var entry=new Entry(streams.Create(TimeSpan.FromSeconds(300)));
            entry.Stream.Closed+=()=>{ lock(gate) { if(emitters.TryGetValue(emitterKey,out var current)&&ReferenceEquals(current,entry)) emitters.Remove(emitterKey); entry.Busy=false; } };
            emitters[emitterKey]=entry;
            return entry.Stream;
        }
    }
    public bool IsEmitterBusy(string emitterKey) { lock(gate) return emitters.TryGetValue(emitterKey,out var entry)&&entry.Busy; }
    public IReadOnlyDictionary<string,object> CreatingSseBody(string status)=>status switch {
        "START"=>new Dictionary<string,object>{{"event","started"},{"message","گزارش شما در حال آماده سازی میباشد."}},
        "SUCCESS"=>new Dictionary<string,object>{{"event","succeed"}},
        "FAILURE"=>new Dictionary<string,object>{{"event","failed"}},
        "COMPLETED"=>new Dictionary<string,object>{{"event","completed"}},
        _=>new Dictionary<string,object>()
    };
    public async Task RunTaskAsync(string emitterKey,Func<CancellationToken,Task> operation,CancellationToken cancellationToken=default)
    {
        ArgumentNullException.ThrowIfNull(operation);
        Entry entry;
        lock(gate) {
            if(!emitters.TryGetValue(emitterKey,out entry!)||entry.Busy) return;
            entry.Busy=true;
        }
        Task Send(string name,string status)=>entry.Stream.Send(new(name,Guid.NewGuid().ToString(),CreatingSseBody(status)),cancellationToken);
        try {
            await Send("message","START");
            await operation(cancellationToken);
            await Send("message","SUCCESS");
            await Send("alert","COMPLETED");
            entry.Stream.Complete();
        }
        catch(Exception error) {
            try { if(error is not OperationCanceledException) await Send("error","FAILURE"); } catch { /* Preserve the operation error. */ }
            entry.Stream.Complete(error);
            if(error is OperationCanceledException) throw;
        }
        finally { lock(gate) entry.Busy=false; }
    }
}
