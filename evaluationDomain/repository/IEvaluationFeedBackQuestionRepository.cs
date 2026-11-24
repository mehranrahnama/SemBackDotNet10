using global::SemBackDotNet10.Common.Pagination;
// Mechanical conversion from Java (repository/EvaluationFeedBackQuestionRepository.java).
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

/// <summary>معادل EvaluationFeedBackQuestionRepository.java (اینترفیس جاوا) — در C# با پیشوند I.</summary>
public interface IEvaluationFeedBackQuestionRepository : ISemRepository<EvaluationFeedBackQuestion,long?>
{
    PagedResult<EvaluationFeedBackQuestionDto> getAllEvaluationFeedBackQuestion(Pageable pageable, string? filter);
}
