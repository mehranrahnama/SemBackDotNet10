using SemLogger = global::Common.logging.SemLogger;
// Mechanical conversion from Java (service/impl/MeasureActPlanLogServiceImpl.java). Complex bodies are marked with TODO.
// وابستگی‌های اعلام‌شده: basicDomain/Common/base هنوز ارسال نشده‌اند.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SemBackDotNet10.measureDomain.model.entity;
using SemBackDotNet10.measureDomain.model.dto;
using SemBackDotNet10.measureDomain.controller.dto;
using SemBackDotNet10.measureDomain.mapper;
using SemBackDotNet10.measureDomain.repository;
using SemBackDotNet10.measureDomain.service.interf;
using SemBackDotNet10.measureDomain.domain;
using SemBackDotNet10.measureDomain.strategy.confirmStatusHandler.context;
using SemBackDotNet10.measureDomain.strategy.confirmStatusHandler.factory;
using SemBackDotNet10.measureDomain.strategy.confirmStatusHandler.handler;
using global::Common.model.entity;
using SemBackDotNet10.basicDomain.Model.Entity;
using SemBackDotNet10.baseDomain.repository;
using SemBackDotNet10.basicDomain.service;
using SemBackDotNet10.SecurityDomain.Model.Entity;
using SemBackDotNet10.SecurityDomain.Repository;
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


namespace SemBackDotNet10.measureDomain.service.impl;

/// <summary>معادل MeasureActPlanLogServiceImpl.java — تزریق وابستگی‌ها با constructor injection انجام شده است.</summary>
public class MeasureActPlanLogServiceImpl : IMeasureActPlanLogService
{
    private readonly MeasureActPlanLogRepository _measureActPlanLogRepository;
    private readonly CommentService _commentService;
    private readonly HistoryService _historyService;
    private readonly SemLogger _semLogger;
    private readonly IMeasureActPlanLogMapper _measureActPlanLogMapper;

    public MeasureActPlanLogServiceImpl(MeasureActPlanLogRepository measureActPlanLogRepository, CommentService commentService, HistoryService historyService, SemLogger semLogger, IMeasureActPlanLogMapper measureActPlanLogMapper)
    {
        _measureActPlanLogRepository = measureActPlanLogRepository;
        _commentService = commentService;
        _historyService = historyService;
        _semLogger = semLogger;
        _measureActPlanLogMapper = measureActPlanLogMapper;
    }

    public CommentRepository GetCommentRepository() => _commentService.GetRepository();
    public HistoryRepository GetHistoryRepository() => _historyService.GetRepository();
    public MeasureActPlanLogRepository GetRepository() => _measureActPlanLogRepository;

    public MeasureActPlanLogRepository getRepository() => GetRepository();
    // TODO(signature): ) { semLogger.info("Enter the method"); Optional<List<MeasureActPlanLog>> logs = measureActPlanLogRepository.findByMeasureIdAndNoNewValueSorted( measureId ); if
    public void createLogWithEmptyNewValues(List<MeasureActPlan> measureActPlans)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureActPlans)
        throw new NotImplementedException();
    }
    public void deleteRecentLogsByMeasureId(long? measureId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureId)
        throw new NotImplementedException();
    }
}
