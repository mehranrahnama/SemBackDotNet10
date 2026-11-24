namespace SemBackDotNet10.SecurityDomain.Model.Entity;

public sealed record RefreshTokenResult(string Token, DateTimeOffset ExpiresAt);
