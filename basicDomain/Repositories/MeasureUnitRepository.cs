#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SemBackDotNet10.baseDomain.repository;

public interface IMeasureUnitRepository : ISemRepository<MeasureUnit, long>
{
    Task<PageResult<MeasureUnitDto>> FindAllMeasureUnitAsync(PageRequest page, string? filter, CancellationToken ct = default);
    Task<MeasureUnit?> FindByCodeAsync(string code, CancellationToken ct = default);
}

public sealed class MeasureUnitRepository(DbContext db)
    : SemRepository<MeasureUnit, long>(db), IMeasureUnitRepository
{
    public Task<PageResult<MeasureUnitDto>> FindAllMeasureUnitAsync(
        PageRequest page,
        string? filter,
        CancellationToken ct = default)
    {
        var query = Set
            .Where(m =>
                filter == null ||
                EF.Functions.Like(m.DesMeasureE, "%" + filter + "%") ||
                EF.Functions.Like(m.DesMeasureF, "%" + filter + "%"))
            .OrderBy(m => m.Id)
            .Select(m => new MeasureUnitDto
            {
                Id = m.Id,
                DesMeasureE = m.DesMeasureE,
                DesMeasureF = m.DesMeasureF
            });

        return PageAsync(query, page, ct);
    }



    public Task<MeasureUnit?> FindByCodeAsync(string code, CancellationToken ct = default)
    {
        return Set.SingleOrDefaultAsync(x => x.Code == code, ct);
    }
}
