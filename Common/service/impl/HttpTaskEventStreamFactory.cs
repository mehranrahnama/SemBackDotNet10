using System.Diagnostics;
using System.Text.Json;
using Common.service.interf;
using Microsoft.AspNetCore.Http;

namespace Common.service.impl;

public sealed class HttpTaskEventStreamFactory(IHttpContextAccessor requests) : ITaskEventStreamFactory
{
    public ITaskEventStream Create(TimeSpan timeout) => new HttpTaskEventStream(
        requests.HttpContext ?? throw new InvalidOperationException("SSE requires an active HTTP request."), timeout);

    private sealed class HttpTaskEventStream(HttpContext context, TimeSpan timeout) : ITaskEventStream
    {
        private readonly long started = Stopwatch.GetTimestamp();
        private int closed;
        public event Action? Closed;

        public async Task Send(TaskStreamEvent value, CancellationToken cancellationToken)
        {
            if (Volatile.Read(ref closed) != 0) throw new InvalidOperationException("The SSE stream is closed.");
            var remaining = timeout - Stopwatch.GetElapsedTime(started);
            if (remaining <= TimeSpan.Zero) throw new TimeoutException("The SSE stream expired.");
            using var deadline = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, context.RequestAborted);
            deadline.CancelAfter(remaining);
            var frame = $"event: {value.Name}\nid: {value.Id}\ndata: {JsonSerializer.Serialize(value.Body)}\n\n";
            await context.Response.WriteAsync(frame, deadline.Token);
            await context.Response.Body.FlushAsync(deadline.Token);
        }

        public void Complete(Exception? error = null)
        {
            if (Interlocked.Exchange(ref closed, 1) == 0) Closed?.Invoke();
        }
    }
}
