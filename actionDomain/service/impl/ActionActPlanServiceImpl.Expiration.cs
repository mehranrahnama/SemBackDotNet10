using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using SemBackDotNet10.actionDomain.model.entity;
using SemBackDotNet10.Common.exception;
using SemBackDotNet10.SecurityDomain.Service.Impl;

namespace SemBackDotNet10.actionDomain.service.impl;

public partial class ActionActPlanServiceImpl
{
    public void extendEndDateOfExpiredProjectByFilter(string? body)
    {
        using var json = JsonDocument.Parse(body!);
        var root = json.RootElement;
        long? levelId = root.TryGetProperty("levelId", out var level) && level.ValueKind != JsonValueKind.Null
            ? level.GetInt64() : null;
        long? planId = root.TryGetProperty("planId", out var plan) && plan.ValueKind != JsonValueKind.Null
            ? plan.GetInt64() : null;
        var userId = SecurityUtils.GetCurrentUserId()
            ?? throw new BusinessException("کاربر جاری یافت نشد");
        CheckPlanningLevelAccess(levelId);
        _planningUnitOfWork.Execute(() =>
            ExtendExpiredCatalogs(_actionPlanCatalogService.findByPlanIdAndLevelIdAndPayeshgar(planId, levelId, userId)));
    }

    public void extendEndDateOfExpiredProject()
    {
        _planningUnitOfWork.Execute(() =>
            ExtendExpiredCatalogs(_actionPlanCatalogService.findAllConnectedActions()));
    }

    private void ExtendExpiredCatalogs(List<ActionPlanCatalog> catalogs)
    {
        var calendar = new PersianCalendar();
        var now = DateTime.Now;
        var year = calendar.GetYear(now);
        var month = calendar.GetMonth(now);
        foreach (var catalog in catalogs)
        {
            var date = catalog.EndDate.Split('/');
            var endYear = int.Parse(date[0], CultureInfo.InvariantCulture);
            var endMonth = int.Parse(date[1], CultureInfo.InvariantCulture);
            if (year < endYear || (year == endYear && month <= endMonth)) continue;

            var incomplete = false;
            foreach (var actionPlan in _actionPlanService.findByCatalogId(catalog.Id))
            {
                // The Java query receives actionPlan.id, despite its catalog-id parameter name.
                var (planValue, actualValue) = _actionActPlanRepository.GetFinalValues(actionPlan.Id);
                if (planValue.HasValue && actualValue.HasValue && actualValue < planValue)
                    incomplete = true;
            }
            if (!incomplete) continue;
            var nextYear = month == 12 ? year + 1 : year;
            var nextMonth = month == 12 ? 1 : month + 1;
            catalog.EndDate = string.Format(CultureInfo.InvariantCulture, "{0}/{1:00}/01", nextYear, nextMonth);
            _actionPlanCatalogService.SaveAsync(catalog).GetAwaiter().GetResult();
        }
    }
}
