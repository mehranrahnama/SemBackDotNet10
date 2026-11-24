using Common.model.entity;
using Common.repository;
using Microsoft.EntityFrameworkCore;

namespace SemBackDotNet10.Data;

public sealed class ReportExecutionPersistenceSession(SemDbContext db) : IPersistenceSession<ReportExecutedData>
{
    public IQueryable<ReportExecutedData> Query => db.ReportExecutedData;

    public ReportExecutedData Save(ReportExecutedData entity)
    {
        var entry = db.Entry(entity);
        if (entry.State == EntityState.Detached)
        {
            if (entity.Id is null or 0)
                db.ReportExecutedData.Add(entity);
            else
                db.ReportExecutedData.Update(entity);
        }
        db.SaveChanges();
        return entity;
    }

    public void Delete(ReportExecutedData entity)
    {
        db.ReportExecutedData.Remove(entity);
        db.SaveChanges();
    }
}

public sealed class ReportExecutionQueries(SemDbContext db) : IReportExecutionQueries
{
    public ReportExecutedData? FindReportsStatusByPlanIdAndLevelId(
        long? planId, long? levelId, long? month, string storedProcedureName)
    {
        // SQL equality and LIKE do not match a null parameter.
        if (planId is null || storedProcedureName is null) return null;
        var pattern = "%" + storedProcedureName + "%";
        return db.ReportExecutedData.SingleOrDefault(row =>
            row.PlanId == planId &&
            (row.LevelId == null || row.LevelId == levelId) &&
            (row.Month == null || row.Month == month) &&
            row.StoredProcedureName != null && EF.Functions.Like(row.StoredProcedureName, pattern));
    }

    public void ExecuteActionProgressPercentNotHoscoAllMonths(int planId, int levelId) =>
        db.Database.ExecuteSqlInterpolated($"EXEC dbo.Report_ActionProgressPercent_NotHosco_AllMonths_Online @PlanId={planId}, @LevelId={levelId}");
}
