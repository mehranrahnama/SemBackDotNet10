using System;
using System.Collections.Generic;
using System.Linq;
using Common.controller.dto.response;
using Common.enums;
using SemBackDotNet10.actionDomain.domain;
using SemBackDotNet10.actionDomain.model.dto;
using SemBackDotNet10.actionDomain.model.entity;
using SemBackDotNet10.Common.exception;

namespace SemBackDotNet10.actionDomain.service.impl;

public partial class ActionActPlanServiceImpl
{
    private string RequiredTenantName() => _configuration["sem.default.tenant.name"]
        ?? _configuration["sem:default:tenant:name"]
        ?? throw new InvalidOperationException("sem.default.tenant.name is not configured.");

    public void editAllActualValueAndBudget(List<ActionActPlanDto> dtos)
    {
        var tenant = RequiredTenantName();
        _planningUnitOfWork.Execute(() =>
        {
            if (tenant == "HOSCO") ValidateProgressAttachments(dtos);
            var errors = new List<RecordErrorResDto>();
            foreach (var dto in dtos)
            {
                var current = FindCorrectionPlan(dto.Id);
                try
                {
                    var catalogId = current.ActionPlanCatalog!.Id;
                    var planId = current.PlanPeriod!.PlanId;
                    var previous = tenant == "IRISA" ? null : _actionActPlanRepository.FindPreviousMonth(
                        catalogId, current.Month - 1, planId);
                    ActionActPlanValidationRules.validatePreviousMonth(tenant, previous, dto);
                    ActionActPlanValidationRules.validateActualValue(dto);
                    current.ActualValue = dto.ActualValue;
                    if (dto.ActualValue is null) current.ActionPCN = null;
                    _actionActPlanRepository.CommitChanges();

                    if (tenant == "IRISA")
                    {
                        foreach (var future in _actionActPlanRepository.FindFutureMonths(catalogId, current.Month))
                        {
                            if (future.ActualValue is not null) continue;
                            future.ActualValue = dto.ActualValue;
                            var final = _actionActPlanRepository.GetFinalPlanValue(catalogId, planId);
                            if (current.ActualValue.HasValue && final.HasValue && current.ActualValue >= final)
                                foreach (var sameYear in _actionActPlanRepository.FindFutureMonthsInPeriod(catalogId, current.Month, planId))
                                    sameYear.ActualValue = current.ActualValue;
                        }
                    }
                    PropagateProgress(catalogId);
                }
                catch (BusinessException error)
                {
                    errors.Add(new RecordErrorResDto { Id = dto.Id,
                        Title = current.ActionPlanCatalog!.Description + " - ماه " + current.Month,
                        Message = error.Message });
                }
            }
            if (errors.Count != 0)
                throw new BusinessException("ثبت پیشرفت به دلیل خطا در برخی ردیف‌ها انجام نشد.", errors);
        });
    }

    private void ValidateProgressAttachments(List<ActionActPlanDto> dtos)
    {
        var ids = dtos.Where(x => x.ActualValue == 100m).Select(x => (long?)FindCorrectionPlan(x.Id).ActionPlanCatalog!.Id).ToList();
        if (ids.Count == 0) return;
        var activities = _actionPlanActivityRepository.findAllByActionPlanCatalogIdIn(ids);
        var entityIds = activities.Select(x => x.EntityId!).ToList();
        var uploaded = _storedAttachmentRepository.FindEntityIdsWithFile(EntityNames.ACTION_PLAN_ACTIVITY, entityIds).ToHashSet();
        var errors = activities.Where(x => !uploaded.Contains(x.EntityId!))
            .GroupBy(x => x.ActionPlanCatalog!.Id)
            .ToDictionary(x => x.Key, x => x.Select(a => a.ActivityName).ToList());
        if (errors.Count > 0)
            throw new BusinessException("برای ثبت پیشرفت 100 درصد، تمام فعالیت‌ها باید دارای فایل پیوست باشند.", errors);
    }

    private void PropagateProgress(long catalogId)
    {
        var ancestors = _actionPlanCatalogRelationService.findAncestorIds(catalogId);
        if (ancestors is null || ancestors.Count == 0) return;
        var source = _actionActPlanRepository.findAllByActionPlanCatalogIdAndDeletedFalseSortedByPlanIdAndMonth(catalogId);
        if (source.Count == 0) throw new BusinessException("رکورد های برنامه ریزی اقدام فرزند، نمیتواند خالی باشد.!");
        foreach (var id in ancestors)
        {
            var target = _actionActPlanRepository.findAllByActionPlanCatalogIdAndDeletedFalseSortedByPlanIdAndMonth(id);
            if (target.Count == 0) throw new BusinessException("تعداد ماه های برنامه ریزی اقدام والد نمیتواند خالی باشد!");
            if (target.Count != source.Count)
                throw new BusinessException("تعداد ماه های برنامه ریزی اقدام والد، باید دقیقا برابر " + source.Count + "باشد!");
            for (var i = 0; i < source.Count; i++)
                _actionActPlanMapper.copyDailyValueAndActualValue(source[i], target[i]);
        }
        _actionActPlanRepository.CommitChanges();
    }
}
