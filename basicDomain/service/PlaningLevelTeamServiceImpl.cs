#nullable enable
using Microsoft.EntityFrameworkCore;
namespace SemBackDotNet10.service.impl;

public sealed class PlaningLevelTeamServiceImpl(ServiceRuntime runtime) : ServiceBase<PlaningLevelTeam>(runtime)
{
    public Task<ServicePage<PlaningLevelTeamDto>> FindAllAsync(string filter, ServicePageRequest page, CancellationToken ct = default)
        => R.Queries.LevelTeamsAsync(ServiceJson.Filter(filter), null, false, page, ct);
    public async Task<ServicePage<PlaningLevelTeamDto>> FindAllByLevelIdAsync(string body, ServicePageRequest page, CancellationToken ct = default)
    {
        var id = ServiceJson.NullableLong(body, "levelId"); await AccessAsync(id, ct);
        return await R.Queries.LevelTeamsAsync(ServiceJson.Filter(body), id, true, page, ct);
    }
    public PlaningLevelTeamDto MapToDto(PlaningLevelTeam entity) => new()
    { Id = entity.Id, LevelId = entity.Level?.Id, UserId = entity.User?.UserId };
    public async Task<PlaningLevelTeam> MapToEntityAsync(PlaningLevelTeamDto dto, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(dto);
        var entity = new PlaningLevelTeam { Id = dto.Id };
        if (dto.LevelId is not null) entity.Level = await RequireAsync<PlaningLevel>(dto.LevelId, "error.PlaningLevel.idIsNotValid", ct);
        if (dto.UserId is not null) entity.User = await RequireAsync<User>(dto.UserId, "error.user.idIsNotValid", ct);
        return entity;
    }
    public Task<PlaningLevelTeam> FindByIdOrElseThrowAsync(long? id, CancellationToken ct = default)
        => RequireAsync<PlaningLevelTeam>(id, "error.planingLevelTeam.idIsNotValid", ct);
    public Task<PlaningLevelTeam> CreateAsync(PlaningLevelTeamDto dto, CancellationToken ct = default)
        => R.TransactionAsync(async () =>
        {
            ArgumentNullException.ThrowIfNull(dto);
            await AccessAsync(dto.LevelId, ct);
            if (await GetRepository().AnyAsync(x => x.User.UserId == dto.UserId && x.Level.Id == dto.LevelId, ct))
                throw new BusinessException("error.planingLevelTeam.isDuplicate");
            return await SaveAsync(await MapToEntityAsync(dto, ct), ct);
        }, ct);
    public Task<PlaningLevelTeam> EditAsync(PlaningLevelTeamDto dto, CancellationToken ct = default)
        => R.TransactionAsync(async () =>
        {
            ArgumentNullException.ThrowIfNull(dto);
            var entity = await FindByIdOrElseThrowAsync(dto.Id, ct);
            if (dto.LevelId is not null) entity.Level = await RequireAsync<PlaningLevel>(dto.LevelId, "error.PlaningLevel.idIsNotValid", ct);
            if (dto.UserId is not null) entity.User = await RequireAsync<User>(dto.UserId, "error.user.idIsNotValid", ct);
            return await SaveAsync(entity, ct);
        }, ct);
    public Task DeleteAsync(long? id, CancellationToken ct = default)
        => R.TransactionAsync(async () => await base.DeleteAsync(await FindByIdOrElseThrowAsync(id, ct), ct), ct);
}
