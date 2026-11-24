using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.basicDomain.Model.Entity;
using SemBackDotNet10.measureDomain.controller.dto.measureDtos;
using SemBackDotNet10.measureDomain.model.dto;

namespace SemBackDotNet10.service.impl;

public sealed class MeasureObjectGridService(ServiceRuntime runtime, IHttpContextAccessor http) : ServiceBase<StrategyObject>(runtime)
{
    public async Task<ServicePage<MeasureObjectDto>> FindAsync(LoadStrategyObjectDto body, ServicePageRequest page, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(body);
        if (body.LevelId is not long levelId || body.PlanId is not long planId || body.IsPlanned is not bool isPlanned)
            throw new ArgumentException("LevelId, PlanId and IsPlanned are required.", nameof(body));
        await AccessAsync(levelId, ct);
        var identity = http.HttpContext?.User.Identity as ClaimsIdentity;
        var roles = identity?.FindAll(identity.RoleClaimType).Select(claim => claim.Value).ToArray() ?? [];
        long? facilitatorId = R.TenantName == "HOSCO" && roles.Length > 0 && roles.All(role => role == "Tashilgar") ? R.User.Id : null;
        var query = R.Db.Database.SqlQuery<MeasureObjectDto>($"""
            SELECT CAST(o.strategyobjectid AS bigint) AS Id, o.title AS ObjectTitle,
                (SELECT COUNT_BIG(*) FROM dbo.tbldimmeasure counted
                    JOIN dbo.tbldimmeasurecatalog countedCatalog ON countedCatalog.dimmeasurecatalogid = counted.dimmeasurecatalogid
                    WHERE counted.confirmstatus <> 50 AND (counted.isplanned = {isPlanned} OR {isPlanned} = 0)
                      AND countedCatalog.levelid = {levelId} AND counted.planid = {planId}
                      AND counted.strategyobjectid = o.strategyobjectid) AS MeasureCount,
                (SELECT CAST(SUM(weighted.weight) AS float) FROM dbo.tbldimmeasure weighted
                    JOIN dbo.tbldimmeasurecatalog weightedCatalog ON weightedCatalog.dimmeasurecatalogid = weighted.dimmeasurecatalogid
                    WHERE weighted.confirmstatus <> 50 AND weightedCatalog.levelid = {levelId}
                      AND weighted.planid = {planId} AND weighted.strategyobjectid = o.strategyobjectid) AS TotalWeight
            FROM dbo.tblstrategyobject o
            JOIN dbo.tblplanperiod period ON period.planid = o.planid
            WHERE o.levelid = {levelId} AND period.planid = {planId}
              AND EXISTS (SELECT 1 FROM dbo.tbldimmeasure m
                  LEFT JOIN dbo.tbldimmeasurecatalog c ON c.dimmeasurecatalogid = m.dimmeasurecatalogid
                  LEFT JOIN dbo.tblDimMeasureReporsitory repository ON repository.dimmeasurerepositoryid = c.dimmeasurecatalogrepositoryid
                  LEFT JOIN dbo.tblUser facilitator ON facilitator.userid = m.tashilgar
                  LEFT JOIN dbo.tblUser responsible ON responsible.userid = m.modir
                  WHERE m.strategyobjectid = o.strategyobjectid AND m.confirmstatus <> 50
                    AND (m.isplanned = {isPlanned} OR {isPlanned} = 0)
                    AND ({body.Status} IS NULL OR m.confirmstatus = {body.Status})
                    AND ({body.FilterMeasure} IS NULL OR repository.measurename LIKE '%' + {body.FilterMeasure} + '%')
                    AND ({body.FilterObject} IS NULL OR o.title LIKE '%' + {body.FilterObject} + '%')
                    AND ({body.FilterFacilitator} IS NULL OR CONCAT(facilitator.nameF, ' ', facilitator.familyF) LIKE '%' + {body.FilterFacilitator} + '%')
                    AND ({body.FilterResponsible} IS NULL OR CONCAT(responsible.nameF, ' ', responsible.familyF) LIKE '%' + {body.FilterResponsible} + '%'))
              AND EXISTS (SELECT 1 FROM dbo.tbldimmeasure assigned
                  LEFT JOIN dbo.tbldimmeasurecatalog assignedCatalog ON assignedCatalog.dimmeasurecatalogid = assigned.dimmeasurecatalogid
                  WHERE assigned.strategyobjectid = o.strategyobjectid AND
                    ({facilitatorId} IS NULL OR assigned.tashilgar = {facilitatorId} OR assignedCatalog.tashilgarhamkar = {facilitatorId}))
            """);
        return await PageAsync(query, page, ct);
    }
}
