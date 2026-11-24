// Mechanical conversion from Java (controller/EvaluationFeedBackQuestionController.java). Complex bodies are marked with TODO.
// وابستگی‌های اعلام‌شده: basicDomain/Common/base هنوز ارسال نشده‌اند.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SemBackDotNet10.evaluationDomain.model.entity;
using SemBackDotNet10.evaluationDomain.model.dto;
using SemBackDotNet10.evaluationDomain.repository;
using SemBackDotNet10.evaluationDomain.service.interf;



namespace SemBackDotNet10.evaluationDomain.controller;

/// <summary>معادل EvaluationFeedBackQuestionController.java — تزریق وابستگی‌ها با constructor injection انجام شده است.</summary>
public class EvaluationFeedBackQuestionController
{
    // TODO(signature): public ResponseEntity<?> getAllEvaluationFeedBackQuestion(@RequestHeader Map<String,String> headers , SearchDto allQuestion = evaluationFeedBackQuestionService.
    // TODO(signature): public ResponseEntity<?> createEvaluationFeedBackQuestion(@RequestHeader Map<String, String> headers, evaluationFeedBackQuestionService.createEvaluationFeedBack
    // @PreAuthorize("@semSecurityUtilityService.checkAccess(#headers.get('requesturi'))")
    [HttpPost("EvaluationFeedBackQuestion/saveQuestionWeights")]
    IActionResult saveQuestionWeights(Dictionary<string?,string?> headers, List<EvaluationFeedBackQuestionDto> dto)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — dto)
        throw new NotImplementedException();
    }
    // TODO(signature): public ResponseEntity<?> editEvaluationFeedBackQuestion(@RequestHeader Map<String, String> headers, evaluationFeedBackQuestionService.editEvaluationFeedBackQues
    // TODO(signature): public ResponseEntity<?> deleteEvaluationFeedBackQuestion(@RequestHeader Map<String, String> headers, evaluationFeedBackQuestionService.deleteEvaluationFeedBack
}
