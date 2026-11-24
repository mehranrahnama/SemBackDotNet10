#nullable enable
using Microsoft.EntityFrameworkCore;

namespace SemBackDotNet10.service.impl;

internal sealed partial class ExternalDomainServices
{
    public async Task HardDeleteActionPlansAsync(List<StrategyObject> strategies, CancellationToken ct)
    {
        RequireTransaction();
        foreach (var id in strategies.Select(x => x.Id).Distinct())
            await db.Database.ExecuteSqlInterpolatedAsync($"DELETE FROM [dbo].[tblactionplan] WHERE [strategyobjectid] = {id} AND [isdeleted] = 1", ct);
    }

    public async Task HardDeleteMeasuresAsync(List<StrategyObject> strategies, CancellationToken ct)
    {
        RequireTransaction();
        foreach (var id in strategies.Select(x => x.Id).Distinct())
            await db.Database.ExecuteSqlInterpolatedAsync($"DELETE FROM [dbo].[tbldimmeasure] WHERE [strategyobjectid] = {id} AND [isdeleted] = 1", ct);
    }

    private void RequireTransaction()
    {
        if (db.Database.CurrentTransaction is null)
            throw new InvalidOperationException("Hard deletion requires the planning transaction.");
    }
}
