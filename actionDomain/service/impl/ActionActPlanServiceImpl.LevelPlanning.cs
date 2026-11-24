using System;
using System.Linq;
using Common.utils;
using SemBackDotNet10.Base.model.dto;
using SemBackDotNet10.Common.enums;
using SemBackDotNet10.Common.exception;
using SemBackDotNet10.SecurityDomain.Service.Impl;

namespace SemBackDotNet10.actionDomain.service.impl;

public partial class ActionActPlanServiceImpl
{
    private void CheckPlanningLevelAccess(long? levelId)
    {
        var userId = SecurityUtils.GetCurrentUserId()
            ?? throw new BusinessException("کاربر جاری یافت نشد");
        _planningLevelUsers.HasAccessInLevelIdAsync(levelId, userId).GetAwaiter().GetResult();
    }

    public SearchDto findAll(long? actionPlanCatalogId, long? levelId,
        string? strategyObjectTitle, int page, int size, string? orderBy, SortDirection direction)
    {
        CheckPlanningLevelAccess(levelId);
        if (actionPlanCatalogId is null) throw new BusinessException("error.ActionActPlan.getAll");
        var request = PageUtils.GetInstance(page, size, orderBy, direction);
        var result = _actionActPlanRepository.FindCatalogLevelPage(actionPlanCatalogId.Value, levelId, request);
        var content = result.Content.ToList();
        foreach (var item in content)
            item.Year = item.PlanId!.Value.ToString().Substring(0, 4);
        return new SearchDto.Builder().content(content).totalElements(result.TotalElements)
            .totalPages(checked((int)result.TotalPages)).currentPage(page).pageSize(size).build();
    }

    public void calculationPlanValue(long? actionPlanCatalogId, long? levelId, long? activePlanId, bool isPlaning)
    {
        CheckPlanningLevelAccess(levelId);
        if (actionPlanCatalogId is null) return; // Java constructs, but does not throw, the exception.
        _planningUnitOfWork.Execute(() =>
        {
            var plans = _actionActPlanRepository.findByActionPlanCatalogIdAndLevelIdGreaterThanActivePlanId(
                actionPlanCatalogId, levelId, activePlanId);
            var previous = _actionActPlanRepository.findByActionPlanCatalogIdAndLevelIdLessThanActivePlanId(
                actionPlanCatalogId, levelId, activePlanId);
            if (plans.Count == 0) throw new BusinessException("برنامه‌ای برای محاسبه یافت نشد.");
            var previousValue = previous.Count == 0 ? (decimal?)null : previous[^1].ActualValue;
            if (!isPlaning && previous.Count != 0 && previousValue is null)
                throw new BusinessException("مقدار پیشرفت واقعی در سال قبل مشخص نشده! ");
            var remaining = isPlaning || previous.Count == 0 ? 100m : 100m - previousValue!.Value;
            var increment = decimal.Round(remaining / plans.Count, 4, MidpointRounding.AwayFromZero);
            var sum = previous.Count == 0 ? increment : increment + previousValue!.Value;
            for (var i = 0; i < plans.Count; i++)
            {
                plans[i].PlanValue = i == plans.Count - 1 ? 100m : sum;
                sum += increment;
            }
        });
    }
}
