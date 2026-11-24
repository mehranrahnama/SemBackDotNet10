#nullable enable
using SemBackDotNet10.basicDomain.Repositories;
using SemBackDotNet10.Common.Pagination;
using CommonPage = global::Common.utils.PageRequest;
using CommonDirection = SemBackDotNet10.Common.enums.SortDirection;

namespace SemBackDotNet10.service.impl;

// Query adapter shared by the planning services.
internal sealed partial class ServiceQueries(
    SemDbContext db,
    global::SemBackDotNet10.SecurityDomain.Tenancy.TenantContext tenantContext,
    global::SemBackDotNet10.basicDomain.Repositories.IDelegationRepository delegations,
    IMeasureUnitRepository measureUnits,
    IMainStrategyRepository mainStrategies,
    global::SemBackDotNet10.basicDomain.Repositories.IOrganizationRepository organizations,
    global::SemBackDotNet10.basicDomain.Repositories.IPerspectiveRepository perspectives,
    global::SemBackDotNet10.basicDomain.Repositories.IPlaningLevelRepository levels) : IServiceQueries
{
    public Task<List<StrategyMap>> StrategyMapReportAsync(long? planId, long? levelId, long? month, CancellationToken ct) =>
        new global::SemBackDotNet10.basicDomain.Repositories.StrategyMapRepository(db).StrategyMapReportAsync(planId, levelId, month, ct);

    public Task<List<StrategyMapDashboard>> StrategyMapDashboardReportAsync(long? planId, long? levelId, long? month, CancellationToken ct) =>
        new global::SemBackDotNet10.basicDomain.Repositories.StrategyMapDashboardRepository(db).StrategyMapDashboardReportAsync(planId, levelId, month, ct);

    public Task<List<global::SemBackDotNet10.basicDomain.Model.Entity.ReportMeasureDataByStrategyObjectId>> ReportMeasuresAsync(long? objectId, long? planId, long? levelId, long? month, bool allMonths, CancellationToken ct)
    {
        var repository = new global::SemBackDotNet10.basicDomain.Repositories.ReportMeasureDataByStrategyObjectIdRepository(db);
        return allMonths ? repository.ReportMeasureDataByStrategyObjectIdAllMonthAsync(objectId, planId, levelId, ct)
            : repository.ReportMeasureDataByStrategyObjectIdAsync(objectId, planId, levelId, month, ct);
    }

    public Task<List<global::SemBackDotNet10.basicDomain.Model.Entity.ReportActionDataByStrategyObjectId>> ReportActionsAsync(long? objectId, long? planId, long? levelId, long? month, bool allMonths, CancellationToken ct)
    {
        var repository = new global::SemBackDotNet10.basicDomain.Repositories.ReportActionDataByStrategyObjectIdRepository(db);
        return allMonths ? repository.ReportActionDataByStrategyObjectIdAllMonthAsync(objectId, planId, levelId, ct)
            : repository.ReportActionDataByStrategyObjectIdAsync(objectId, planId, levelId, month, ct);
    }

    public Task<long> CountActiveStrategyAsync(long? planId, long? levelId, CancellationToken ct) =>
        levelId.HasValue ? mainStrategies.CountActiveStrategyByLevelIdAsync(levelId.Value, ct)
            : mainStrategies.CountActiveStrategyAsync(planId.GetValueOrDefault(), ct);

    public Task<List<long>> UserDelegationsAsync(long userId, DateTime instant, CancellationToken ct) =>
        delegations.FindActiveToUserIdsAsync(userId, instant, ct);

    private static CommonPage ToPageRequest(ServicePageRequest page)
    {
        _ = page.Offset;
        var direction = page.Direction switch
        {
            SortDirection.Ascending => CommonDirection.Asc,
            SortDirection.Descending => CommonDirection.Desc,
            _ => throw new ArgumentOutOfRangeException(nameof(page))
        };
        return global::Common.utils.PageUtils.GetInstance(page.Page, page.Size, page.OrderBy, direction);
    }

    private static ServicePage<T> ToServicePage<T>(PagedResult<T> page) =>
        new(page.Items, page.TotalElements, page.Page, page.Size);

    private static ServicePage<T> ToServicePage<T>(PlaningPagedResult<T> page) =>
        new(page.Items, page.TotalElements, page.Page, page.Size);

    public async Task<ServicePage<MeasureUnitDto>> MeasureUnitsAsync(string? filter, ServicePageRequest page, CancellationToken ct) =>
        ToServicePage(await measureUnits.FindAllMeasureUnitsAsync(filter, ToPageRequest(page), ct));

    public async Task<ServicePage<OrganizationDto>> OrganizationsAsync(string? filter, ServicePageRequest page, CancellationToken ct) =>
        ToServicePage(await organizations.SearchByDesAsync(filter!, ToPageRequest(page), ct));

    public async Task<ServicePage<OrganizationDto>> PositionsWithModirAsync(string? filter, bool isCommittee, ServicePageRequest page, CancellationToken ct) =>
        ToServicePage(isCommittee
            ? await organizations.CommitteeAllWithModirInfoAsync(filter, ToPageRequest(page), ct)
            : await organizations.OrganizationAllWithModirInfoAsync(filter, ToPageRequest(page), ct));

    public async Task<ServicePage<MainStrategyDto>> MainStrategiesAsync(string? filter, long? levelId, bool filterByLevel, ServicePageRequest page, CancellationToken ct) =>
        ToServicePage(filterByLevel
            ? await mainStrategies.FindAllMainStrategiesByLevelIdAsync(filter, levelId, ToPageRequest(page), ct)
            : await mainStrategies.FindAllMainStrategiesAsync(filter, ToPageRequest(page), ct));

    public async Task<ServicePage<PlaningLevelDto>> LevelsAsync(string? filter, ServicePageRequest page, CancellationToken ct) =>
        ToServicePage(await levels.FindAllPlaningLevelsAsync(filter!, ToPageRequest(page), ct));

    public async Task<ServicePage<PlaningLevelDto>> CurrentUserLevelsAsync(string? filter, List<long> ids, long? parentId, bool filterByParent, ServicePageRequest page, CancellationToken ct) =>
        ToServicePage(filterByParent
            ? await levels.FindCurrentUserLevelsByParentLevelIdAsync(filter!, ids, parentId, ToPageRequest(page), ct)
            : await levels.FindCurrentUserLevelsAsync(filter!, ids, ToPageRequest(page), ct));

    public async Task<ServicePage<PerspectiveDto>> PerspectivesAsync(long levelId, long planId, string? filter, ServicePageRequest page, CancellationToken ct) =>
        ToServicePage(await perspectives.FindByLevelIdAndPlanIdAsync(levelId, planId, filter, ToPageRequest(page), ct));
}
