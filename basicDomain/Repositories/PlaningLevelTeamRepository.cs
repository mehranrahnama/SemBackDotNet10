#nullable enable

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.basicDomain.Model.Dto;
using SemBackDotNet10.basicDomain.Model.Entity;

namespace SemBackDotNet10.baseDomain.repository;

public interface IPlaningLevelTeamRepository
    : ISemRepository<PlaningLevelTeam, long>
{
    Task<PageResult<PlaningLevelTeamDto>> FindAllPlaningLevelTeamsAsync(
        PageRequest page,
        string? filter,
        CancellationToken ct = default);

    Task<PageResult<PlaningLevelTeamDto>> FindByLevelIdAsync(
        PageRequest page,
        string? filter,
        long? levelId,
        CancellationToken ct = default);

    Task<PlaningLevelTeam?> FindByUserIdAndLevelIdAsync(
        long userId,
        long levelId,
        CancellationToken ct = default);
}

public sealed class PlaningLevelTeamRepository(DbContext db)
    : SemRepository<PlaningLevelTeam, long>(db),
      IPlaningLevelTeamRepository
{
    public Task<PageResult<PlaningLevelTeamDto>>
        FindAllPlaningLevelTeamsAsync(
            PageRequest page,
            string? filter,
            CancellationToken ct = default)
    {
        var query = Set.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter))
        {
            var pattern = $"%{filter}%";

            query = query.Where(x =>
                (x.Level != null &&
                    (
                        EF.Functions.Like(x.Level.Code, pattern) ||
                        EF.Functions.Like(x.Level.DesLevel, pattern) ||
                        EF.Functions.Like(
                            x.Level.TypeLevel.ToString(),
                            pattern) ||
                        EF.Functions.Like(
                            x.Level.CreateDate.ToString(),
                            pattern)
                    )) ||
                (x.User != null &&
                    (
                        EF.Functions.Like(x.User.NameF, pattern) ||
                        EF.Functions.Like(x.User.FamilyF, pattern)
                    )));
        }

        var result = query
            .OrderBy(x => x.Id)
            .Select(x => new PlaningLevelTeamDto
            {
                Id = x.Id,
                LevelId = x.LevelId,
                UserId = x.UserId
            });

        return PageAsync(result, page, ct);
    }

    public Task<PageResult<PlaningLevelTeamDto>>
        FindByLevelIdAsync(
            PageRequest page,
            string? filter,
            long? levelId,
            CancellationToken ct = default)
    {
        var query = Set.AsQueryable();

        if (levelId.HasValue)
        {
            query = query.Where(x => x.LevelId == levelId.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter))
        {
            var pattern = $"%{filter}%";

            query = query.Where(x =>
                EF.Functions.Like(x.Id.ToString(), pattern) ||
                (x.LevelId.HasValue &&
                    EF.Functions.Like(
                        x.LevelId.Value.ToString(),
                        pattern)) ||
                (x.UserId.HasValue &&
                    EF.Functions.Like(
                        x.UserId.Value.ToString(),
                        pattern)) ||
                (x.User != null &&
                    (
                        EF.Functions.Like(x.User.NameF, pattern) ||
                        EF.Functions.Like(x.User.FamilyF, pattern)
                    )));
        }

        var result = query
            .OrderBy(x => x.Id)
            .Select(x => new PlaningLevelTeamDto
            {
                Id = x.Id,
                LevelId = x.LevelId,
                UserId = x.UserId
            });

        return PageAsync(result, page, ct);
    }

    public Task<PlaningLevelTeam?> FindByUserIdAndLevelIdAsync(
        long userId,
        long levelId,
        CancellationToken ct = default)
    {
        return Set.SingleOrDefaultAsync(
            x => x.UserId == userId &&
                 x.LevelId == levelId,
            ct);
    }
}
