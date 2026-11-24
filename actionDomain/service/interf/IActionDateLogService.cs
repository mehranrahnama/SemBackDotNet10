#nullable enable
using SemBackDotNet10.actionDomain.model.entity;
using SemBackDotNet10.actionDomain.repository;
using SemBackDotNet10.Base.Service.Inter;

namespace SemBackDotNet10.actionDomain.service.interf;

public interface IActionDateLogService : IBaseService<ActionDateLog, long?, ActionDateLogRepository>
{
    ActionDateLog? findByActionPlanCatalogOrElseThrow(ActionPlanCatalog actionPlanCatalog);
    ActionDateLog? getOldestEndDateByActionPlanCatalogId(long? actionPlanCatalogId);
}
