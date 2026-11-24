// Mechanical conversion from Java (repository/EvaluationFeedBackRepository.java).
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

/// <summary>معادل EvaluationFeedBackRepository.java (اینترفیس جاوا) — در C# با پیشوند I.</summary>
public interface IEvaluationFeedBackRepository : ISemRepository<EvaluationFeedBack,long?>
{
    List<EvaluationFeedBack> findAllByPlanIdAndMonthAndLevelId(long? planId, long? fromMonth, long? toMonth, long? levelId);
    List<EvaluationFeedBack> findAllByEvaluationFeedBackQuestionId(long? evaluationFeedBackQuestionId);
    EvaluationFeedBack findByEvaluationTeamHeaderLevelAndEvaluationFeedBackQuestion(EvaluationTeamHeaderLevel evaluationTeamHeaderLevel, EvaluationFeedBackQuestion evaluationFeedBackQuestion);
    List<Cartable> findAllAsCartable(long? planId, string? filter, bool? isCompleted, string? taskPrefix, string? statusPrefix, string? statusCompleted, List<string?> userRoles, long? userId, string? masterRaterRoleName, string? responsibleRoleName, string? facilitatorRoleName, string? adminRoleName);
    EvaluationFeedBackDto findDtoById(long? id);
    int updateConfirmStatus(long? Id, long? newStatus, string? taskUrl);
    long? findConfirmStatusById(long? id);
}
