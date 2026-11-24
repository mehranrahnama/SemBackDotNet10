using global::SemBackDotNet10.basicDomain.Model.Entity;
#nullable enable

namespace SemBackDotNet10.service.impl;

// Implement with repository DTO projections and parameterized report SQL; never return placeholder results.
public interface IServiceQueries
{
    Task<ServicePage<DelegationDto>> DelegationsAsync(string tenant, long userId, string? description, long? alternativeUserId, DateTime? startDate, DateTime? endDate, ServicePageRequest page, CancellationToken ct);
    Task<List<Delegation>> DelegationsByUserIdAsync(long userId, CancellationToken ct);
    Task<List<Delegation>> AllDelegationsAsync(string tenant, CancellationToken ct);
    Task<List<Delegation>> ActiveDelegationsAsync(string tenant, CancellationToken ct);
    Task<bool> HasActiveDelegationAsync(long fromId, long toId, CancellationToken ct);
    Task<List<long>> UserDelegationsAsync(long userId, DateTime instant, CancellationToken ct);
    Task<ServicePage<MainStrategyDto>> MainStrategiesAsync(string? filter, long? levelId, bool filterByLevel, ServicePageRequest page, CancellationToken ct);
    Task<long> CountActiveStrategyAsync(long? planId, long? levelId, CancellationToken ct);
    Task<ServicePage<MeasureUnitDto>> MeasureUnitsAsync(string? filter, ServicePageRequest page, CancellationToken ct);
    Task<ServicePage<OrganizationDto>> OrganizationsAsync(string? filter, ServicePageRequest page, CancellationToken ct);
    Task<ServicePage<OrganizationDto>> PositionsWithModirAsync(string? filter, bool isCommittee, ServicePageRequest page, CancellationToken ct);
    Task<ServicePage<PlaningLevelOrganizationDto>> LevelOrganizationsAsync(string? filter, long? levelId, bool filterByLevel, ServicePageRequest page, CancellationToken ct);
    Task<ServicePage<PlaningLevelTeamDto>> LevelTeamsAsync(string? filter, long? levelId, bool filterByLevel, ServicePageRequest page, CancellationToken ct);
    Task<ServicePage<PlaningLevelDto>> LevelsAsync(string? filter, ServicePageRequest page, CancellationToken ct);
    Task<ServicePage<PlaningLevelDto>> CurrentUserLevelsAsync(string? filter, List<long> ids, long? parentId, bool filterByParent, ServicePageRequest page, CancellationToken ct);
    Task<ServicePage<PerspectiveDto>> PerspectivesAsync(long levelId, long planId, string? filter, ServicePageRequest page, CancellationToken ct);
    Task<List<StrategyMap>> StrategyMapReportAsync(long? planId, long? levelId, long? month, CancellationToken ct);
    Task<List<StrategyMapDashboard>> StrategyMapDashboardReportAsync(long? planId, long? levelId, long? month, CancellationToken ct);
    Task<List<ReportMeasureDataByStrategyObjectId>> ReportMeasuresAsync(long? objectId, long? planId, long? levelId, long? month, bool allMonths, CancellationToken ct);
    Task<List<ReportActionDataByStrategyObjectId>> ReportActionsAsync(long? objectId, long? planId, long? levelId, long? month, bool allMonths, CancellationToken ct);
}

public interface IExternalDomainServices
{
    Task EnsureLevelMembershipAsync(long userId, long levelId, CancellationToken ct);
    Task<User> GetUserAsync(long id, CancellationToken ct);
    Task<List<long>> GetUserLevelIdsAsync(string source, long userId, CancellationToken ct);
    Task<global::SemBackDotNet10.basicDomain.Model.Entity.PlanPeriod?> FindActivePlanPeriodAsync(CancellationToken ct);
    Task<global::SemBackDotNet10.basicDomain.Model.Entity.PlanPeriod> FindPlanPeriodByPlanIdAsync(long? planId, CancellationToken ct);
    Task AddDabirComiteRoleAsync(long userId, CancellationToken ct);
    Task AddRaeesComiteAsync(long userId, string tenant, CancellationToken ct);
    Task AddModirUserRoleAsync(long userId, CancellationToken ct);
    Task AddTashilgarUserRoleAsync(long userId, CancellationToken ct);
    Task RemoveRaeesComiteAsync(long userId, CancellationToken ct);
    Task RemoveDabirComiteRoleAsync(long userId, CancellationToken ct);
    // Domain event must be transactional/outbox-backed, not published before commit.
    Task ChangeFacilitatorAsync(PlaningLevel level, long? newFacilitator, long? oldFacilitator, long? newResponsible, long? oldResponsible, CancellationToken ct);
    Task<IReadOnlyList<CommitteeImport>> GetCommitteeTahavolAsync(CancellationToken ct);
    Task<List<StrategyObject>> DeletedStrategiesByPerspectiveAsync(long id, CancellationToken ct);
    Task<bool> HasActiveStrategiesByPerspectiveAsync(long id, CancellationToken ct);
    Task HardDeleteActionPlansAsync(List<StrategyObject> strategies, CancellationToken ct);
    Task HardDeleteMeasuresAsync(List<StrategyObject> strategies, CancellationToken ct);
}
public sealed record CommitteeMember(int Role, long PersonalCode);
public sealed record CommitteeUnit(string Code, string Name);
public sealed record CommitteeImport(string Code, string Name, IReadOnlyList<CommitteeMember> Members, IReadOnlyList<CommitteeUnit> Units);
