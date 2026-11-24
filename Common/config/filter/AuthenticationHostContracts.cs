using System.Security.Claims;
using Microsoft.AspNetCore.Http;
namespace Common.config.filter;
public sealed record SemTokenIdentity(string? Username);
public sealed record SemUserIdentity(ClaimsPrincipal Principal,bool HasUser,bool HasCurrentOrgan);
public interface ISemTokenProvider
{
    string? GetJwtToken(HttpContext context);
    SemTokenIdentity? ValidateAndParse(string token);
    ClaimsPrincipal GetAuthentication(string token,SemUserIdentity user,HttpContext context);
}
public interface ISemUserDetails { SemUserIdentity? LoadByUsername(string username); }
public interface ICurrentSemUser { bool HasCurrentUser { get; } }
public interface IApiCredentialValidator { bool Validate(string key,string hash); }
public class SemJwtException(string message):Exception(message);
public sealed class SemExpiredJwtException(string message):SemJwtException(message);
public interface ISwitchUserHost
{
    // Must enforce administrator permission, preserve original identity and implement exit.
    Task<ClaimsPrincipal> Switch(HttpContext context,string accountName);
    Task<ClaimsPrincipal> Exit(HttpContext context);
    Task Success(HttpContext context);
}
public interface ISwitchUserAccounts
{
    string? FindForSwitch(string username);
    string CreateLdapAccount(string username);
}
