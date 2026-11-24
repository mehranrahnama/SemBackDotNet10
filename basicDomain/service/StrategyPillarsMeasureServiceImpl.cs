using Common.exception;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.Base.model.dto;
using SemBackDotNet10.Models.Entities;
using SemBackDotNet10.baseDomain.repository;

namespace SemBackDotNet10.service.impl;

public sealed class StrategyPillarsMeasureServiceImpl(SemDbContext db)
{
    private readonly StrategyPillarsMeasureRepository repository = new(db);

    public async Task SaveAsync(List<StrategyPillarsMeasureDto> rows, bool edit, CancellationToken ct)
    {
        foreach (var dto in rows)
        {
            var row = edit ? await FindAsync(dto.Id, ct) : new StrategyPillarsMeasure();
            if (!edit || dto.StrategyPillarsId.GetValueOrDefault() != 0)
            {
                var id = dto.StrategyPillarsId;
                if (!await db.Set<StrategyPillar>().AnyAsync(x => x.Id == id, ct))
                    throw new BusinessException("error.StrategyPillar.idIsNotValid");
                row.StrategyPillarsId = id!.Value;
            }
            if (!edit || dto.MeasureUnitId.GetValueOrDefault() != 0)
            {
                var id = dto.MeasureUnitId;
                if (!await db.Set<MeasureUnit>().AnyAsync(x => x.Id == id, ct))
                    throw new BusinessException("error.MeasureUnit.idIsNotValid");
                row.MeasureUnitId = id!.Value;
            }
            if (!edit || !string.IsNullOrEmpty(dto.MeasureName)) row.MeasureName = dto.MeasureName ?? "";
            if (!edit || !string.IsNullOrEmpty(dto.MeasurePlan)) row.MeasurePlan = dto.MeasurePlan;
            if (!edit || dto.Year.GetValueOrDefault() != 0) row.Year = dto.Year.GetValueOrDefault();
            row.Deleted = edit && dto.IsDeleted;
            if (!edit) db.Add(row);
        }
        await db.SaveChangesAsync(ct);
    }

    private async Task<StrategyPillarsMeasure> FindAsync(long? id, CancellationToken ct)
        => (id is long key ? await repository.FindByStrategyPillarsMeasureIdAsync(key, ct) : null)
            ?? throw new BusinessException("شاخص ارکان استراتژی یافت نشد");

    public async Task<StrategyPillarsMeasureDto> GetAsync(long id, CancellationToken ct)
    {
        var row = await FindAsync(id, ct);
        return new() { Id = row.Id, StrategyPillarsId = row.StrategyPillarsId, MeasureName = row.MeasureName,
            MeasurePlan = row.MeasurePlan, MeasureUnitId = row.MeasureUnitId, Year = row.Year, IsDeleted = row.Deleted };
    }

    public async Task<SearchDto> PageAsync(long id, string? filter, int page, int size, CancellationToken ct)
    {
        _ = new PageRequest(page, size).Offset;
        var rows = await repository.FindAllStrategyPillarsMeasureByStrategyPillarsIdAsync(filter, id, ct);
        var groups = rows.GroupBy(x => (x.MeasureName, x.StrategyPillarsId)).Select(g =>
        {
            var first = g.First();
            return new StrategyPillarsMeasurePageDto { StrategyPillarsId = first.StrategyPillarsId, MeasureName = first.MeasureName,
                MeasureUnitId = first.MeasureUnitId, MeasureUnitName = first.MeasureUnit?.DesMeasureF, IsDeleted = first.Deleted,
                Ids = g.Select(x => x.Id).ToList(), Year = g.Select(x => x.Year).ToList(), MeasurePlan = g.Select(x => x.MeasurePlan).ToList() };
        }).ToList();
        return new SearchDto.Builder().content(groups).totalElements(groups.Count)
            .totalPages((int)Math.Ceiling((double)groups.Count / size)).currentPage(page).pageSize(size).build();
    }

    public async Task SoftDeleteAsync(string ids, CancellationToken ct)
    {
        foreach (var id in ids.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).Distinct())
            (await FindAsync(id, ct)).Deleted = true;
        await db.SaveChangesAsync(ct);
    }
}
