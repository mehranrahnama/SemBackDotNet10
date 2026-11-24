#nullable enable
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
namespace SemBackDotNet10.service.impl;

public sealed class MainStrategyServiceImpl(ServiceRuntime runtime) : ServiceBase<MainStrategy>(runtime)
{
    public Task<MainStrategy> MainStrategyCreateAsync(MainStrategyDto dto, CancellationToken ct = default)
        => R.TransactionAsync(async () =>
        {
            ArgumentNullException.ThrowIfNull(dto);
            await AccessAsync(dto.LevelId, ct);
            var entity = new MainStrategy { DesStrategyF = dto.DesStrategyF, DateStart = dto.DateStart, DateEnd = dto.DateEnd };
            if (dto.PerspectiveId is not null) entity.Perspective = await R.Db.Set<Perspective>().FindAsync([dto.PerspectiveId], ct);
            if (dto.LevelId is not null) entity.PlaningLevel = await RequireAsync<PlaningLevel>(dto.LevelId, "error.PlaningLevel.idIsNotValid", ct);
            if (dto.AccountableId is not null) entity.Accountable = await R.External.GetUserAsync(dto.AccountableId.Value, ct);
            return await SaveAsync(entity, ct);
        }, ct);
    public Task<MainStrategy> MainStrategyEditAsync(MainStrategyDto dto, CancellationToken ct = default)
        => R.TransactionAsync(async () =>
        {
            ArgumentNullException.ThrowIfNull(dto);
            var entity = await FindByIdOrElseThrowAsync(dto.Id, ct);
            entity.DesStrategyF = dto.DesStrategyF; entity.DateStart = dto.DateStart; entity.DateEnd = dto.DateEnd;
            if (dto.AccountableId is not null) entity.Accountable = await R.External.GetUserAsync(dto.AccountableId.Value, ct);
            return await SaveAsync(entity, ct);
        }, ct);
    public async Task<MainStrategyDto> MapToDtoAsync(MainStrategy entity, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        entity = await GetRepository().Include(x => x.Perspective).Include(x => x.PlaningLevel).Include(x => x.Accountable).SingleAsync(x => x.Id == entity.Id, ct);
        var dto = new MainStrategyDto { DesStrategyF = entity.DesStrategyF, DateStart = entity.DateStart, DateEnd = entity.DateEnd };
        if (entity.Perspective is not null) { dto.PerspectiveId = entity.Perspective.Id; dto.PerspectiveDescription = entity.Perspective.DesperspectF; }
        if (R.TenantName == "MSCHOLDING") dto.LevelId = entity.PlaningLevel.Id;
        if (entity.Accountable is not null) { dto.AccountableId = entity.Accountable.UserId; dto.AccountableName = entity.Accountable.NameF + " " + entity.Accountable.FamilyF; }
        return dto; // Java omits Id in this mapper; intentionally retained.
    }
    public Task<ServicePage<MainStrategyDto>> FindAllAsync(string? filter, ServicePageRequest page, CancellationToken ct = default)
        => R.Queries.MainStrategiesAsync(filter, null, false, page, ct);
    public async Task<ServicePage<MainStrategyDto>> FindAllByLevelIdAsync(string? filter, ServicePageRequest page, string body, CancellationToken ct = default)
    {
        var id = ServiceJson.NullableLong(body, "levelId"); await AccessAsync(id, ct);
        return await R.Queries.MainStrategiesAsync(filter, id, true, page, ct);
    }
    public Task<ServicePage<MainStrategyDto>> FindAllByLevelIdForRoadMapAsync(string? filter, ServicePageRequest page, string body, CancellationToken ct = default)
        => R.Queries.MainStrategiesAsync(filter, ServiceJson.NullableLong(body, "levelId"), true, page, ct);
    public Task<MainStrategy> FindByIdOrElseThrowAsync(long? id, CancellationToken ct = default)
        => RequireAsync<MainStrategy>(id, "error.MainStrategy.idIsNotValid", ct);
    public Task<long> CountActiveStrategyAsync(long? planId, CancellationToken ct = default) => R.Queries.CountActiveStrategyAsync(planId, null, ct);
    public async Task<long> CountActiveStrategyByLevelIdAsync(long? levelId, CancellationToken ct = default)
    { await AccessAsync(levelId, ct); return await R.Queries.CountActiveStrategyAsync(null, levelId, ct); }
}

internal static class ServiceJson
{
    internal static long? NullableLong(string json, string key)
    { using var doc = JsonDocument.Parse(json); return Long(doc.RootElement, key); }
    internal static long? Long(JsonElement root, string key) => root.TryGetProperty(key, out var v) && v.ValueKind != JsonValueKind.Null ? v.GetInt64() : null;
    internal static string? Text(JsonElement root, string key) => root.TryGetProperty(key, out var v) && v.ValueKind != JsonValueKind.Null ? v.GetString() : null;
    internal static string Filter(string json)
    { using var doc = JsonDocument.Parse(json); return doc.RootElement.GetProperty("filter").GetString() ?? throw new JsonException("filter must be a string"); }
    internal static T EnumName<T>(string name, T _) where T : struct, Enum
    { if (!Enum.GetNames<T>().Contains(name, StringComparer.Ordinal)) throw new ArgumentException("Unknown enum name: " + name); return Enum.Parse<T>(name); }
}
