// Mechanical conversion from Java (controller/EvaluationFeedBackController.java). Complex bodies are marked with TODO.
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

/// <summary>معادل EvaluationFeedBackController.java — تزریق وابستگی‌ها با constructor injection انجام شده است.</summary>
public class EvaluationFeedBackController
{
    // TODO(signature): public ResponseEntity<?> createEvaluationFeedBack(@RequestHeader Map<String,String> headers , evaluationFeedBackService.create(evaluationFeedBackDto); return ne
    // TODO(signature): public ResponseEntity<?> getAllByLevelIdAndEvaluationPeriod(@RequestHeader Map<String,String> headers , List<EvaluationFeedBackDto> result = evaluationFeedBackS
    // TODO(signature): public ResponseEntity<?> editEvaluationFeedBack(@RequestHeader Map<String,String> headers , evaluationFeedBackService.edit(evaluationFeedBackDto); return new Re
    // TODO(signature): public ResponseEntity<?> deleteEvaluationFeedBack(@RequestHeader Map<String,String> headers , evaluationFeedBackService.delete(evaluationFeedBackId); return new
    // @PreAuthorize("@semSecurityUtilityService.checkAccess(#headers.get('requesturi'))")
    [HttpGet("evaluationFeedBack/findById/{id}")]
    IActionResult findById()
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — findById
        throw new NotImplementedException();
    }
    // @PreAuthorize("@semSecurityUtilityService.checkAccess(#headers.get('requesturi'))")
    [HttpPut("evaluationFeedBack/editConfirmStatus/{id}")]
    IActionResult editConfirmStatus()
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — editConfirmStatus
        throw new NotImplementedException();
    }
}
