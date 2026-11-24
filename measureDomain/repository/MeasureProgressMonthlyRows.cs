using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.Data;

namespace SemBackDotNet10.measureDomain.repository;

// Read-side mapping only. Consumers must add planning-level and user authorization
// before returning rows; this reader is not an HTTP service.
internal static class MeasureProgressMonthlyRows
{
    internal sealed class Row
    {
        public long Id { get; set; }
        public long? MeasureId { get; set; }
        public long PlanId { get; set; }
        public long LevelId { get; set; }
        public long CatalogId { get; set; }
        public string? MeasureName { get; set; }
        public string? MeasureUnitName { get; set; }
        public int MeasurePeriod { get; set; }
        public int DesiredMethod { get; set; }
        public byte MeasureType { get; set; }
        public byte MeasurePayeshType { get; set; }
        public long? Modir { get; set; }
        public string? ModirName { get; set; }
        public long? Tashilgar { get; set; }
        public long? TashilgarHamkar { get; set; }
        public string? PayeshgarName { get; set; }
        public long? CatalogTashilgar { get; set; }
        public string? ExecuterName { get; set; }
        public string? StrategyObjTitle { get; set; }
        public string? DesLevel { get; set; }
        public string? TaskUrl { get; set; }
        public bool IsRoot { get; set; }
        public long? Month { get; set; }
        public decimal? PlanValue { get; set; }
        public decimal? MonthPlanValue { get; set; }
        public decimal? ActualValue { get; set; }
        public decimal? DailyValue { get; set; }
        public long? Status { get; set; }
        public string? Deviation { get; set; }
        public string? Correction { get; set; }
        public long? CorrectionExecuterId { get; set; }
        public DateTime? CorrectionStartDateAction { get; set; }
        public DateTime? CorrectionDeadlineDate { get; set; }
        public decimal? CorrectionActualValue { get; set; }
        public decimal? MeasurePCN { get; set; }
    }

    internal static async Task<(List<Row> Content, long Total)> ReadPageAsync(IQueryable<Row> query,
        int offset, int size, CancellationToken ct)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(offset);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(size);
        var total = await query.LongCountAsync(ct);
        var content = await query.OrderBy(row => row.MeasureName).ThenBy(row => row.Month)
            .ThenBy(row => row.Id).Skip(offset).Take(size).ToListAsync(ct);
        return (content, total);
    }

    // Java's progress queries require active, non-deleted rows with a plan value.
    // The plan ID comes from the parent measure, not the monthly table.
    internal static IQueryable<Row> Query(SemDbContext db, long planId, long levelId, long userId,
        bool includeDirectSublevels, bool admin, bool sanaye, bool mscHolding,
        long? month, long? status, long? managerId, string? filter)
    {
        var industry = status == 40 && sanaye;
        var anyAssignment = status != 0 && status != 20 && status != 30 && status != 70 && !industry;
        var monitor = anyAssignment || status == 0;
        var manager = anyAssignment || status == 20;
        var facilitator = anyAssignment || status == 30;
        var responsible = anyAssignment || status == 70;
        return db.Database.SqlQuery<Row>($"""
        SELECT CAST(p.dimmeasureactplanid AS bigint) AS Id,
               CAST(p.dimmeasureid AS bigint) AS MeasureId,
               CAST(period.planid AS bigint) AS PlanId, CAST(l.levelid AS bigint) AS LevelId,
               CAST(c.dimmeasurecatalogid AS bigint) AS CatalogId, r.measurename AS MeasureName,
               unit.desmeasuref AS MeasureUnitName, CAST(c.measureperiod AS int) AS MeasurePeriod,
               CAST(r.desiredmethod AS int) AS DesiredMethod, CAST(r.measuretype AS tinyint) AS MeasureType,
               CAST(r.measurepayeshtype AS tinyint) AS MeasurePayeshType,
               CAST(assignment.ManagerId AS bigint) AS Modir, CONCAT(u.nameF, N' ', u.familyF) AS ModirName,
               CAST(assignment.MonitorId AS bigint) AS Tashilgar,
               CAST(CASE WHEN {mscHolding} = 0 THEN c.tashilgarhamkar END AS bigint) AS TashilgarHamkar,
               CONCAT(mon.nameF, N' ', mon.familyF) AS PayeshgarName,
               CAST(c.tashilgar AS bigint) AS CatalogTashilgar,
               CONCAT(execUser.nameF, N' ', execUser.familyF) AS ExecuterName,
               CASE WHEN {mscHolding} = 1 THEN o.title END AS StrategyObjTitle,
               CASE WHEN {mscHolding} = 1 THEN l.deslevel END AS DesLevel, p.taskurl AS TaskUrl,
               CAST(CASE WHEN l.parentid IS NULL AND EXISTS (
                   SELECT 1 FROM dbo.tbldimmeasurecatalogrelation AS rel
                   WHERE rel.mainmeasurecatid = c.dimmeasurecatalogid) THEN 1 ELSE 0 END AS bit) AS IsRoot,
               CAST(p.month AS bigint) AS Month,
               p.planvalue AS PlanValue, p.monthplanvalue AS MonthPlanValue,
               p.actualvalue AS ActualValue, p.dailyvalue AS DailyValue,
               CAST(p.confirmstatus AS bigint) AS Status,
               p.deviation AS Deviation, p.correction AS Correction,
               CAST(p.correctionexecuter AS bigint) AS CorrectionExecuterId,
               p.correctionstartdateaction AS CorrectionStartDateAction,
               p.correctiondeadlinedate AS CorrectionDeadlineDate,
               p.correctionactualvalue AS CorrectionActualValue, p.measurePCN AS MeasurePCN
        FROM dbo.tbldimmeasureactplan AS p
        JOIN dbo.tbldimmeasure AS m ON m.measureid = p.dimmeasureid
        JOIN dbo.tbldimmeasurecatalog AS c ON c.dimmeasurecatalogid = m.dimmeasurecatalogid
        JOIN dbo.tblplanperiod AS period ON period.planid = m.planid
        JOIN dbo.tblplaninglevel AS l ON l.levelid = c.levelid
        JOIN dbo.tblDimMeasureReporsitory AS r ON r.dimmeasurerepositoryid = c.dimmeasurecatalogrepositoryid
        JOIN dbo.tblmeasureunit AS unit ON unit.measureunitid = r.measureunitid
        LEFT JOIN dbo.tblstrategyobject AS o ON o.strategyobjectid = m.strategyobjectid
        CROSS APPLY (SELECT CASE WHEN {mscHolding} = 1 THEN m.modir ELSE c.modir END AS ManagerId,
                            CASE WHEN {mscHolding} = 1 THEN m.tashilgar ELSE c.tashilgar END AS MonitorId) AS assignment
        LEFT JOIN dbo.tblUser AS u ON u.userid = assignment.ManagerId
        LEFT JOIN dbo.tblUser AS mon ON mon.userid = assignment.MonitorId
        LEFT JOIN dbo.tblUser AS execUser ON execUser.userid = c.tashilgar
        WHERE p.active = 1 AND p.isdeleted = 0 AND p.planvalue IS NOT NULL AND m.isplanned = 1
          AND period.planid = {planId}
          AND (l.levelid = {levelId} OR ({includeDirectSublevels} = 1 AND l.parentid = {levelId}))
          AND ({mscHolding} = 0 OR m.confirmstatus = 20 OR l.parentid IS NULL)
          AND ({month} IS NULL OR p.month = {month})
          AND ({status} IS NULL OR p.confirmstatus = {status})
          AND (({admin} = 1 AND ({managerId} IS NULL OR assignment.ManagerId = {managerId}))
            OR ({manager} = 1 AND assignment.ManagerId = {userId})
            OR ({monitor} = 1 AND (assignment.MonitorId = {userId}
                OR ({mscHolding} = 0 AND c.tashilgarhamkar = {userId})))
            OR ({facilitator} = 1 AND l.facilitator = {userId})
            OR ({responsible} = 1 AND l.responsibleid = {userId})
            OR {industry} = 1)
          AND ({filter} IS NULL OR r.measurename LIKE N'%' + {filter} + N'%'
            OR CONCAT(u.nameF, N' ', u.familyF) LIKE N'%' + {filter} + N'%')
        """);
    }
}
