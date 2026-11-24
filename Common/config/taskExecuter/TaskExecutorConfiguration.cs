using System.Threading.Channels;
namespace Common.config.taskExecuter;
// Fixed 15 workers + bounded 100 queue. Not an emulation of Java's 15..50 elastic pool.
public sealed class TaskExecutorConfiguration:IAsyncDisposable
{
    public const int CorePoolSize=15,MaxPoolSize=50,QueueCapacity=100,KeepAliveSeconds=600;
    public const int JdbcQueryTimeoutSeconds=60000; // Java JdbcTemplate uses SECONDS, not milliseconds.
    private readonly Channel<(Func<Task> Work,ExecutionContext? Context,TaskCompletionSource Completion)> queue=
        Channel.CreateBounded<(Func<Task>,ExecutionContext?,TaskCompletionSource)>(new BoundedChannelOptions(QueueCapacity){FullMode=BoundedChannelFullMode.Wait});
    private readonly Task[] workers;
    public TaskExecutorConfiguration()=>workers=Enumerable.Range(0,CorePoolSize).Select(_=>Task.Run(Run)).ToArray();
    public Task Submit(Func<Task> work)
    {
        ArgumentNullException.ThrowIfNull(work);
        var completion=new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        if(!queue.Writer.TryWrite((work,ExecutionContext.Capture(),completion)))
            throw new InvalidOperationException("Task executor queue is full or shutting down.");
        return completion.Task;
    }
    private async Task Run()
    {
        await foreach(var item in queue.Reader.ReadAllAsync())
        {
            try
            {
                Task? task=null;
                if(item.Context is null) task=item.Work();
                else ExecutionContext.Run(item.Context,_=>task=item.Work(),null);
                await task!;item.Completion.SetResult();
            }
            catch(Exception ex){item.Completion.SetException(ex);}
        }
    }
    public async ValueTask DisposeAsync(){queue.Writer.TryComplete();await Task.WhenAll(workers);}
}
