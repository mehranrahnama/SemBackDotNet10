namespace Common.utils;
// Instance-scoped bridge, deliberately not a process-wide mutable service locator.
public sealed class SpringContext(IServiceProvider services)
{
    public T GetBean<T>() where T:class => services.GetService(typeof(T)) as T
        ?? throw new InvalidOperationException($"Required service {typeof(T).FullName} is not registered.");
}
