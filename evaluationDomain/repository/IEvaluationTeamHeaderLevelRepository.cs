using EvalLevel = SemBackDotNet10.evaluationDomain.model.entity.EvaluationTeamHeaderLevel;
// Mechanical conversion from Java (repository/EvaluationTeamHeaderLevelRepository.java).
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

/// <summary>معادل EvaluationTeamHeaderLevelRepository.java (اینترفیس جاوا) — در C# با پیشوند I.</summary>
public interface IEvaluationTeamHeaderLevelRepository : ISemRepository<EvalLevel,long?>
{
    EvalLevel findFirstByPlanIdAndLevelIdAndMonth(long? planId, long? levelId, long? month);
    EvalLevel findFirstByPlanIdAndLevelId_hosco(long? planId, long? levelId);
    List<EvalLevel> findAllIdByEvaluationTeamHeaderId(long? evaluationTeamHeaderId);
    EvalLevel findByPlanIdAndMonthAndLevelId(long? planId, long? fromMonth, long? toMonth, long? levelId);
    List<long?> getCurrentUserLevelIds(long? userId);
    List<long?> findLevelIdsByUserIdAndPlanId(long? userId, long? planId);
}
