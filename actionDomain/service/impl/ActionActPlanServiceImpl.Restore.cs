#nullable enable
namespace SemBackDotNet10.actionDomain.service.impl;

public partial class ActionActPlanServiceImpl
{
    // This is ONLY the one-argument overload from the Java source.
    // It does not perform the attachment cleanup of the other overload.
    // Caller owns an encompassing transaction if both commands must be atomic.
    public void restoreActionActPlans(long? actionPlanCatalogId)
    {
        _actionActPlanRepository.removeActiveActionActPlansByActionPlanCatalog(actionPlanCatalogId);
        _actionActPlanRepository.restoreRemovedActionActPlanByActionPlanCatalog(actionPlanCatalogId);
    }
}
