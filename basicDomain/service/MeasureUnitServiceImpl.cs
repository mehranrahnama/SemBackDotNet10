#nullable enable
using System.Text.Json;
using Common.exception;
using Microsoft.EntityFrameworkCore;

namespace SemBackDotNet10.service.impl;

public sealed class MeasureUnitServiceImpl(ServiceRuntime runtime) : ServiceBase<MeasureUnit>(runtime)
{
    public Task<ServicePage<MeasureUnitDto>> FindAllAsync(string? filter, ServicePageRequest page, CancellationToken ct = default)
        => R.Queries.MeasureUnitsAsync(filter, page, ct);

    public Task<MeasureUnit> CreateAsync(MeasureUnitDto dto, CancellationToken ct = default)
        => R.TransactionAsync(async () =>
        {
            ArgumentNullException.ThrowIfNull(dto);
            return await SaveAsync(new MeasureUnit { DesMeasureF = dto.DesMeasureF, DesMeasureE = dto.DesMeasureE }, ct);
        }, ct);

    public Task<MeasureUnit> EditAsync(MeasureUnitDto dto, CancellationToken ct = default)
        => R.TransactionAsync(async () =>
        {
            ArgumentNullException.ThrowIfNull(dto);
            var entity = await FindByIdOrElseThrowAsync(dto.Id, ct);
            entity.DesMeasureE = dto.DesMeasureE;
            entity.DesMeasureF = dto.DesMeasureF;
            return await SaveAsync(entity, ct);
        }, ct);

    public async Task<MeasureUnitDto> MapToDtoAsync(MeasureUnit entity, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        entity = await FindByIdOrElseThrowAsync(entity.Id, ct);
        return new MeasureUnitDto { Id = entity.Id, DesMeasureE = entity.DesMeasureE, DesMeasureF = entity.DesMeasureF };
    }

    // The Java method returns null and defines no filtering semantics. Fail explicitly instead.
    public Task<ServicePage<MeasureUnitDto>> ListAsync(MeasureUnitDto dto, ServicePageRequest page, CancellationToken ct = default)
        => throw new NotSupportedException("MeasureUnit.list has no implementation in the Java source; define DTO filtering semantics before enabling it.");

    public async Task<MeasureUnit> FindByIdOrElseThrowAsync(long? id, CancellationToken ct = default)
    {
        if (id is null) throw new BusinessException("error.MeasureUnit.idIsNotValid");
        return await FindByIdAsync(id.Value, ct) ?? throw new BusinessException("error.employee.idIsNotValid", id.Value.ToString(System.Globalization.CultureInfo.InvariantCulture));
    }

    // JSON values are converted to text, matching JSONObject.get(...).toString().
    // Enforce a unique index on Code to prevent concurrent duplicate inserts.
    public Task<MeasureUnit> FindOrInsertProcessMeasureUnitAsync(JsonElement json, CancellationToken ct = default)
        => R.TransactionAsync(async () =>
        {
            var code = json.GetProperty("kpiUnitCode").ToString();
            var entity = await GetRepository().SingleOrDefaultAsync(x => x.Code == code, ct);
            return entity ?? await SaveAsync(new MeasureUnit { Code = code, DesMeasureF = json.GetProperty("kpiUnitName").ToString() }, ct);
        }, ct);
}
