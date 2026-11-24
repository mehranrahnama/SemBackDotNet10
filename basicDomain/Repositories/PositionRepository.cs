#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace SemBackDotNet10.baseDomain.repository;

public interface IPositionRepository : ISemRepository<Position, long>
{
    Task<PageResult<PositionDto>> SearchByDesAsync(PageRequest page, string? param, CancellationToken ct = default);
}

public sealed class PositionRepository(DbContext db)
    : SemRepository<Position, long>(db), IPositionRepository
{
    public Task<PageResult<PositionDto>> SearchByDesAsync(PageRequest page, string? param, CancellationToken ct = default)
    {
        return PageAsync(Set.Where(x => param == null || EF.Functions.Like(x.DesPost, "%" + param + "%")).Select(x => new { x.Id, x.DesPost, x.CodPost }).Distinct().OrderBy(x => x.Id).Select(x => new PositionDto(x.Id, x.DesPost, x.CodPost)), page, ct);
    }
}
