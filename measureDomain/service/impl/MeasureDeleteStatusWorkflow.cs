using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.Common.exception;
using SemBackDotNet10.Data;
using SemBackDotNet10.service.impl;
using global::Common.service.interf;

namespace SemBackDotNet10.measureDomain.service.impl;

// Internal workflow; the caller must supply the configured task base URL.
internal sealed class MeasureDeleteStatusWorkflow(ServiceRuntime runtime, CartableStatusService statuses)
{
    internal Task ExecuteAsync(MeasureDeleteStatusRequest request, string taskBaseUrl, CancellationToken ct)
        => runtime.TransactionAsync(async () =>
        {
            var db = (SemDbContext)runtime.Db;
            var source = await db.Database.SqlQuery<SourceRow>($"""
                SELECT CAST(m.dimmeasurecatalogid AS bigint) AS CatalogId,
                       CAST(m.planid AS bigint) AS PlanId,
                       CAST(p.levelid AS bigint) AS LevelId,
                       CAST(p.parentid AS bigint) AS ParentId,
                       CAST(m.isdeleted AS bit) AS Deleted
                FROM dbo.tbldimmeasure m WITH (UPDLOCK, HOLDLOCK)
                LEFT JOIN dbo.tblstrategyobject o ON o.strategyobjectid = m.strategyobjectid
                LEFT JOIN dbo.tblplaninglevel p ON p.levelid = o.levelid
                WHERE m.measureid = {request.MeasureId}
                """).SingleOrDefaultAsync(ct)
                ?? throw new BusinessException("error.measure.idIsNotValid");

            await new PlaningLevelUserServiceImpl(runtime)
                .HasAccessInLevelIdAsync(source.LevelId, runtime.User.Id, ct);
            var isAdmin = runtime.User.IsAdmin;
            request.Validate(source.Deleted, isAdmin, source.ParentId.HasValue);

            var tenantName = runtime.TenantName;
            if (!request.OnlyThisLevel)
                await new MeasureDeletionPropagation(db, statuses).ExecuteAsync(request.MeasureId,
                    source.CatalogId, source.PlanId, isAdmin, tenantName, taskBaseUrl, ct);

            await db.Dashboards.Where(d => d.MeasureId == request.MeasureId).ExecuteDeleteAsync(ct);
            await new MeasureSoftDeletion(db, statuses).ExecuteAsync(request.MeasureId, source.PlanId,
                tenantName, taskBaseUrl, ct);

            if (request.OnlyThisLevel)
                await new MeasureDeletionOutgoingRelations(db).RemoveAsync(source.CatalogId, source.PlanId, tenantName, ct);
        }, ct);

    private sealed class SourceRow
    {
        public long CatalogId { get; set; }
        public long PlanId { get; set; }
        public long? LevelId { get; set; }
        public long? ParentId { get; set; }
        public bool Deleted { get; set; }
    }
}
