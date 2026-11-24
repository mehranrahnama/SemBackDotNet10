#nullable enable
using SemBackDotNet10.actionDomain.model.entity;
using SemBackDotNet10.baseDomain.repository;

namespace SemBackDotNet10.actionDomain.repository;

/// <summary>Spring ActionDateLogRepository contract and the shared .NET service contract.</summary>
public interface ActionDateLogRepository : ISemRepository<ActionDateLog, long?>,
    global::SemBackDotNet10.Base.repository.ISemRepository<ActionDateLog, long?>
{
    ActionDateLog? findFirstByActionPlanCatalogOrderByModifiedDateDesc(ActionPlanCatalog actionPlanCatalog);
    ActionDateLog? findByActionPlanCatalogAndLastModified(long? actionPlanCatalogId);
    int countByActionPlanCatalogId(long? actionPlanCatalogId);
    ActionDateLog? findFirstByActionPlanCatalogIdOrderByModifiedDateAsc(long? actionPlanCatalogId);
}
