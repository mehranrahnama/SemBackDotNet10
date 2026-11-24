using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SemBackDotNet10.service.impl;

public sealed class PlanPeriod { public int? Id { get; set; } public long PlanId { get; set; } public short StatusType { get; set; } }
public sealed record PlanPeriodDto(int? Id, long PlanId, short StatusType);
public interface IPlanPeriodRepository
{
    Task<PlanPeriod?> FindByIdAsync(int id, CancellationToken ct = default);
    Task<PlanPeriod?> FindByPlanIdAsync(long planId, CancellationToken ct = default);
    Task<PlanPeriod?> FindActiveAsync(CancellationToken ct = default);
    Task<int> CountActiveAsync(CancellationToken ct = default);
    Task<PageResult<PlanPeriodDto>> FindAllAsync(string? filter, int page, int size, string orderBy, SortDirection direction, CancellationToken ct = default);
    Task<PlanPeriod> SaveAsync(PlanPeriod period, CancellationToken ct = default);
    Task<IReadOnlyList<PlanPeriod>> FindAllByPlanIdsAsync(IReadOnlyCollection<long> planIds, CancellationToken ct = default);
}

/// <summary>Manages plan periods; the repository enforces persistence, while this service applies active-period rules.</summary>
public sealed class PlanPeriodServiceImpl(IPlanPeriodRepository planPeriodRepository, IServiceLogger? logger = null)
{
    private readonly IServiceLogger _logger = logger ?? new NullServiceLogger();
    public IPlanPeriodRepository Repository => planPeriodRepository;
    public async Task<PlanPeriod> CreateAsync(PlanPeriodDto dto, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(dto);
        return await planPeriodRepository.SaveAsync(new PlanPeriod { PlanId = dto.PlanId, StatusType = dto.StatusType }, ct);
    }
    public async Task<PlanPeriod> CreatePlanPeriodByPlanIdAsync(long planId, CancellationToken ct = default)
    {
        var existing = await FindPlanPeriodByPlanIdAsync(planId, ct);
        if (existing is not null) return existing;
        try { return await CreateAsync(new(null, planId, 3), ct); }
        catch (Exception ex) when (ex is not OperationCanceledException) { _logger.Error("Could not create plan period", ex); throw new ServiceException(ex.Message); }
    }
    public static string? GetStatusTypeDescription(short statusType) => statusType switch { 2 => "فعال", 3 => "مختومه", _ => null };
    public static PlanPeriodDto MapToDto(PlanPeriod period) => new(period.Id, period.PlanId, period.StatusType);
    public async Task<SearchResult<PlanPeriodDto>> FindAllAsync(string? filter, int page, int size, string orderBy = "statusType", SortDirection direction = SortDirection.Ascending, CancellationToken ct = default)
    {
        if (page < 0 || size <= 0) throw new ArgumentOutOfRangeException(nameof(page));
        if (!string.IsNullOrWhiteSpace(filter) && filter.TrimStart().StartsWith('{'))
        { using var doc = System.Text.Json.JsonDocument.Parse(filter); filter = doc.RootElement.TryGetProperty("filter", out var v) ? v.GetString() : null; }
        var result = await planPeriodRepository.FindAllAsync(filter, page, size, orderBy, direction, ct);
        return new(result.Items, result.TotalElements, result.TotalPages, page, size);
    }
    public async Task<PlanPeriod> EditAsync(PlanPeriodDto dto, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(dto);
        if (dto.StatusType == 2 && await planPeriodRepository.CountActiveAsync(ct) >= 1) throw new ServiceException("error.planperiod.already-active");
        if (dto.Id is null) throw new ServiceException("error.user.idIsNotValid");
        var period = await planPeriodRepository.FindByIdAsync(dto.Id.Value, ct) ?? throw new ServiceException("error.user.idIsNotValid");
        period.PlanId = dto.PlanId; period.StatusType = dto.StatusType;
        return await planPeriodRepository.SaveAsync(period, ct);
    }
    public async Task<PlanPeriod> FindByIdOrElseThrowAsync(int? id, CancellationToken ct = default)
    { if (id is null) throw new ServiceException("error.user.idIsNotValid"); return await planPeriodRepository.FindByIdAsync(id.Value, ct) ?? throw new ServiceException("error.user.idIsNotValid"); }
    public async Task<PlanPeriod> FindByPlanIdAsync(long planId, CancellationToken ct = default) => await planPeriodRepository.FindByPlanIdAsync(planId, ct) ?? throw new ServiceException("error.planPeriod.notFound");
    public Task<PlanPeriod?> FindPlanPeriodByPlanIdAsync(long planId, CancellationToken ct = default) => planPeriodRepository.FindByPlanIdAsync(planId, ct);
    public async Task<PlanPeriod> FindByPlanIdOrCreateAsync(long planId, CancellationToken ct = default) => await planPeriodRepository.FindByPlanIdAsync(planId, ct) ?? await CreatePlanPeriodByPlanIdAsync(planId, ct);
    public async Task<PlanPeriod> FindActivePlanPeriodAsync(CancellationToken ct = default)
    { try { return await planPeriodRepository.FindActiveAsync(ct) ?? throw new InvalidOperationException("No active plan period."); } catch (Exception ex) when (ex is not OperationCanceledException) { _logger.Error("Error finding active plan period", ex); throw new ServiceException("خطا در دریافت دوره برنامه فعال."); } }
}
