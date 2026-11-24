using System;
using System.Collections.Generic;
using SemBackDotNet10.actionDomain.model.entity;
using SemBackDotNet10.actionDomain.model.dto;
using SemBackDotNet10.Common.exception;

namespace SemBackDotNet10.actionDomain.service.impl;

public partial class ActionActPlanServiceImpl
{
    public List<ActionActPlan> findAllWithTaskUrlIsNotNull()
        => _actionActPlanRepository.findAllWithTaskUrlIsNotNull();

    public List<ActionActPlan> findAllByActionPlanCatalogId(long? catalogId)
    {
        if (catalogId is null) throw new BusinessException("error.ActionActPlan.getAll");
        return _actionActPlanRepository.findByActionPlanCatalogIdAndDeletedIsFalse(catalogId);
    }

    public List<ActionActPlanDto> getCorrectionByActPlanId(long? id)
    {
        if (id is null) throw new BusinessException("error.actionActPlan.idIsNotValid");
        var found = _actionActPlanRepository.FindByIdWithCatalogAndPeriod(id)
            ?? throw new BusinessException("error.actionActPlan.idIsNotValid");
        if (found.ActionPlanCatalog is null || found.PlanPeriod is null)
            throw new BusinessException("اطلاعات دوره یا کاتالوگ اقدام موجود نیست.");
        return _actionActPlanRepository.FindCorrectionRows(
            found.ActionPlanCatalog.Id, found.PlanPeriod.PlanId);
    }

    public void restoreDeletedActionActPlan(ActionPlanCatalog actionPlanCatalog)
    {
        ArgumentNullException.ThrowIfNull(actionPlanCatalog);
        if (actionPlanCatalog.Id <= 0)
            throw new BusinessException("error.ActionActPlan.getAll");
        try
        {
            var rows = _actionActPlanRepository
                .findAllByActionPlanCatalogAndDeletedIsTrue(actionPlanCatalog.Id);
            if (rows.Count == 0) return;
            foreach (var row in rows) row.IsDeleted = false;
            _actionActPlanRepository.CommitChanges();
        }
        catch (BusinessException) { throw; }
        catch (Exception)
        {
            // No SQL/server details are exposed. A failed context should be discarded
            // by the request/unit-of-work owner, not reused for another commit.
            throw new BusinessException("خطای پایگاه داده");
        }
    }

    public void deleteActionActPlanByActionPlanCatalog(long? actionPlanCatalogId)
    {
        if (actionPlanCatalogId is null)
            throw new BusinessException("error.ActionActPlan.getAll");
        try
        {
            var rows = _actionActPlanRepository
                .findByActionPlanCatalogIdAndDeletedIsFalse(actionPlanCatalogId);
            if (rows.Count == 0) return;
            foreach (var row in rows) row.IsDeleted = true;
            _actionActPlanRepository.CommitChanges();
        }
        catch (BusinessException) { throw; }
        catch (Exception)
        {
            throw new BusinessException("خطای پایگاه داده");
        }
    }
}
