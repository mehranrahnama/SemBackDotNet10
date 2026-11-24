// Mechanical conversion from Java (service/impl/EvaluationFeedBackServiceImpl.java). Complex bodies are marked with TODO.
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

/// <summary>معادل EvaluationFeedBackServiceImpl.java — تزریق وابستگی‌ها با constructor injection انجام شده است.</summary>
public class EvaluationFeedBackServiceImpl : IEvaluationFeedBackService
{
    private readonly CartableStatusService _cartableStatusService;
    private readonly global::SemBackDotNet10.evaluationDomain.repository.IEvaluationFeedBackRepository _evaluationFeedBackRepository;
    private readonly global::SemBackDotNet10.evaluationDomain.repository.IEvaluationTeamHeaderLevelRepository _evaluationTeamHeaderLevelRepository;
    private readonly SemLogger _semLogger;
    private readonly IEvaluationFeedBackQuestionService _evaluationFeedBackQuestionService;
    private readonly UserRolesHelpers _userRolesHelpers;

    public EvaluationFeedBackServiceImpl(CartableStatusService cartableStatusService, global::SemBackDotNet10.evaluationDomain.repository.IEvaluationFeedBackRepository evaluationFeedBackRepository, global::SemBackDotNet10.evaluationDomain.repository.IEvaluationTeamHeaderLevelRepository evaluationTeamHeaderLevelRepository, SemLogger semLogger, IEvaluationFeedBackQuestionService evaluationFeedBackQuestionService, UserRolesHelpers userRolesHelpers)
    {
        _cartableStatusService = cartableStatusService;
        _evaluationFeedBackRepository = evaluationFeedBackRepository;
        _evaluationTeamHeaderLevelRepository = evaluationTeamHeaderLevelRepository;
        _semLogger = semLogger;
        _evaluationFeedBackQuestionService = evaluationFeedBackQuestionService;
        _userRolesHelpers = userRolesHelpers;
    }

    public global::SemBackDotNet10.evaluationDomain.repository.IEvaluationFeedBackRepository GetRepository() => _evaluationFeedBackRepository;
    public global::SemBackDotNet10.evaluationDomain.repository.IEvaluationFeedBackRepository getRepository() => GetRepository();
    public void create(EvaluationFeedBackDto evaluationFeedBackDto)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — evaluationFeedBackDto)
        throw new NotImplementedException();
    }
    EvaluationFeedBackDto mapToDto(EvaluationFeedBack evaluationFeedBack)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — evaluationFeedBack)
        throw new NotImplementedException();
    }
    public List<EvaluationFeedBackDto> getAllByLevelIdAndEvaluationPeriod(EvaluationFeedBackDto evaluationFeedBackDto)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — evaluationFeedBackDto)
        throw new NotImplementedException();
    }
    public void edit(EvaluationFeedBackDto evaluationFeedBackDto)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — evaluationFeedBackDto)
        throw new NotImplementedException();
    }
    public EvaluationFeedBack findByIdOrElseThrow(long? id)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — id)
        throw new NotImplementedException();
    }
    public void delete(long? evaluationFeedBackId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — evaluationFeedBackId)
        throw new NotImplementedException();
    }
    public EvaluationFeedBackDto findEvaluationFeedbackById(long? id)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — id)
        throw new NotImplementedException();
    }
    // @Transactional
    public void editConfirmStatus(string? outCome, long? id)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — id)
        throw new NotImplementedException();
    }
    string? generateEvaluationFeedbackTaskUrl(long? oldStatus, long? newStatus, long? id)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — id)
        throw new NotImplementedException();
    }
}
