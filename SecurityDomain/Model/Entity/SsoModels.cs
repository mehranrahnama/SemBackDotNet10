namespace SemBackDotNet10.SecurityDomain.Model.Entity;

public sealed record SsoSynchronizationResult(
    long UserAccountId,
    long? UserId,
    string Username,
    bool IsNewAccount);

public sealed class SsoUserDto
{
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? Username { get; init; }
    public string? Mobile { get; init; }
}
