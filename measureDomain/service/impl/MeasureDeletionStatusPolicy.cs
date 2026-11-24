using global::Common.model.dto;
using global::Common.service.interf;
using static global::Common.utils.cartable.CartableStatusConstants;

namespace SemBackDotNet10.measureDomain.service.impl;

internal static class MeasureDeletionStatusPolicy
{
    // The Java pair is (old status, new status); the direct branch ends at 50.
    internal static CartableStatusPair Resolve(string tenantName, bool isPlanned, CartableStatusService statuses)
    {
        if (tenantName == "MSCHOLDING" || !isPlanned)
        {
            return new CartableStatusPair
            {
                OldStatus = tenantName is "MSCHOLDING" or "IRISA" ? 36L : 31L,
                NewStatus = FINAL_DELETION_CONFIRMATION_STATUS
            };
        }

        return statuses.FindCartableStatusPair("Measure", OUTCOME_CONFIRM,
            START_DELETION_CONFIRMATION_STATUS, ACTION_DELETE);
    }
}
