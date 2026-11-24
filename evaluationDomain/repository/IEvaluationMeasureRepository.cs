using global::SemBackDotNet10.Common.Pagination;
// Mechanical conversion from Java (repository/EvaluationMeasureRepository.java).
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

/// <summary>معادل EvaluationMeasureRepository.java (اینترفیس جاوا) — در C# با پیشوند I.</summary>
public interface IEvaluationMeasureRepository : ISemRepository<EvaluationMeasure,long?>
{
    EvaluationMeasure findByDimMeasureActId(long? measureActId);
    List<EvaluationMeasurePageDto> findAllMeasurePage(long? planId, long? fromMonth, long? toMonth, string? filter);
    List<EvaluationMeasurePageDto> findAllEvalMeasurePageForMasterRater(long? planId, long? fromMonth, long? toMonth, long? currentUserId, string? filter);
    PagedResult<EvaluationMeasureDto> findAllMeasureDetails(Pageable pageable, long? planId, List<long?> levelId, long? fromMonth, long? toMonth, long? currentUserId, bool? isAdmin, string? filter, long? positionId);
    List<EvaluationMeasure> findAllByEvaluationTeamHeaderLevelId(long? evaluationTeamHeaderLevelId);
    EvaluationMeasureDto findEvaluationMeasureDtoById(long? evaluationMeasureId);
    List<EvaluationMeasure> findAllEvaluationByMeasureActPlan(List<MeasureActPlan> measureActPlanList);
    List<EvaluationMeasure> inConfirmProcessEvaluationMeasure();
    List<EvaluationMeasure> findByConfirmStatusAndDeletedIsFalseAndDailyValueIsNotNullAndActualValueIsNotNullAndConfirmStatusIgnoreCase(long? confirmStatus, List<long?> ignoredConfirmStatus, long? planId, string? assigneeRole, string? prefix, string? filter);
    List<EvaluationMeasure> findByConfirmStatusAndDeletedIsFalseAndDailyValueIsNotNullAndActualValueIsNotNull(long? confirmStatus, long? planId, string? assigneeRole, string? prefix, string? filter);
    void deleteAllByMeasureActPlanIds(List<long?> measureActPlanIds);
}
