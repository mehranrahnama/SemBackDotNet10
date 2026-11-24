using Rp = SemBackDotNet10.baseDomain.repository;
using Entity = SemBackDotNet10.basicDomain.Model.Entity.PlanPeriod;

namespace SemBackDotNet10.service.impl;

public sealed class PlanPeriodRepositoryAdapter(SemDbContext db) : IPlanPeriodRepository
{
    private readonly Rp.PlanPeriodRepository repository = new(db);

    public async Task<PlanPeriod?> FindByIdAsync(int id, CancellationToken ct = default)
        => Map(await repository.FindByIdAsync(id, ct));
    public async Task<PlanPeriod?> FindByPlanIdAsync(long planId, CancellationToken ct = default)
        => Map(await repository.FindByPlanIdAsync(planId, ct));
    public async Task<PlanPeriod?> FindActiveAsync(CancellationToken ct = default)
        => Map(await repository.FindByStatusTypeIsActiveAsync(ct));
    public Task<int> CountActiveAsync(CancellationToken ct = default) => repository.CountActivePlanPeriodsAsync(ct);
    public async Task<PageResult<PlanPeriodDto>> FindAllAsync(string? filter, int page, int size, string orderBy, SortDirection direction, CancellationToken ct = default)
    {
        var result = await repository.FindAllPlanPeriodsAsync(new Rp.PageRequest(page, size), filter, ct);
        return new(result.Content.Select(x => new PlanPeriodDto(x.Id, x.PlanId!.Value, x.StatusType)).ToList(), result.TotalElements, checked((int)result.TotalPages));
    }
    public async Task<PlanPeriod> SaveAsync(PlanPeriod period, CancellationToken ct = default)
    {
        var entity = period.Id is int id ? await repository.FindByIdAsync(id, ct)
            ?? throw new global::SemBackDotNet10.Common.exception.BusinessException("error.planPeriod.notFound") : new Entity();
        entity.PlanId = period.PlanId;
        entity.StatusType = period.StatusType;
        return Map(await repository.SaveAsync(entity, ct))!;
    }
    public async Task<IReadOnlyList<PlanPeriod>> FindAllByPlanIdsAsync(IReadOnlyCollection<long> planIds, CancellationToken ct = default)
        => (await repository.FindAllByPlanIdInAsync(planIds, ct)).Select(x => Map(x)!).ToList();
    public Task DeleteAsync(int id, CancellationToken ct) => repository.DeleteByIdAsync(id, ct);

    private static PlanPeriod? Map(Entity? entity) => entity is null ? null : new()
    { Id = entity.Id, PlanId = entity.PlanId, StatusType = entity.StatusType };
}
