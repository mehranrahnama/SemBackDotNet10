using SemBackDotNet10.actionDomain.domain;
using global::Common.service.interf;
using SemBackDotNet10.measureDomain.model.entity;
using static global::Common.utils.cartable.CartableStatusConstants;

namespace SemBackDotNet10.measureDomain.service.impl;

// Apply only after the transaction has removed prior deleted copies and passive plans/logs.
internal static class MeasureDeletionTransition
{
    internal static void Apply(Measure measure, IReadOnlyList<MeasureActPlan> retainedPlans,
        string tenantName, string taskBaseUrl, CartableStatusService statuses)
    {
        if (measure.Deleted) return;
        var id = measure.Id ?? throw new InvalidOperationException("The measure must already be persisted.");
        var pair = MeasureDeletionStatusPolicy.Resolve(tenantName, measure.Planned, statuses);
        var newStatus = pair.NewStatus ?? throw new InvalidOperationException("The deletion status is missing.");
        var url = BuildTaskUrl(taskBaseUrl, id, pair.OldStatus, newStatus);

        foreach (var plan in retainedPlans)
        {
            plan.Deleted = true;
            plan.Confirmstatus = newStatus;
            plan.TaskUrl = url;
            plan.OutCome = OUTCOME_CONFIRM;
        }

        measure.DeleteFirstStatus = measure.Confirmstatus ?? 0L;
        measure.Deleted = true;
        measure.Confirmstatus = newStatus;
        measure.TaskUrl = url;
        measure.OutCome = OUTCOME_CONFIRM;
    }

    internal static string BuildTaskUrl(string taskBaseUrl, long id, long? oldStatus, long? newStatus)
        => new ActionTaskUrlBuilder(taskBaseUrl).Build(
            "Measurement/MeasureManagement/MeasureManagementCartable", id, newStatus, oldStatus);
}
