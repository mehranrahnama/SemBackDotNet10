using System;
using Common.messages;
using Common.service.interf;
using Common.utils.cartable;
using Microsoft.Extensions.Configuration;
using SemBackDotNet10.actionDomain.domain;
using SemBackDotNet10.actionDomain.model.entity;
using SemBackDotNet10.actionDomain.repository;
using SemBackDotNet10.actionDomain.strategy.confirmStatusHandler.context;
using SemBackDotNet10.Common.exception;

namespace SemBackDotNet10.actionDomain.strategy.confirmStatusHandler.handler;

public class ActionActPlanConfirmStatusHandler(
    CartableStatusService cartableStatusService,
    ActionActPlanRepository repository,
    IConfiguration configuration) : IConfirmStatusHandler
{
    public string entity() => nameof(ActionActPlan);

    // The caller owns the transaction and commits the tracked changes together.
    public void updateCartableStatus(CartableStatusContext context)
    {
        try
        {
            var plan = repository.FindForCorrection(context.EntityId)
                ?? throw new BusinessException(BusinessMessages.DATA_INCONSISTENT);
            var firstRejection = string.Equals(context.Outcome, CartableStatusConstants.OUTCOME_REJECT,
                    StringComparison.OrdinalIgnoreCase)
                && plan.Confirmstatus == CartableStatusConstants.FIRST_CREATION_CONFIRMATION_STATUS;

            if (firstRejection)
                RejectFirstStep(plan);
            else
            {
                var pair = cartableStatusService.FindCartableStatusPair(nameof(ActionActPlan),
                    context.Outcome!, plan.Confirmstatus, CartableStatusConstants.ACTION_CREATE);
                UpdatePlan(plan, pair.NewStatus, pair.OldStatus, context.Outcome);
            }

            if (plan.ActionPlanCatalog is null || plan.PlanPeriod is null)
                throw new BusinessException(BusinessMessages.CARTABLE_DATA_INCONSISTENT);
            var catalogIds = repository.FindConnectedCatalogIds(plan.ActionPlanCatalog.Id);
            var plans = repository.FindPlansForStatusPropagation(catalogIds, plan.Month, plan.PlanPeriod.PlanId);
            if (plans.Count == 0 || plans.Count != catalogIds.Count)
                throw new BusinessException(BusinessMessages.CARTABLE_DATA_INCONSISTENT);
            foreach (var connectedPlan in plans)
            {
                if (firstRejection) RejectFirstStep(connectedPlan);
                else UpdatePlan(connectedPlan, plan.Confirmstatus, connectedPlan.Confirmstatus, plan.OutCome);
            }
        }
        catch (BusinessException) { throw; }
        catch (Exception)
        {
            throw new BusinessException(BusinessMessages.UPDATE_FAILED.Replace("%s", "وضعیت کارتابل"));
        }
    }

    private static void RejectFirstStep(ActionActPlan plan)
    {
        plan.OutCome = CartableStatusConstants.OUTCOME_REJECT;
        plan.Confirmstatus = CartableStatusConstants.START_CREATION_CONFIRMATION_STATUS;
        plan.TaskUrl = null;
    }

    private void UpdatePlan(ActionActPlan plan, long? newStatus, long? oldStatus, string? outcome)
    {
        if (newStatus is null)
            throw new BusinessException(BusinessMessages.CARTABLE_INVALID_STATE);
        var baseUrl = configuration["taskUrl"]
            ?? throw new InvalidOperationException("taskUrl is not configured.");
        plan.TaskUrl = new ActionTaskUrlBuilder(baseUrl).generateProgressTaskUrl(plan.Id, newStatus, oldStatus);
        plan.Confirmstatus = newStatus.Value;
        plan.OutCome = outcome;
    }
}
