using System;
using System.Linq;
using System.Text.Json;
using Common.utils;
using SemBackDotNet10.Base.model.dto;
using SemBackDotNet10.Common.enums;
using SemBackDotNet10.Common.exception;

namespace SemBackDotNet10.actionDomain.service.impl;

public partial class ActionActPlanServiceImpl
{
    public SearchDto loadReplanCartableGrid(string? body, int page, int size, string? orderBy, SortDirection direction)
    {
        using var json = JsonDocument.Parse(body!);
        var root = json.RootElement;
        long? catalogId = root.TryGetProperty("actionPlanCatalogId", out var id) && id.ValueKind != JsonValueKind.Null
            ? id.GetInt64() : null;
        var activePeriod = root.TryGetProperty("ActivePlanPeriod", out var period) && period.ValueKind != JsonValueKind.Null
            ? period.GetString() : null;
        var catalog = _actionPlanCatalogService.findByIdOrElseThrow(catalogId);
        var dateLog = _actionDateLogService.findByActionPlanCatalogOrElseThrow(catalog);
        if (catalog.PlaningLevel is null)
            throw new BusinessException("error.PlaningLevel.idIsNotValid");
        var levelName = _actionActPlanRepository.FindGridLevelName(catalog.PlaningLevel.Id)
            ?? throw new BusinessException("error.PlaningLevel.idIsNotValid");
        var request = PageUtils.GetInstance(page, size);
        var active = _actionActPlanRepository.FindReplanPage(catalog.Id, request, false);
        var expired = _actionActPlanRepository.FindReplanPage(catalog.Id, request, true);
        var content = active.Content.ToList();
        if (expired.Content.Count > 0)
        {
            foreach (var item in content)
            {
                var old = expired.Content.FirstOrDefault(x => x.Year == item.Year && x.Month == item.Month);
                if (old != null)
                {
                    item.PlanValueNew = item.PlanValue;
                    item.PlanValue = old.PlanValue;
                    item.ActualValueNew = item.ActualValue;
                    item.ActualValue = old.ActualValue;
                }
                else if (long.Parse(item.Year!) >= long.Parse(activePeriod!.Substring(0, 4)))
                {
                    item.PlanValueNew = item.PlanValue;
                    item.PlanValue = null;
                }
                if (dateLog != null)
                {
                    item.EndDate = dateLog.EndDateOld;
                    item.ReasonEndDateChange = dateLog.ReasonEndDateChange;
                }
                item.ActionName = catalog.Description;
                item.StartDate = catalog.StartDate;
                item.DesLevel = levelName;
            }
        }
        return new SearchDto.Builder().content(content).totalElements(active.TotalElements)
            .totalPages(checked((int)active.TotalPages)).currentPage(page).pageSize(size).build();
    }

    public SearchDto loadReplanGrid(string? body, int page, int size, string? orderBy, SortDirection direction)
    {
        using var json = JsonDocument.Parse(body!);
        var root = json.RootElement;
        long? actionPlanId = root.TryGetProperty("actionPlanId", out var id) && id.ValueKind != JsonValueKind.Null
            ? id.GetInt64() : null;
        var activePeriod = root.TryGetProperty("ActivePlanPeriod", out var period) && period.ValueKind != JsonValueKind.Null
            ? period.GetString() : null;
        var catalog = _actionPlanCatalogService.findByActionPlanIdOrElseThrow(actionPlanId);
        var request = PageUtils.GetInstance(page, size);
        var active = _actionActPlanRepository.FindReplanPage(catalog.Id, request, false);
        var expired = _actionActPlanRepository.FindReplanPage(catalog.Id, request, true);
        var content = active.Content.ToList();
        if (expired.Content.Count > 0)
        {
            var log = _actionDateLogService.findByActionPlanCatalogOrElseThrow(catalog);
            foreach (var item in content)
            {
                if (log != null)
                {
                    item.ReasonEndDateChange = log.ReasonEndDateChange;
                    item.EndDate = log.EndDateNew;
                }
                var old = expired.Content.FirstOrDefault(x => x.Year == item.Year && x.Month == item.Month);
                if (old != null)
                {
                    item.PlanValueNew = item.PlanValue;
                    item.PlanValue = old.PlanValue;
                    item.ActualValueNew = item.ActualValue;
                    item.ActualValue = old.ActualValue;
                }
                else if (long.Parse(item.Year!) >= long.Parse(activePeriod!.Substring(0, 4)))
                {
                    item.PlanValueNew = item.PlanValue;
                    item.PlanValue = null;
                    item.ActualValueNew = item.ActualValue;
                }
            }
        }
        return new SearchDto.Builder().content(content).totalElements(active.TotalElements)
            .totalPages(checked((int)active.TotalPages)).currentPage(page).pageSize(size).build();
    }
}
