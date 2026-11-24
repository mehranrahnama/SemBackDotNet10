using System.Security.Claims;

namespace SemBackDotNet10.SecurityDomain.Model.SemUserDetail;

/// <summary>ASP.NET Core equivalent of the Java impersonation principal.</summary>
public sealed class ImpersonateUser
{
    private readonly IReadOnlyCollection<Claim> _claims;

    public ImpersonateUser(
        string userName,
        long userId,
        string? password,
        IEnumerable<string>? authorities = null)
    {
        UserName = userName;
        UserId = userId;
        Password = password;
        _claims = (authorities ?? [])
            .Where(a => !string.IsNullOrWhiteSpace(a))
            .Distinct(StringComparer.Ordinal)
            .OrderBy(a => a, StringComparer.Ordinal)
            .Select(a => new Claim(ClaimTypes.Role, a))
            .ToArray();
    }

    public long UserId { get; }
    public string UserName { get; }
    public string? Password { get; private set; }
    public IReadOnlyCollection<Claim> Claims => _claims;
    public bool IsEnabled => true;

    public ClaimsPrincipal ToPrincipal() =>
        new(new ClaimsIdentity(
            new[] { new Claim(ClaimTypes.NameIdentifier, UserId.ToString()), new Claim(ClaimTypes.Name, UserName) }
                .Concat(_claims),
            authenticationType: "Impersonation"));

    public void EraseCredentials() => Password = null;
}
