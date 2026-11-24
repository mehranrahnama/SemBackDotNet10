using System;
using System.Collections.Generic;
using SemBackDotNet10.actionDomain.controller.dto.actionActPlanDtos;

namespace SemBackDotNet10.actionDomain.repository;

public sealed partial class ActionActPlanRepository
{
    /// <summary>
    /// Stages Java updatePlanValues' initial per-ID PlanValue assignments.
    /// Does not save, calculate percentages, create a transaction, or run propagation.
    /// Caller must validate DTOs and own the complete workflow transaction.
    /// </summary>
    public void StagePlanValues(IEnumerable<UpdateActionActPlanDto> dtos)
    {
        ArgumentNullException.ThrowIfNull(dtos);
        foreach (var dto in dtos)
        {
            ArgumentNullException.ThrowIfNull(dto);
            var entity = FindById(dto.ActionActPlanId)
                ?? throw new KeyNotFoundException(
                    $"ActionActPlan with ID '{dto.ActionActPlanId}' was not found.");
            // Source addresses rows by DTO ID only. Do not introduce catalog,
            // year, month, or deleted restrictions here. Duplicate IDs are applied
            // in input order; the last supplied value wins.
            entity.PlanValue = dto.PlanValue;
        }
    }
}
