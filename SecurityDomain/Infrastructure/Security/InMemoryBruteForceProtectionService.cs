using System.Collections.Concurrent;
using SemBackDotNet10.SecurityDomain.Interfaces;

namespace SemBackDotNet10.SecurityDomain.Infrastructure.Security;

public sealed class InMemoryBruteForceProtectionService(IConfiguration configuration) : IBruteForceProtectionService
{
    private readonly ConcurrentDictionary<string, Failure> _failures = new(StringComparer.OrdinalIgnoreCase);
    private readonly int _limit = configuration.GetValue("Security:BruteForce:MaxFailures", 5);
    private readonly TimeSpan _window = TimeSpan.FromMinutes(configuration.GetValue("Security:BruteForce:WindowMinutes", 15));

    public bool IsBlocked(string key) => _failures.TryGetValue(key, out var failure) && failure.ExpiresAt > DateTimeOffset.UtcNow && failure.Count >= _limit;

    public void RegisterFailure(string key) => _failures.AddOrUpdate(key, _ => new(1, DateTimeOffset.UtcNow.Add(_window)), (_, current) => current.ExpiresAt <= DateTimeOffset.UtcNow ? new(1, DateTimeOffset.UtcNow.Add(_window)) : current with { Count = current.Count + 1 });

    public void RegisterSuccess(string key) => _failures.TryRemove(key, out _);

    private sealed record Failure(int Count, DateTimeOffset ExpiresAt);
}
