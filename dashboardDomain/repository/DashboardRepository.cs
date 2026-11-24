using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.Data;
using SemBackDotNet10.dashboardDomain.model.entity;

namespace SemBackDotNet10.dashboardDomain.repository;

public interface IDashboardRepository
{
    Task<bool> MeasureExistsAsync(long id);

    Task<bool> ActionPlanExistsAsync(long id);

    /// <summary>exec [dbo].[proc] @p0, @p1, ... و برگرداندن ردیف‌ها (معادل List&lt;Tuple&gt;).</summary>
    Task<List<DbRow>> ExecAsync(string procedure, params object?[] args);

    /// <summary>اجرای پروسیجر بدون نتیجه (معادل @Procedure void).</summary>
    Task ExecNonQueryAsync(string procedure, params object?[] args);

    Task<Dashboard?> FindByUserAndMeasureAsync(long userId, long measureId);

    Task<Dashboard?> FindByUserAndActionPlanAsync(long userId, long actionPlanId);

    Task<List<Dashboard>> FindAllByDeletedIsFalseAndMeasureInAsync(IEnumerable<long> measureIds);

    Task<List<Dashboard>> FindAllByDeletedIsFalseAndMeasureIdAsync(long measureId);

    Task SaveAllAsync(IEnumerable<Dashboard> items);

    Task DeleteAsync(Dashboard dashboard);

    /// <summary>soft delete: داشبوردهای مربوط به شاخص‌های یک کاتالوگ.</summary>
    Task DeleteByMeasureCatalogIdAsync(long catalogId);

    Task DeleteByActionPlanIdAsync(long actionPlanId);

    Task DeleteByMeasureIdAsync(long measureId);
}

public class DashboardRepository : IDashboardRepository
{
    private readonly SemDbContext _context;

    public Task<bool> MeasureExistsAsync(long id) => _context.Database.SqlQuery<int>($"""
        SELECT 1 AS Value FROM dbo.tblDimMeasure WHERE MeasureId = {id}
        """).AnyAsync();

    public Task<bool> ActionPlanExistsAsync(long id) => _context.Database.SqlQuery<int>($"""
        SELECT 1 AS Value FROM dbo.tblActionPlan WHERE ActionPlanId = {id}
        """).AnyAsync();

    public DashboardRepository(SemDbContext context) => _context = context;

    public async Task<List<DbRow>> ExecAsync(string procedure, params object?[] args)
    {
        var connection = _context.Database.GetDbConnection();
        var wasClosed = connection.State != System.Data.ConnectionState.Open;
        if (wasClosed) await connection.OpenAsync();
        try
        {
            await using var command = CreateCommand(connection, procedure, args);
            await using var reader = await command.ExecuteReaderAsync();
            return await DbRowMapper.ReadRowsAsync(reader, CancellationToken.None);
        }
        finally
        {
            if (wasClosed) await connection.CloseAsync();
        }
    }

    public async Task ExecNonQueryAsync(string procedure, params object?[] args)
    {
        var connection = _context.Database.GetDbConnection();
        var wasClosed = connection.State != System.Data.ConnectionState.Open;
        if (wasClosed) await connection.OpenAsync();
        try
        {
            await using var command = CreateCommand(connection, procedure, args);
            await command.ExecuteNonQueryAsync();
        }
        finally
        {
            if (wasClosed) await connection.CloseAsync();
        }
    }

    private static DbCommand CreateCommand(DbConnection connection, string procedure, object?[] args)
    {
        var command = connection.CreateCommand();
        command.CommandTimeout = 0;
        var names = new List<string>();
        for (var i = 0; i < args.Length; i++)
        {
            var p = command.CreateParameter();
            p.ParameterName = $"@p{i}";
            p.Value = args[i] ?? DBNull.Value;
            command.Parameters.Add(p);
            names.Add(p.ParameterName);
        }
        command.CommandText = $"exec [dbo].[{procedure}] {string.Join(", ", names)}".TrimEnd();
        return command;
    }

    public Task<Dashboard?> FindByUserAndMeasureAsync(long userId, long measureId) =>
        _context.Set<Dashboard>().FirstOrDefaultAsync(d => d.UserId == userId && d.MeasureId == measureId);

    public Task<Dashboard?> FindByUserAndActionPlanAsync(long userId, long actionPlanId) =>
        _context.Set<Dashboard>().FirstOrDefaultAsync(d => d.UserId == userId && d.ActionPlanId == actionPlanId);

    public Task<List<Dashboard>> FindAllByDeletedIsFalseAndMeasureInAsync(IEnumerable<long> measureIds)
    {
        var ids = measureIds.ToList();
        return _context.Set<Dashboard>()
            .Where(d => !d.Deleted && d.MeasureId != null && ids.Contains(d.MeasureId.Value))
            .ToListAsync();
    }

    public Task<List<Dashboard>> FindAllByDeletedIsFalseAndMeasureIdAsync(long measureId) =>
        _context.Set<Dashboard>().Where(d => !d.Deleted && d.MeasureId == measureId).ToListAsync();

    public async Task SaveAllAsync(IEnumerable<Dashboard> items)
    {
        _context.Set<Dashboard>().AddRange(items.Where(i => i.Id == 0));
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Dashboard dashboard)
    {
        _context.Set<Dashboard>().Remove(dashboard);
        await _context.SaveChangesAsync();
    }

    public Task DeleteByMeasureCatalogIdAsync(long catalogId) =>
        _context.Database.ExecuteSqlInterpolatedAsync(
            $@"UPDATE d SET d.isdeleted = 1
               FROM dbo.tbldashboard d
               INNER JOIN dbo.tbldimmeasure m ON m.measureid = d.dimmeasureid
               WHERE d.isdeleted = 0 AND m.dimmeasurecatalogid = {catalogId}");

    public Task DeleteByActionPlanIdAsync(long actionPlanId) =>
        _context.Set<Dashboard>().Where(d => d.ActionPlanId == actionPlanId).ExecuteDeleteAsync();

    public Task DeleteByMeasureIdAsync(long measureId) =>
        _context.Set<Dashboard>().Where(d => d.MeasureId == measureId).ExecuteDeleteAsync();
}
