using Common.exception;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
namespace Common.utils.security;

// Host must map its authenticated SemUserDetail and switch-user authorities explicitly.
public sealed record SecurityUser(long? Id, IReadOnlyCollection<string> RoleNames);
public sealed record SecurityAccount(string? AccountName);
public sealed record SecurityOrgan(long? Id);
public interface ISecurityContextHost
{
    ClaimsPrincipal? Authentication { get; }
    SecurityUser? CurrentUser { get; }
    SecurityAccount? CurrentAccount { get; }
    SecurityOrgan? CurrentOrgan { get; }
    ClaimsPrincipal? Impersonator { get; }
}
public sealed class SecurityUtils(ISecurityContextHost host, IHttpContextAccessor requests)
{
    public HttpRequest? GetCurrentRequest()=>requests.HttpContext?.Request;
    public ClaimsPrincipal? GetAuthentication()=>host.Authentication;
    public SecurityUser? GetCurrentUser()=>host.Authentication is null ? null : host.CurrentUser;
    public SecurityAccount? GetCurrentAccount()=>GetCurrentUser() is null ? null : host.CurrentAccount;
    public SecurityOrgan? GetCurrentOrgan()=>host.Authentication is null ? null : host.CurrentOrgan;
    public long? GetCurrentOrganIdOrElseThrow()=>(GetCurrentOrgan() ?? throw new BusinessException("error.security.currentOrganNotFound")).Id;
    public SecurityUser GetCurrentUserOrElseThrow()=>GetCurrentUser() ?? throw new BusinessException("error.security.currentUserNotFound");
    public SecurityAccount GetCurrentAccountOrElseThrow()=>GetCurrentAccount() ?? throw new BusinessException("error.security.currentUserNotFound");
    public string? GetUsernameOfCurrentUser()=>GetCurrentAccount()?.AccountName;
    public long? GetUserIdOfCurrentUserOrElseThrow()=>GetCurrentUserOrElseThrow().Id;
    public ClaimsPrincipal? GetImpersonator()=>host.Authentication is null ? null : host.Impersonator;
    public bool IsImpersonating()=>GetImpersonator() is not null;
}
