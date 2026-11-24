using SearchDto = global::SemBackDotNet10.Base.model.dto.SearchDto;
// Mechanical conversion from Java (service/impl/EvaluationActionServiceImpl.java). Complex bodies are marked with TODO.
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
using global::Common.model.entity;
using SemBackDotNet10.basicDomain.Model.Entity;
using SemBackDotNet10.basicDomain.Model.Dto;
using SemBackDotNet10.baseDomain.repository;
using SemBackDotNet10.basicDomain.service;
using SemBackDotNet10.actionDomain.model.entity;
using global::Common.repository;
using global::Common.service.interf;
using global::Common.model.dto;
using global::Common.controller.dto;
using SemBackDotNet10.Common.enums;
using SemBackDotNet10.Common.exception;
using global::Common.messages;
using global::Common.repository;
using global::Common.service.interf;
using global::Common.service.impl;
using global::Common.logging;
using global::Common.model.dto;
using global::Common.utils.cartable;
using global::Common.utils.security;
using global::Common.utils.userRoles;


namespace SemBackDotNet10.evaluationDomain.service.impl;

/// <summary>معادل EvaluationActionServiceImpl.java — تزریق وابستگی‌ها با constructor injection انجام شده است.</summary>
public class EvaluationActionServiceImpl(global::Common.service.interf.CommentService commentService, global::Common.service.interf.HistoryService historyService, global::SemBackDotNet10.evaluationDomain.repository.IEvaluationActionRepository evaluationActionRepository) : IEvaluationActionService
{
    public CommentRepository GetCommentRepository() => commentService.GetRepository();
    public CommentRepository getCommentRepository() => GetCommentRepository();
    public HistoryRepository GetHistoryRepository() => historyService.GetRepository();
    public HistoryRepository getHistoryRepository() => GetHistoryRepository();
    public global::SemBackDotNet10.evaluationDomain.repository.IEvaluationActionRepository GetRepository() => evaluationActionRepository;
    public global::SemBackDotNet10.evaluationDomain.repository.IEvaluationActionRepository getRepository() => GetRepository();
    public EvaluationAction findByIdOrElseThrow(long? id)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — id)
        throw new NotImplementedException();
    }
    public SearchDto loadGridEvaluationAction(string? body, int page, int size, string? orderBy, SortDirection direction)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — direction)
        throw new NotImplementedException();
    }
    SearchDto findAllEvaluationAction(long? planId, long? fromMonth, long? toMonth, int page, int size, string? orderBy, SortDirection direction)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — direction)
        throw new NotImplementedException();
    }
    SearchDto findAllEvaluationActionForMasterRater(long? planId, long? fromMonth, long? toMonth, long? masterRater, int page, int size, string? orderBy, SortDirection direction)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — direction)
        throw new NotImplementedException();
    }
    public void insertDataToEvaluationAction(string? body)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — body)
        throw new NotImplementedException();
    }
    public SearchDto findAllEvaluationActionDetails(string? body, int page, int size, string? orderBy, SortDirection direction)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — direction)
        throw new NotImplementedException();
    }
    public EvaluationActionDto updateEvaluationAction(EvaluationActionDto evaluationActionDto)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — evaluationActionDto)
        throw new NotImplementedException();
    }
    public void editConfirmStatus(EvaluationActionDto evaluationActionDto)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — evaluationActionDto)
        throw new NotImplementedException();
    }
    public EvaluationActionDto getEvaluationActionById(long? evaluationActionId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — evaluationActionId)
        throw new NotImplementedException();
    }
    void calculateAssessmentPCN(EvaluationAction evaluationAction)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — evaluationAction)
        throw new NotImplementedException();
    }
    public void findAndDeleteEvaluationAction(List<ActionActPlan> actionActPlans)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — actionActPlans)
        throw new NotImplementedException();
    }
    public void updateDisabledStatus(List<EvaluationActionDto> evaluationActionDtoList)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — evaluationActionDtoList)
        throw new NotImplementedException();
    }
    public void deleteEvaluationActionByCatalogId(long? actionPlanCatalogId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — actionPlanCatalogId)
        throw new NotImplementedException();
    }
}
