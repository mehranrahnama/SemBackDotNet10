namespace SemBackDotNet10.SecurityDomain.Authentication
{
    public sealed record AuthenticationResult(
      bool IsAuthenticated,
      long TenantId,
      string Username,
      string? DisplayName = null,
      string? ProviderType = null,
      string? ErrorCode = null,
      long? UserId = null,
      long? AccountId = null,
      IReadOnlyCollection<string>? Roles = null,
      IReadOnlyCollection<long>? RoleIds = null)
    {
        public static AuthenticationResult Success(
            long tenantId,
            string username,
            string? displayName,
            string providerType,
            long? userId = null,
            long? accountId = null,
            IReadOnlyCollection<string>? roles = null,
            IReadOnlyCollection<long>? roleIds = null)
        {
            return new AuthenticationResult(
                true,
                tenantId,
                username,
                displayName,
                providerType,
                UserId: userId,
                AccountId: accountId,
                Roles: roles,
                RoleIds: roleIds);
        }

        public static AuthenticationResult Failed(
            long tenantId,
            string username,
            string errorCode)
        {
            return new AuthenticationResult(
                false,
                tenantId,
                username,
                ErrorCode: errorCode);
        }
    }
}