#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace SemBackDotNet10.baseDomain.repository;

public interface IPlaningLevelUnitRepository : ISemRepository<PlaningLevelUnit, long>
{
    Task<PlaningLevelUnit?> FindByUnitCodeAsync(string unitCode, CancellationToken ct = default);
}

public sealed class PlaningLevelUnitRepository(DbContext db)
    : SemRepository<PlaningLevelUnit, long>(db), IPlaningLevelUnitRepository
{
    public Task<PlaningLevelUnit?> FindByUnitCodeAsync(string unitCode, CancellationToken ct = default)
    {
        return Set.SingleOrDefaultAsync(x => x.UnitCode == unitCode, ct);
    }
}
