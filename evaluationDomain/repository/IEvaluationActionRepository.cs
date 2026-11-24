// Mechanical conversion from Java (repository/EvaluationActionRepository.java).
// JPQL @Query ها به LINQ/EF Core ترجمه شوند؛ امضاهای متد عیناً حفظ شده‌اند.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SemBackDotNet10.evaluationDomain.model.entity;
using SemBackDotNet10.evaluationDomain.model.dto;
using SemBackDotNet10.actionDomain.model.entity;
using SemBackDotNet10.measureDomain.model.entity;
using global::Common.model.entity;
using SemBackDotNet10.basicDomain.Model.Entity;
using global::Common.model.dto;
using global::Common.repository;
using SemBackDotNet10.Base.repository;

namespace SemBackDotNet10.evaluationDomain.repository;

/// <summary>معادل EvaluationActionRepository.java (اینترفیس جاوا) — در C# با پیشوند I.</summary>
public interface IEvaluationActionRepository : ISemRepository<EvaluationAction,long?>
{
    EvaluationAction findByActionActId(long? actionActId);
    List<EvaluationActionPageDto> findAllActionPage(long? planId, long? fromMonth, long? toMonth);
    List<EvaluationActionPageDto> findAllActionPage_hosco(long? planId);
    List<EvaluationActionPageDto> findAllActionEvalPageForMasterRater(long? planId, long? fromMonth, long? toMonth, long? masterRater);
    List<EvaluationActionPageDto> findAllActionEvalPageForMasterRater_hosco(long? planId, long? masterRater);
    List<EvaluationAction> findAllByEvaluationTeamHeaderLevelId(long? evaluationTeamHeaderLevelId);
    EvaluationActionDto findEvaluationActionDtoById(long? evaluationActionId);
    List<EvaluationAction> findAllEvaluationByActionActPlan(List<ActionActPlan> actionActPlans);
    List<EvaluationAction> findByConfirmStatusAndDeletedIsFalseAndDailyValueIsNotNullAndActualValueIsNotNullAndConfirmStatusIgnoreCase(long? confirmStatus, List<long?> ignoredConfirmStatus, long? planId, string? assigneeRole, string? prefix, string? filter);
    List<EvaluationAction> findByConfirmStatusAndDeletedIsFalseAndDailyValueIsNotNullAndActualValueIsNotNull(long? confirmStatus, long? planId, string? assigneeRole, string? prefix, string? filter);
    void deleteAllByActionPlanCatalogId(long? actionPlanCatalogId);
}
