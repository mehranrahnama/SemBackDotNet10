using SearchDto = global::SemBackDotNet10.Base.model.dto.SearchDto;
// Mechanical conversion from Java (service/impl/EvaluationMeasureServiceImpl.java). Complex bodies are marked with TODO.
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

/// <summary>معادل EvaluationMeasureServiceImpl.java — تزریق وابستگی‌ها با constructor injection انجام شده است.</summary>
public class EvaluationMeasureServiceImpl : IEvaluationMeasureService
{
    private readonly global::Common.service.interf.CommentService _commentService;
    private readonly global::Common.service.interf.HistoryService _historyService;
    private readonly global::SemBackDotNet10.evaluationDomain.repository.IEvaluationMeasureRepository _evaluationMeasureRepository;
    private readonly IEvaluationTeamHeaderLevelService _evaluationTeamHeaderLevelService;
    private readonly UserRolesHelpers _userRolesHelpers;

    public EvaluationMeasureServiceImpl(global::Common.service.interf.CommentService commentService, global::Common.service.interf.HistoryService historyService, global::SemBackDotNet10.evaluationDomain.repository.IEvaluationMeasureRepository evaluationMeasureRepository, IEvaluationTeamHeaderLevelService evaluationTeamHeaderLevelService, UserRolesHelpers userRolesHelpers)
    {
        _commentService = commentService;
        _historyService = historyService;
        _evaluationMeasureRepository = evaluationMeasureRepository;
        _evaluationTeamHeaderLevelService = evaluationTeamHeaderLevelService;
        _userRolesHelpers = userRolesHelpers;
    }

    public CommentRepository GetCommentRepository() => _commentService.GetRepository();
    public CommentRepository getCommentRepository() => GetCommentRepository();
    public HistoryRepository GetHistoryRepository() => _historyService.GetRepository();
    public HistoryRepository getHistoryRepository() => GetHistoryRepository();
    public global::SemBackDotNet10.evaluationDomain.repository.IEvaluationMeasureRepository GetRepository() => _evaluationMeasureRepository;
    public global::SemBackDotNet10.evaluationDomain.repository.IEvaluationMeasureRepository getRepository() => GetRepository();
    public EvaluationMeasure findByIdOrElseThrow(long? id)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — id)
        throw new NotImplementedException();
    }
    public SearchDto loadGridEvaluationMeasure(string? body, int page, int size, string? orderBy, SortDirection direction)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — direction)
        throw new NotImplementedException();
    }
    public void insertDataToEvaluationMeasure(string? body)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — body)
        throw new NotImplementedException();
    }
    SearchDto findAllEvaluationMeasure(long? planId, long? fromMonth, long? toMonth, string? filter, int page, int size, string? orderBy, SortDirection direction)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — direction)
        throw new NotImplementedException();
    }
    SearchDto findAllEvaluationMeasureForMasterRater(long? planId, long? fromMonth, long? toMonth, long? currentUserId, string? filter, int page, int size, string? orderBy, SortDirection direction)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — direction)
        throw new NotImplementedException();
    }
    public SearchDto findAllEvaluationMeasureDetails(string? body, int page, int size, string? orderBy, SortDirection direction)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — direction)
        throw new NotImplementedException();
    }
    public EvaluationMeasureDto updateEvaluationMeasure(EvaluationMeasureDto evaluationMeasureDto)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — evaluationMeasureDto)
        throw new NotImplementedException();
    }
    public void editConfirmStatus(EvaluationMeasureDto evaluationMeasureDto)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — evaluationMeasureDto)
        throw new NotImplementedException();
    }
    public EvaluationMeasureDto getEvaluationMeasureById(long? evaluationMeasureId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — evaluationMeasureId)
        throw new NotImplementedException();
    }
    void calculateAssessmentPCN(EvaluationMeasure evaluationMeasure)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — evaluationMeasure)
        throw new NotImplementedException();
    }
    public void findAndDeleteEvaluationMeasure(long? measureActPlanId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureActPlanId)
        throw new NotImplementedException();
    }
    public void updateDisabledStatus(List<EvaluationMeasureDto> evaluationMeasureDtoList)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — evaluationMeasureDtoList)
        throw new NotImplementedException();
    }
    // @Transactional(propagation = Propagation.MANDATORY)
    public void deleteEvaluationsForMeasureActPlanIds(List<long?> measureActPlanIds)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureActPlanIds)
        throw new NotImplementedException();
    }
}
