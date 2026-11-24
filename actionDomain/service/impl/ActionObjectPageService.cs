using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.Common.exception;
using SemBackDotNet10.SecurityDomain.Tenancy;
using SemBackDotNet10.actionDomain.model.dto;
using SemBackDotNet10.basicDomain.Model.Entity;

namespace SemBackDotNet10.service.impl;

public sealed class ActionObjectPageService(ServiceRuntime runtime, IHttpContextAccessor http, TenantContext tenant) : ServiceBase<StrategyObject>(runtime)
{
    public sealed record GridInput(long? LevelId, long? PlanId, long? Status, bool? IsPlanned,
        string? FilterObject, string? FilterAction, string? FilterFacilitator, string? FilterResponsible);

    public async Task<ServicePage<ActionObjectDto>> ReadAsync(GridInput b, ServicePageRequest page, CancellationToken ct)
    {
        if (!tenant.HasTenant || b.LevelId is not long levelId)
            throw new UnAuthorizedException();
        var tenantId = tenant.TenantId;
        if (!await R.Db.Set<PlaningLevel>().AnyAsync(x => x.Id == levelId && x.FactoryId == tenantId, ct))
            throw new UnAuthorizedException();
        await AccessAsync(levelId, ct);
        var identity = http.HttpContext?.User.Identity as ClaimsIdentity;
        var roles = identity?.FindAll(identity.RoleClaimType).Select(c => c.Value).ToArray() ?? [];
        long? facilitator = R.TenantName == "HOSCO" && roles.Length > 0 && roles.All(x => x == "Tashilgar") ? R.User.Id : null;
        // Java applies both person-name filters to payeshgar and accepts rows with no payeshgar.
        var query = R.Db.Database.SqlQuery<ActionObjectDto>($"""
            SELECT CAST(o.strategyobjectid AS bigint) AS ObjectId, o.title AS ObjectTitle,
                   totals.ActionCount, CAST(0 AS int) AS ActionCount2,
                   totals.TotalWeight, CAST(0 AS int) AS TotalWeight2
            FROM dbo.tblstrategyobject o
            JOIN dbo.tblplanperiod pp ON pp.planid = o.planid
            CROSS APPLY (
                SELECT COUNT_BIG(*) AS ActionCount,
                    COALESCE(SUM(CASE WHEN {b.IsPlanned} = 0 OR c.isplanned = 1 OR c.actionplancatalogid IS NULL
                        THEN CAST(a.weight AS float) END), 0) AS TotalWeight
                FROM dbo.tblactionplan a
                LEFT JOIN dbo.tblactionplancatalog c ON c.actionplancatalogid = a.actionplancatalogid
                WHERE a.strategyobjectid = o.strategyobjectid AND a.levelid = {b.LevelId}
                    AND a.planid = {b.PlanId} AND a.confirmstatus <> 50 AND a.isdeleted = 0
            ) totals
            WHERE o.levelid = {b.LevelId} AND pp.planid = {b.PlanId}
                AND EXISTS (
                    SELECT 1 FROM dbo.tblactionplan a
                    LEFT JOIN dbo.tblactionplancatalog c ON c.actionplancatalogid = a.actionplancatalogid
                    LEFT JOIN dbo.tblUser p ON p.userid = a.payeshgar
                    WHERE a.strategyobjectid = o.strategyobjectid AND a.levelid = {b.LevelId}
                        AND a.planid = {b.PlanId} AND a.confirmstatus <> 50 AND a.isdeleted = 0
                        AND ({b.IsPlanned} = 0 OR c.isplanned = 1 OR c.actionplancatalogid IS NULL)
                        AND ({b.Status} IS NULL OR a.confirmstatus = {b.Status})
                        AND ({facilitator} IS NULL OR p.userid = {facilitator})
                        AND ({b.FilterObject} IS NULL OR o.title LIKE '%' + {b.FilterObject} + '%')
                        AND ({b.FilterAction} IS NULL OR c.description LIKE '%' + {b.FilterAction} + '%')
                        AND ({b.FilterFacilitator} IS NULL OR p.userid IS NULL OR CONCAT(p.nameF, ' ', p.familyF) LIKE '%' + {b.FilterFacilitator} + '%')
                        AND ({b.FilterResponsible} IS NULL OR p.userid IS NULL OR CONCAT(p.nameF, ' ', p.familyF) LIKE '%' + {b.FilterResponsible} + '%')
                )
            """);
        return await PageAsync(query, page, ct);
    }
}
