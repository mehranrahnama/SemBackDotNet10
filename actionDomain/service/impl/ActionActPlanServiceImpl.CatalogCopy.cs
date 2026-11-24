using System;
using System.Collections.Generic;
using SemBackDotNet10.actionDomain.model.entity;
using SemBackDotNet10.Common.exception;

namespace SemBackDotNet10.actionDomain.service.impl;

public partial class ActionActPlanServiceImpl
{
    public void copyAndCreateActionActPlan(List<ActionPlanCatalog> actionPlanCatalogs, ActionPlanCatalog baseActionPlanCatalog)
    {
        if (actionPlanCatalogs is null || actionPlanCatalogs.Count == 0 || baseActionPlanCatalog is null) return;
        _planningUnitOfWork.Execute(() =>
        {
            var source = _actionActPlanRepository.FindPlansForCatalogCopy(baseActionPlanCatalog.Id);
            var changed = new List<ActionActPlan>();
            foreach (var catalog in actionPlanCatalogs)
            {
                if (catalog is null) continue;
                foreach (var plan in source)
                {
                    var existing = _actionActPlanRepository.findByActionPlanCatalogIdAndMonthAndPlanPeriodAndDeletedFalse(
                        catalog.Id, plan.Month, plan.PlanPeriod?.PlanId);
                    if (existing != null)
                    {
                        _actionActPlanMapper.copySupremeLevelValues(plan, existing);
                        changed.Add(existing);
                        continue;
                    }
                    try
                    {
                        var clone = _actionActPlanMapper.clone(plan, catalog);
                        if (clone != null)
                        {
                            _actionActPlanRepository.StageNewActionActPlan(clone);
                            changed.Add(clone);
                        }
                    }
                    catch (Exception)
                    {
                        // Java skips a source record when the mapper cannot clone it.
                    }
                }
            }
            if (changed.Count == 0) return;
            try
            {
                _actionActPlanRepository.CommitChanges();
                foreach (var plan in changed)
                    if (plan.TaskUrl != null)
                        plan.TaskUrl = System.Text.RegularExpressions.Regex.Replace(plan.TaskUrl, @"entityId=\d+", "entityId=" + plan.Id);
                _actionActPlanRepository.CommitChanges();
            }
            catch (Exception)
            {
                throw new BusinessException("خطایی در فرآیند ذخیره برنامه‌ریزی رخ داده است.");
            }
        });
    }
}
