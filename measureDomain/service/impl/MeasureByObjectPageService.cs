using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.Common.exception;
using SemBackDotNet10.basicDomain.Model.Entity;
using SemBackDotNet10.measureDomain.model.dto;

namespace SemBackDotNet10.service.impl;

public sealed record MeasureByObjectInput(long? LevelId, long? PlanId, long? ObjectId, long? Status, string? Filter);

public sealed class MeasureByObjectPageService(ServiceRuntime runtime, IHttpContextAccessor http) : ServiceBase<StrategyObject>(runtime)
{
    // Java measure-by-object grid projection.
    public async Task<ServicePage<MeasureDto>> FindAsync(MeasureByObjectInput body, ServicePageRequest page, CancellationToken ct)
    {
        if (body.LevelId is not long levelId) throw new ArgumentException("LevelId is required.", nameof(body));
        await AccessAsync(levelId, ct);
        var currentUserId = R.User.Id;
        var identity = http.HttpContext?.User.Identity as ClaimsIdentity;
        var roles = identity?.FindAll(identity.RoleClaimType).Select(claim => claim.Value).ToArray() ?? [];
        long? facilitatorId = R.TenantName == "HOSCO" && roles.Length > 0 && roles.All(role => role == "Tashilgar") ? currentUserId : null;
        var query = R.Db.Database.SqlQuery<MeasureByObjectRow>($"""
            SELECT CAST(m.measureid AS bigint) AS Id, o.title AS StrategyObjectTitle,
                CAST(o.strategyobjectid AS bigint) AS StrategyObjectId, r.measurename AS MeasureName,
                CONCAT(t.nameF, ' ', t.familyF) AS Tashilgar, CONCAT(manager.nameF, ' ', manager.familyF) AS Modir,
                CAST(c.measureperiod AS int) AS MeasurePeriod, unit.desmeasuref AS DesMeasureF,
                CAST(m.weight AS real) AS Weight, CAST(c.dimmeasurecatalogid AS bigint) AS MeasureCatalogId,
                m.currentvalue AS CurrentValue, m.currentyeartarget AS CurrentYearTarget,
                m.oneyearlatertarget AS OneYearLaterTarget, m.twoyearlatertarget AS TwoYearLaterTarget,
                c.datatype AS DataType, CAST(r.desiredmethod AS int) AS DesiredMethod,
                r.formula AS Formula, r.formulades AS FormulaDes, m.targettheory AS TargetTheory,
                m.zerotarget AS ZeroTarget, m.flgkpi AS FlgKpi, m.zeroinitial AS ZeroInitial,
                m.perfectextreme AS PerfectExtreme, m.warnextreme AS WarnExtreme,
                CAST(m.confirmstatus AS bigint) AS MeasureConfirmStatus, CAST(m.isdeleted AS bit) AS Deleted,
                c.code AS MeasureCode, pl.desthreechar AS CommitteeCode,
                CAST(r.measurepayeshtype AS tinyint) AS MeasurePayeshType,
                CAST(m.modir AS bigint) AS ModirId, CAST(m.tashilgar AS bigint) AS TashilgarId,
                CAST(m.benchmarkvalue AS decimal(38, 10)) AS BenchmarkValue,
                CAST(m.addtosupremelevel AS bit) AS SendToUpperLevels,
                CAST(m.sendtoparentonly AS bit) AS SendToParentOnly,
                CAST(m.isreplanned AS bit) AS IsReplanned,
                CASE
                    WHEN EXISTS (SELECT 1 FROM dbo.tbldimmeasureactplan ap
                        WHERE ap.dimmeasureid = m.measureid AND ap.isdeleted = 0)
                     AND NOT EXISTS (SELECT 1 FROM dbo.tbldimmeasureactplan ap
                        WHERE ap.dimmeasureid = m.measureid AND ap.isdeleted = 0 AND ap.planvalue IS NOT NULL)
                        THEN 3
                    -- Java's fallback uses the full collection, including deleted rows.
                    WHEN EXISTS (SELECT 1 FROM dbo.tbldimmeasureactplan ap WHERE ap.dimmeasureid = m.measureid)
                        THEN CASE WHEN m.isplanned = 1 THEN 2 ELSE 1 END
                    ELSE 0
                END AS IsPlanned,
                CAST(CASE WHEN EXISTS (SELECT 1 FROM dbo.tbldashboard d WHERE d.dimmeasureid = m.measureid
                    AND d.userid = {currentUserId} AND d.isdeleted = 0) THEN 1 ELSE 0 END AS bit) AS AddedToDashboard
            FROM dbo.tbldimmeasure m
            LEFT JOIN dbo.tblstrategyobject o ON o.strategyobjectid = m.strategyobjectid
            JOIN dbo.tbldimmeasurecatalog c ON c.dimmeasurecatalogid = m.dimmeasurecatalogid
            JOIN dbo.tblDimMeasureReporsitory r ON r.dimmeasurerepositoryid = c.dimmeasurecatalogrepositoryid
            JOIN dbo.tblmeasureunit unit ON unit.measureunitid = r.measureunitid
            LEFT JOIN dbo.tblUser t ON t.userid = c.tashilgar
            LEFT JOIN dbo.tblUser manager ON manager.userid = c.modir
            JOIN dbo.tblplaninglevel pl ON pl.levelid = c.levelid
            JOIN dbo.tblplanperiod pp ON pp.planid = m.planid
            WHERE pl.levelid = {body.LevelId} AND pp.planid = {body.PlanId}
              AND o.strategyobjectid = {body.ObjectId} AND m.confirmstatus <> 50
              AND ({body.Status} IS NULL OR m.confirmstatus = {body.Status})
              AND ({body.Filter} IS NULL
                OR CAST(m.weight AS nvarchar(100)) LIKE N'%' + {body.Filter} + N'%'
                OR CONCAT(t.nameF, t.familyF, manager.nameF, manager.familyF, r.measurename, unit.desmeasuref) LIKE N'%' + {body.Filter} + N'%'
                OR (SELECT d.desf FROM dbo.tbldes d WHERE d.desgrpid = 23
                    AND d.val = CAST(c.measureperiod AS nvarchar(50))) LIKE N'%' + {body.Filter} + N'%')
              AND EXISTS (SELECT 1 FROM dbo.tbldimmeasure assigned
                  LEFT JOIN dbo.tbldimmeasurecatalog assignedCatalog ON assignedCatalog.dimmeasurecatalogid = assigned.dimmeasurecatalogid
                  WHERE assigned.strategyobjectid = o.strategyobjectid AND
                    ({facilitatorId} IS NULL OR assigned.tashilgar = {facilitatorId} OR assignedCatalog.tashilgarhamkar = {facilitatorId}))
            """);
        var rows = await PageAsync(query, page, ct);
        var content = rows.Content.Select(row => new MeasureDto
        {
            Id = row.Id, StrategyObjectTitle = row.StrategyObjectTitle, StrategyObjectId = row.StrategyObjectId,
            MeasureName = row.MeasureName, Tashilgar = row.Tashilgar, Modir = row.Modir,
            MeasurePeriod = row.MeasurePeriod, DesMeasureF = row.DesMeasureF, Weight = row.Weight,
            MeasureCatalogId = row.MeasureCatalogId, CurrentValue = row.CurrentValue,
            CurrentYearTarget = row.CurrentYearTarget, OneYearLaterTarget = row.OneYearLaterTarget,
            TwoYearLaterTarget = row.TwoYearLaterTarget, DataType = row.DataType,
            DesiredMethod = row.DesiredMethod, Formula = row.Formula, FormulaDes = row.FormulaDes,
            TargetTheory = row.TargetTheory, ZeroTarget = row.ZeroTarget, FlgKpi = row.FlgKpi,
            ZeroInitial = row.ZeroInitial, PerfectExtreme = row.PerfectExtreme, WarnExtreme = row.WarnExtreme,
            MeasureConfirmStatus = row.MeasureConfirmStatus, Deleted = row.Deleted,
            MeasureCode = row.MeasureCode is not null && row.CommitteeCode is not null ? $"{row.MeasureCode}-{row.CommitteeCode}" : row.MeasureCode,
            CommitteeCode = row.CommitteeCode, MeasurePayeshType = row.MeasurePayeshType,
            ModirId = row.ModirId, TashilgarId = row.TashilgarId, BenchmarkValue = row.BenchmarkValue,
            SendToUpperLevels = row.SendToUpperLevels ?? false, SendToParentOnly = row.SendToParentOnly,
            AddedToDashboard = row.AddedToDashboard, IsPlanned = row.IsPlanned, IsReplanned = row.IsReplanned
        }).ToArray();
        if (body.PlanId is long planId)
            await PopulateUpperLevelAsync(content, planId, ct);
        if (R.TenantName == "HOSCO" && content.Length > 0)
            await PopulateOrganizationsAsync(content, ct);
        return new ServicePage<MeasureDto>(content, rows.TotalElements, rows.CurrentPage, rows.PageSize);
    }

    private async Task PopulateOrganizationsAsync(MeasureDto[] content, CancellationToken ct)
    {
        var ids = content.Select(m => m.Id.GetValueOrDefault()).ToArray();
        var links = await R.Db.Database.SqlQuery<OrganizationLink>($"""
            SELECT CAST(c.measureid AS bigint) AS MeasureId, CAST(c.measurecoworkerid AS bigint) AS Id,
                   CAST(p.planlevelorgid AS bigint) AS OrganizationLinkId, o.desorganF AS DesOrganF, CAST(0 AS bit) AS IsBeneficiary
            FROM dbo.tbldimmeasurecoworker c
            JOIN dbo.tblplaninglevelorg p ON p.planlevelorgid = c.planinglevelorgid
            JOIN dbo.tblorganization o ON o.organizationid = p.organizationid
            UNION ALL
            SELECT CAST(b.measureid AS bigint), CAST(b.measurebeneficiaryid AS bigint),
                   CAST(p.planlevelorgid AS bigint), o.desorganF, CAST(1 AS bit)
            FROM dbo.tbldimmeasurebeneficiary b
            JOIN dbo.tblplaninglevelorg p ON p.planlevelorgid = b.planinglevelorgid
            JOIN dbo.tblorganization o ON o.organizationid = p.organizationid
            """).Where(link => ids.Contains(link.MeasureId)).ToListAsync(ct);
        var byMeasure = links.ToLookup(link => link.MeasureId);
        foreach (var measure in content)
        {
            var assigned = byMeasure[measure.Id.GetValueOrDefault()];
            measure.MeasureCoworkerDtoList = assigned.Where(x => !x.IsBeneficiary).Select(x => new MeasureCoworkerDto
            {
                MeasureCoworkerId = x.Id, PlaningLevelOrganizationId = x.OrganizationLinkId, DesOrganF = x.DesOrganF
            }).ToList();
            measure.MeasureBeneficiaryDtoList = assigned.Where(x => x.IsBeneficiary).Select(x => new MeasureBeneficiaryDto
            {
                MeasureBeneficiaryId = x.Id, PlaningLevelOrganizationId = x.OrganizationLinkId, DesOrganF = x.DesOrganF
            }).ToList();
        }
    }

    private sealed class OrganizationLink
    {
        public long MeasureId { get; set; }
        public long Id { get; set; }
        public long OrganizationLinkId { get; set; }
        public string? DesOrganF { get; set; }
        public bool IsBeneficiary { get; set; }
    }

    private async Task PopulateUpperLevelAsync(MeasureDto[] content, long planId, CancellationToken ct)
    {
        var parents = new Dictionary<long, long?>();
        var objectives = new Dictionary<long, UpperLevelStrategyObjectDto?>();
        foreach (var measure in content)
        {
            if (!measure.SendToUpperLevels || measure.MeasureCatalogId is not long catalogId)
                continue;

            var ancestors = new List<long>();
            var seen = new HashSet<long>();
            var current = catalogId;
            while (true)
            {
                if (!parents.TryGetValue(current, out var parent))
                {
                    var matches = await R.Db.Database.SqlQuery<long?>($"""
                        SELECT CAST(r.mainmeasurecatid AS bigint) AS Value
                        FROM dbo.tbldimmeasurecatalogrelation r
                        WHERE r.relatedmeasurecatid = {current} AND r.planid = {planId}
                        """).Take(2).ToListAsync(ct);
                    if (matches.Count > 1)
                        throw new BusinessException("شاخص های در روابط محاسباتی نمیتوانند بیش از یک والد داشته باشند.");
                    parent = matches.SingleOrDefault();
                    parents.Add(current, parent);
                }
                if (parent is not long id) break;
                if (!seen.Add(id))
                    throw new BusinessException("ساختار شاخص‌ها دارای حلقه است و معتبر نمی‌باشد.");
                ancestors.Add(id);
                current = id;
            }

            // Validate the whole ancestry before choosing the nearest eligible objective.
            foreach (var ancestor in ancestors)
            {
                if (!objectives.TryGetValue(ancestor, out var objective))
                {
                    var row = await R.Db.Database.SqlQuery<UpperLevelRow>($"""
                        SELECT CAST(pl.typelevel AS smallint) AS TypeLevel,
                               CAST(o.strategyobjectid AS bigint) AS Id, o.title AS Title
                        FROM dbo.tbldimmeasure m
                        JOIN dbo.tbldimmeasurecatalog c ON c.dimmeasurecatalogid = m.dimmeasurecatalogid
                        JOIN dbo.tblplaninglevel pl ON pl.levelid = c.levelid
                        LEFT JOIN dbo.tblstrategyobject o ON o.strategyobjectid = m.strategyobjectid
                        WHERE m.dimmeasurecatalogid = {ancestor} AND m.planid = {planId} AND m.isdeleted = 0
                        """).SingleOrDefaultAsync(ct);
                    objective = row is { TypeLevel: 1 } ? new UpperLevelStrategyObjectDto { Id = row.Id, Title = row.Title } : null;
                    objectives.Add(ancestor, objective);
                }
                if (objective is null) continue;
                measure.TopLevelStrategyObject = objective;
                break;
            }
        }
    }

    private sealed class UpperLevelRow
    {
        public short TypeLevel { get; set; }
        public long? Id { get; set; }
        public string? Title { get; set; }
    }
}

public sealed class MeasureByObjectRow
{
    public long Id { get; set; }
    public string? StrategyObjectTitle { get; set; }
    public long? StrategyObjectId { get; set; }
    public string? MeasureName { get; set; }
    public string? Tashilgar { get; set; }
    public string? Modir { get; set; }
    public int MeasurePeriod { get; set; }
    public string? DesMeasureF { get; set; }
    public float? Weight { get; set; }
    public long MeasureCatalogId { get; set; }
    public decimal? CurrentValue { get; set; }
    public decimal? CurrentYearTarget { get; set; }
    public decimal? OneYearLaterTarget { get; set; }
    public decimal? TwoYearLaterTarget { get; set; }
    public byte? DataType { get; set; }
    public int DesiredMethod { get; set; }
    public string? Formula { get; set; }
    public string? FormulaDes { get; set; }
    public string? TargetTheory { get; set; }
    public bool ZeroTarget { get; set; }
    public bool FlgKpi { get; set; }
    public bool ZeroInitial { get; set; }
    public decimal? PerfectExtreme { get; set; }
    public decimal? WarnExtreme { get; set; }
    public long? MeasureConfirmStatus { get; set; }
    public bool Deleted { get; set; }
    public string? MeasureCode { get; set; }
    public string? CommitteeCode { get; set; }
    public byte MeasurePayeshType { get; set; }
    public long? ModirId { get; set; }
    public long? TashilgarId { get; set; }
    public decimal? BenchmarkValue { get; set; }
    public bool? SendToUpperLevels { get; set; }
    public bool SendToParentOnly { get; set; }
    public bool AddedToDashboard { get; set; }
    public int IsPlanned { get; set; }
    public bool IsReplanned { get; set; }
}
