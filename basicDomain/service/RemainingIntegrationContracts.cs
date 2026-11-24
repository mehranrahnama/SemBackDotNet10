#nullable enable
using DomainStrategyDto = SemBackDotNet10.basicDomain.Model.Dto.StrategyObjectDto;
using DomainStrategyObject = SemBackDotNet10.basicDomain.Model.Entity.StrategyObject;

using SemBackDotNet10.actionDomain.model.entity;
using RoleDto = SemBackDotNet10.SecurityDomain.Model.Dto.RoleDto;

namespace SemBackDotNet10.service.impl;

// Required adapters: use the same tenant-scoped DbContext as ServiceRuntime.
// No default/no-op implementations are supplied. Token mutations are atomic with account changes.
public interface IRemainingServiceQueries
{
    Task<bool> IsPlanActiveAsync(long? planId, CancellationToken ct);
    Task<ServicePage<DomainStrategyDto>> StrategyObjectsAsync(string? filter, long? levelId, long? planId, long? perspectiveId, ServicePageRequest page, CancellationToken ct);
    Task<long> StrategyCountAsync(long? planId, long? levelId, CancellationToken ct);
    Task<bool> StrategyHasDependenciesAsync(long id, CancellationToken ct);
    Task<List<ActionPlan>> ActionPlansByCatalogAndPeriodAsync(ActionPlan source, CancellationToken ct);
    Task<ServicePage<User>> UsersAsync(string tenant, string? filter, long? levelId, ServicePageRequest page, CancellationToken ct);
    Task<Tenant?> UserTenantAsync(string username, CancellationToken ct);
    Task<long> ActiveUserCountAsync(long? planId, long? levelId, CancellationToken ct);
    Task<UserDto?> UserDetailsAsync(string accountName, CancellationToken ct);
    Task<List<RoleDto>> UserRolesAsync(long userId, CancellationToken ct);
    Task<bool> HasRoleAsync(long userId, string role, CancellationToken ct);
    Task<ServicePage<UserDto>> UsersByRoleAsync(string role, string? filter, ServicePageRequest page, CancellationToken ct);
}
public interface IRemainingExternalServices
{
    Task CopyActionPlanAsync(DomainStrategyObject target, long planId, ActionPlan source, CancellationToken ct);
    // Queue a notification in a transactional outbox. Never send plaintext passwords.
    Task QueueNewUserNotificationAsync(User user, CancellationToken ct);
    Task DeleteUserAccountsRolesMembershipsAndTokensAsync(long userId, CancellationToken ct);
    // Blacklist all refresh tokens, delete them, and clean expired refresh/blacklist tokens.
    Task RevokeUserTokensAsync(long userId, CancellationToken ct);
    Task<List<User>> ReadDirectoryUsersAsync(IReadOnlyList<string> attributes, int countLimit, CancellationToken ct);
    // Create LDAP-provider accounts named username + "_IRISA_LDAP"; idempotent, unique per provider/account.
    Task EnsureLdapAccountsAsync(IReadOnlyList<User> users, CancellationToken ct);
}
