#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SemBackDotNet10.baseDomain.repository;

public interface IMainStrategyRepository : ISemRepository<MainStrategy, long>
{
    Task<PageResult<MainStrategyDto>> FindAllMainStrategiesAsync(PageRequest page, string? filter, CancellationToken ct = default);
    Task<PageResult<MainStrategyDto>> FindAllMainStrategiesByLevelIdAsync(PageRequest page, string? filter, long levelId, CancellationToken ct = default);
    Task<long?> CountActiveStrategyAsync(long planId, CancellationToken ct = default);
    Task<long?> CountActiveStrategyByLevelIdAsync(long levelId, CancellationToken ct = default);
}

public sealed class MainStrategyRepository(DbContext db)
    : SemRepository<MainStrategy, long>(db), IMainStrategyRepository
{
    public Task<PageResult<MainStrategyDto>> FindAllMainStrategiesAsync(PageRequest page, string? filter, CancellationToken ct = default)
    {
        var q = Set.Where(s => filter == null || EF.Functions.Like(s.DesStrategyF, "%" + filter + "%"));
        return PageAsync(q.OrderBy(s => s.Id).Select(s => new MainStrategyDto
        {
            Id = s.Id,

            DesStrategyF = s.DesStrategyF,

            PerspectiveDescription = s.Perspective == null
        ? null
        : s.Perspective.DesperspectF,

            PerspectiveId = s.Perspective == null
        ? (long?)null
        : s.Perspective.Id,

            DateStart = s.DateStart,
            DateEnd = s.DateEnd,

            AccountableName =
        (s.Accountable == null ? "" : s.Accountable.NameF ?? "")
        + " " +
        (s.Accountable == null ? "" : s.Accountable.FamilyF ?? ""),

            AccountableId = s.Accountable == null
        ? (long?)null
        : s.Accountable.UserId
        }), page, ct);
    }

    public Task<PageResult<MainStrategyDto>> FindAllMainStrategiesByLevelIdAsync(PageRequest page, string? filter, long levelId, CancellationToken ct = default)
    {
        var q = Set.Where(s => filter == null || EF.Functions.Like(s.DesStrategyF, "%" + filter + "%"));
        q = q.Where(s => s.PlaningLevel.Id == levelId);
        return PageAsync(q.OrderBy(s => s.Id).Select(s => new MainStrategyDto
        {
            Id = s.Id,

            DesStrategyF = s.DesStrategyF,

            PerspectiveDescription = s.Perspective == null
                ? null
                : s.Perspective.DesperspectF,

            PerspectiveId = s.Perspective == null
                ? (long?)null
                : s.Perspective.Id,

            DateStart = s.DateStart,
            DateEnd = s.DateEnd,

            AccountableName =
                (s.Accountable == null ? "" : s.Accountable.NameF ?? "")
                + " " +
                (s.Accountable == null ? "" : s.Accountable.FamilyF ?? ""),

            AccountableId = s.Accountable == null
                ? (long?)null
                : s.Accountable.UserId
        }), page, ct);
    }

    public async Task<long?> CountActiveStrategyAsync(long planId, CancellationToken ct = default)
    {
        // Materialize before SingleOrDefault: SQL Server EXEC is not composable.
        var values = await Db.Database.SqlQuery<long?>($"EXEC [dbo].[CountActiveStrategy]").ToListAsync(ct);
        return values.SingleOrDefault();
    }

    public async Task<long?> CountActiveStrategyByLevelIdAsync(long levelId, CancellationToken ct = default)
    {
        // Materialize before SingleOrDefault: SQL Server EXEC is not composable.
        var values = await Db.Database.SqlQuery<long?>($"EXEC [dbo].[CountActiveStrategyByLevelId] {levelId}").ToListAsync(ct);
        return values.SingleOrDefault();
    }
}
