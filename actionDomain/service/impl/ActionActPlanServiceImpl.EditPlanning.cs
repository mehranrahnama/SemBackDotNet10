using System;
using System.Collections.Generic;
using System.Globalization;
using SemBackDotNet10.actionDomain.model.dto;
using SemBackDotNet10.Common.exception;
using SemBackDotNet10.SecurityDomain.Service.Impl;

namespace SemBackDotNet10.actionDomain.service.impl;

public partial class ActionActPlanServiceImpl
{
    public void EditActionActPlan(List<ActionActPlanDto> actPlanDtoList)
    {
        _planningUnitOfWork.Execute(() =>
        {
            var first = actPlanDtoList[0];
            var last = actPlanDtoList[actPlanDtoList.Count - 1];
            var catalog = _actionPlanCatalogService.findByIdOrElseThrow(first.ActionPlanCatalogId);
            var start = catalog.StartDate.Split('/');
            var end = catalog.EndDate.Split('/');
            var startPlan = long.Parse(start[0] + "01", CultureInfo.InvariantCulture);
            var endPlan = long.Parse(end[0] + "01", CultureInfo.InvariantCulture);
            var startMonth = long.Parse(start[1], CultureInfo.InvariantCulture);
            var endMonth = long.Parse(end[1], CultureInfo.InvariantCulture);
            if (first.PlanId != startPlan || first.Month != startMonth ||
                last.PlanId != endPlan || last.Month != endMonth)
                throw new BusinessException("برنامه ریزی با تاریخ شروع و پایان اقدام تطابق ندارد. لطفا مجددا برنامه ریزی کنید.");

            var currentUserId = SecurityUtils.GetCurrentUserId()
                ?? throw new BusinessException("کاربر جاری یافت نشد");
            _userService.UserIsAdmin(currentUserId);

            foreach (var dto in actPlanDtoList)
            {
                var plan = findByIdOrElseThrow(dto.Id);
                plan.PlanValue = dto.PlanValue;
                var itemCatalog = _actionPlanCatalogService.findByIdOrElseThrow(dto.ActionPlanCatalogId);
                _actionPlanCatalogService.setIsPlanned(itemCatalog);
            }
        });
    }
}
