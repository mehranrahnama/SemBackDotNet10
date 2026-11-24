using System;
using System.Collections.Generic;
using System.Globalization;
using SemBackDotNet10.actionDomain.model.dto;
using SemBackDotNet10.actionDomain.model.entity;
using SemBackDotNet10.Common.exception;

namespace SemBackDotNet10.actionDomain.service.impl;

public partial class ActionActPlanServiceImpl
{
    public void setTaskUrl(List<ActionActPlanDto> dtos)
    {
        var tenant = RequiredTenantName();
        _planningUnitOfWork.Execute(() =>
        {
            foreach (var dto in dtos)
            {
                var plan = FindCorrectionPlan(dto.Id);
                var newStatus = 20L;
                var useConfiguredStatus = tenant == "HOSCO" || tenant == "SANGAN" &&
                    _cartableStatusService.GetRepository().FindByEntityNameAndOperationTypeAndOutCome(
                        nameof(ActionActPlan), "create", "confirm").Count != 0;
                if (tenant == "HOSCO" && !plan.ActionPlanCatalog!.IsPlanned)
                    throw new BusinessException(" برنامه ریزی اقدام های مورد نظر تایید نشده است");
                if (useConfiguredStatus)
                {
                    newStatus = _cartableStatusService.FindNewStatus(nameof(ActionActPlan), dto.OutCome!, dto.Confirmstatus)
                        ?? throw new InvalidOperationException("The configured ActionActPlan transition has no target status.");
                    var baseUrl = _configuration["taskUrl"]
                        ?? throw new InvalidOperationException("taskUrl is not configured.");
                    // This workflow concatenates '&' directly in the Java source.
                    plan.TaskUrl = baseUrl + "Progress/ActionProgress/ActionProgressCartable&entityId="
                        + plan.Id + "&oldStatus=" + plan.Confirmstatus + "&newStatus=" + newStatus;
                }
                plan.Confirmstatus = newStatus;
                ExtendFinalMonthIfNeeded(plan, dto);
            }
        });
    }

    private void ExtendFinalMonthIfNeeded(ActionActPlan plan, ActionActPlanDto dto)
    {
        var catalog = plan.ActionPlanCatalog!;
        var date = catalog.EndDate.Split('/');
        var year = long.Parse(date[0], CultureInfo.InvariantCulture);
        var endMonth = long.Parse(date[1], CultureInfo.InvariantCulture);
        var endPlanId = long.Parse(date[0] + "01", CultureInfo.InvariantCulture);
        if (plan.PlanPeriod!.PlanId != endPlanId || plan.Month != endMonth) return;
        if (dto.ActualValue is null) throw new BusinessException("مقدار پیشرفت واقعی مشخص نشده است.");
        if (dto.ActualValue >= 100m) return;

        var nextYear = endMonth == 12 ? year + 1 : year;
        var nextMonth = endMonth == 12 ? 1 : endMonth + 1;
        catalog.EndDate = endMonth == 12 ? $"{nextYear}/01/31" : $"{year}/{nextMonth}/30";
        _actionPlanCatalogService.SaveAsync(catalog).GetAwaiter().GetResult();
        if (endMonth == 12) _actionPlanCatalogService.checkExistOrCreateActionPlan(catalog);
        var period = endMonth == 12
            ? _planPeriodService.FindByPlanIdOrCreate(long.Parse(nextYear.ToString(CultureInfo.InvariantCulture) + "01", CultureInfo.InvariantCulture))
            : plan.PlanPeriod;
        var newPlan = new ActionActPlan
        {
            Month = nextMonth, ActionPlanCatalog = catalog, PlanPeriod = period,
            PlanValue = plan.PlanValue, Confirmstatus = 0L
        };
        _actionActPlanRepository.StageNewActionActPlan(newPlan);
        if (_actionPlanService.isAddToSupremeLevel(catalog.Id))
            foreach (var ancestor in _actionPlanCatalogRelationService.findAncestorEntity(catalog.Id))
                _actionActPlanRepository.StageNewActionActPlan(_actionActPlanMapper.clone(newPlan, ancestor)!);
    }
}
