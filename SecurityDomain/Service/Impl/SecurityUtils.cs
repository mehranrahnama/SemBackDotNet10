using Microsoft.AspNetCore.Http;
using SemBackDotNet10.Common.exception;
using System.Security.Claims;

namespace SemBackDotNet10.SecurityDomain.Service.Impl
{
  



    /// <summary>
    /// معادل SecurityUtils جاوا: دسترسی به کاربر جاری از طریق IHttpContextAccessor.
    /// در Program.cs باید SecurityUtils.Configure(accessor) صدا زده شود.
    /// </summary>
    public static class SecurityUtils
    {
        private static IHttpContextAccessor? _httpContextAccessor;

        public static void Configure(IHttpContextAccessor accessor) => _httpContextAccessor = accessor;

        public static ClaimsPrincipal? GetCurrentUser() =>
            _httpContextAccessor?.HttpContext?.User;

        public static long? GetCurrentUserId()
        {
            var user = GetCurrentUser();
            if (user?.Identity?.IsAuthenticated != true) return null;

            var claim = user.FindFirst("user_id")
                        ?? user.FindFirst("sub")
                        ?? user.FindFirst(ClaimTypes.NameIdentifier)
                        ?? user.FindFirst("userId");

            return long.TryParse(claim?.Value, out var id) ? id : null;
        }

        public static string? GetCurrentUserName() =>
            GetCurrentUser()?.Identity?.Name;
    }

    /// <summary>معادل SemSecurityUtilityService جاوا.</summary>
    public sealed class SemSecurityUtilityService
    {
        public const string AdminRole = "Administrators";

        /// <summary>معادل getCurrentUserId().</summary>
        public long GetCurrentUserId() =>
            SecurityUtils.GetCurrentUserId()
            ?? throw new UnAuthorizedException();

        /// <summary>معادل getUserFromAuthentication().getRole()</summary>
        public IReadOnlyCollection<string> GetCurrentUserRoles()
        {
            var user = SecurityUtils.GetCurrentUser();
            if (user is null) return Array.Empty<string>();

            return user.Claims
                .Where(c => c.Type == ClaimTypes.Role || c.Type == "role")
                .Select(c => c.Value)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        public bool IsCurrentUserAdmin() =>
            GetCurrentUserRoles().Contains(AdminRole, StringComparer.OrdinalIgnoreCase);
    }

}
