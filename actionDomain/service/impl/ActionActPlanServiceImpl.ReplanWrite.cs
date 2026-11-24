using System;
using System.Collections.Generic;
using System.Globalization;
using SemBackDotNet10.actionDomain.domain;
using SemBackDotNet10.actionDomain.model.dto;
using SemBackDotNet10.actionDomain.model.entity;
using SemBackDotNet10.Common.exception;

namespace SemBackDotNet10.actionDomain.service.impl;

public partial class ActionActPlanServiceImpl
{
    public void saveReplanValues(List<ActionActReplanDto> changes, Dictionary<string?, string?> actionInfo)
    {
        try
        {
            _actionActPlanRepository.EnsureReplanContext(_planningUnitOfWork);
            _planningUnitOfWork.Execute(() =>
            {
                if (!actionInfo.TryGetValue("actionPlanId", out var value)
                    || !long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var actionPlanId))
                    throw new BusinessException("شناسه اقدام معتبر نیست.");
                var catalog = _actionActPlanRepository.FindReplanCatalogByActionPlan(actionPlanId)
                    ?? throw new BusinessException("کاتالوگ اقدام یافت نشد.");
                if (catalog.ReplannedStatus is not null and not 0 and not 20)
                    throw new BusinessException(" اقدام در روال تغییر برنامه ریزی می باشد");

                runReplanProcess(catalog);
                var logs = ActionActReplanChanges.Apply(changes, catalog, id =>
                    _actionActPlanRepository.FindByIdWithCatalogAndPeriod(id)
                        ?? throw new BusinessException("error.actionActPlan.idIsNotValid"));
                _actionActPlanRepository.StageReplanLogs(logs);
            });
        }
        catch (BusinessException) { throw; }
        catch (Exception error) { throw new BusinessException(error.Message); }
    }

    private void runReplanProcess(ActionPlanCatalog catalog)
    {
        var tenant = RequiredTenantName();
        catalog.DeleteFirstStatus = catalog.IsReplanned ? 1L : 0L;
        catalog.ReplannedForHistory = true;
        catalog.IsReplanned = true;
        long? newStatus;
        if (tenant != "MSCHOLDING" && tenant != "IRISA")
        {
            newStatus = _cartableStatusService.FindNewStatus(nameof(ActionPlanCatalog), "replanConfirm", 0L);
            var baseUrl = _configuration["taskUrl"]
                ?? throw new InvalidOperationException("taskUrl is not configured.");
            // This Java workflow concatenates the query parameters directly.
            catalog.TaskUrl = baseUrl + "Action/ActionReplan/ActionReplanCartable&entityId=" + catalog.Id
                + "&oldStatus=" + catalog.ReplannedStatus + "&newStatus=" + newStatus;
        }
        else
        {
            newStatus = 20L;
            var expired = _actionActPlanRepository.findAllByActionPlanCatalogAndDeletedIsTrue(catalog.Id);
            _actionActPlanRepository.DeleteEvaluationsForPlans(expired);
            _actionActPlanRepository.DeletePlanAttachments(expired);
            _actionActPlanRepository.StageRemoval(expired);
        }
        catalog.ReplannedStatus = newStatus;
        catalog.ReplanStatusForHistory = newStatus
            ?? throw new BusinessException("وضعیت بازبرنامه‌ریزی معتبر نیست.");
        catalog.OutCome = "replanConfirm";
        _actionActPlanRepository.StageReplanHistory(catalog);
    }
}
