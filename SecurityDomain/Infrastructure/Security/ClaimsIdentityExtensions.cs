using SemBackDotNet10.SecurityDomain.Model.Entity;
using System.Security.Claims;

namespace SemBackDotNet10.SecurityDomain.Infrastructure.Security;

public static class ClaimsIdentityExtensions
{
    public static void AddCustomRole(this ClaimsIdentity identity, CustomRole role)
    {
        ArgumentNullException.ThrowIfNull(identity);
        ArgumentNullException.ThrowIfNull(role);
        identity.AddClaims(role.ToClaims());
    }
}
