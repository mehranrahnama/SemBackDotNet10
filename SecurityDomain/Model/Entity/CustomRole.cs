using System.Security.Claims;

namespace SemBackDotNet10.SecurityDomain.Model.Entity;

public sealed record CustomRole
{
    public string Role { get; }
    public AuthenticationSource Source { get; }

    public CustomRole(string role, AuthenticationSource source)
    {
        if (string.IsNullOrWhiteSpace(role))
            throw new ArgumentException("Role cannot be empty.", nameof(role));

        Role = role;
        Source = source;
    }

    public IEnumerable<Claim> ToClaims()
    {
        yield return new Claim(ClaimTypes.Role, Role);
        yield return new Claim("authentication_source", Source.ToString());
    }
}
