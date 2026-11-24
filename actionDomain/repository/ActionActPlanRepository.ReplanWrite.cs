using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.actionDomain.model.entity;

namespace SemBackDotNet10.actionDomain.repository;

public sealed partial class ActionActPlanRepository
{
    public void EnsureReplanContext(SemBackDotNet10.actionDomain.persistence.ActionPlanningUnitOfWork unitOfWork)
    {
        if (!unitOfWork.UsesContext(Db))
            throw new System.InvalidOperationException("Replan repository and transaction must share the same DbContext.");
    }

    public ActionPlanCatalog? FindReplanCatalogByActionPlan(long actionPlanId)
        => Db.Set<ActionPlan>().Where(plan => plan.Id == actionPlanId)
            .Select(plan => plan.ActionPlanCatalog).SingleOrDefault();

    public void StageReplanLogs(IEnumerable<ActionActPlanLog> logs)
        => Db.Set<ActionActPlanLog>().AddRange(logs);

    public void StageReplanHistory(ActionPlanCatalog catalog)
    {
        Db.Set<global::Common.model.entity.History>().Add(new global::Common.model.entity.History
        {
            EntityName = nameof(ActionPlanCatalog), EntityId = catalog.Id,
            Status = catalog.ReplanStatusForHistory, OutCome = catalog.OutCome,
            DeleteFirstStatus = catalog.DeleteFirstStatus
        });
        var comments = Db.Set<global::Common.model.entity.Comment>()
            .Where(c => c.EntityId == catalog.Id && c.EntityName == nameof(ActionPlanCatalog)).ToList();
        foreach (var comment in comments) comment.Lock = true;
    }

    // SQL Server tables and the string enum value are taken from the Java mappings.
    // The commands use this repository's connection and its current transaction.
    public void DeletePlanAttachments(IEnumerable<ActionActPlan> plans)
    {
        foreach (var id in plans.Select(plan => plan.Id).Distinct())
        {
            var entityId = id.ToString(CultureInfo.InvariantCulture);
            var resource = global::Common.enums.EntityNames.ACTION_ACT_PLAN.ToString();
            Db.Database.ExecuteSqlInterpolated($@"
DECLARE @fileIds TABLE (file_id bigint PRIMARY KEY);
INSERT INTO @fileIds(file_id)
    SELECT DISTINCT file_id FROM attach.stored_attachment
    WHERE resource = {resource} AND entity_id = {entityId} AND file_id IS NOT NULL;
DELETE FROM attach.stored_attachment WHERE resource = {resource} AND entity_id = {entityId};
DELETE FROM attach.stored_file WHERE file_id IN (SELECT file_id FROM @fileIds);");
        }
    }
}
