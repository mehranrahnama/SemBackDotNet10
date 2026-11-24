using EntityManager = global::SemBackDotNet10.Data.SemDbContext;
using Measure = global::SemBackDotNet10.measureDomain.model.entity.Measure;
using MeasureRepository = global::SemBackDotNet10.measureDomain.model.entity.MeasureRepository;
using SemLogger = global::Common.logging.SemLogger;
// Mechanical conversion from Java (service/impl/MeasureReplanLogServiceImpl.java). Complex bodies are marked with TODO.
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
using SemBackDotNet10.measureDomain.controller.dto.measureActPlanDtos;
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

/// <summary>معادل MeasureReplanLogServiceImpl.java — تزریق وابستگی‌ها با constructor injection انجام شده است.</summary>
public class MeasureReplanLogServiceImpl : IMeasureReplanLogService
{
    private readonly MeasureReplanLogRepository _measureReplanLogRepository;
    private readonly EntityManager _entityManager;
    private readonly IMeasureReplanLogMapper _measureReplanLogMapper;
    private readonly CommentService _commentService;
    private readonly HistoryService _historyService;
    private readonly MeasureActPlanRepository _measureActPlanRepository;
    private readonly MeasureActPlanLogRepository _measureActPlanLogRepository;
    private readonly SemLogger _semLogger;

    public MeasureReplanLogServiceImpl(MeasureReplanLogRepository measureReplanLogRepository, EntityManager entityManager, IMeasureReplanLogMapper measureReplanLogMapper, CommentService commentService, HistoryService historyService, MeasureActPlanRepository measureActPlanRepository, MeasureActPlanLogRepository measureActPlanLogRepository, SemLogger semLogger)
    {
        _measureReplanLogRepository = measureReplanLogRepository;
        _entityManager = entityManager;
        _measureReplanLogMapper = measureReplanLogMapper;
        _commentService = commentService;
        _historyService = historyService;
        _measureActPlanRepository = measureActPlanRepository;
        _measureActPlanLogRepository = measureActPlanLogRepository;
        _semLogger = semLogger;
    }

    public CommentRepository GetCommentRepository() => _commentService.GetRepository();
    public HistoryRepository GetHistoryRepository() => _historyService.GetRepository();
    public MeasureReplanLogRepository GetRepository() => _measureReplanLogRepository;

    public MeasureReplanLogRepository getRepository() => GetRepository();
    public void createOrUpdateMeasureReplanLog(Measure measure, MeasureCatalog measureCatalog, MeasureRepository measureRepository, int measurePeriod, int desiredMethod, int measurePayeshType, decimal? currentYearTarget)
    {
        _semLogger.Info("Enter the method");
        var hasExpiredPlans = _measureActPlanRepository.findAllByMeasureAndDeletedIsTrue(measure).Count > 0;
        var log = hasExpiredPlans
            ? findFirstByMeasureOrderByCreatedDate(measure)
            : new MeasureReplanLog
            {
                Measure = measure,
                MeasurePeriodOld = measureCatalog.MeasurePeriod,
                DesiredMethodOld = measureRepository.DesiredMethod,
                MeasurePayeshTypeOld = measureRepository.MeasurePayeshType
            };
        log.MeasurePeriodNew = measurePeriod;
        log.DesiredMethodNew = desiredMethod;
        log.MeasurePayeshTypeNew = unchecked((byte)measurePayeshType);
        log.CurrentYearTarget = hasExpiredPlans ? currentYearTarget : measure.CurrentYearTarget;
        _measureReplanLogRepository.SaveAsync(log).GetAwaiter().GetResult();
        _semLogger.Info("Exit method");
    }
    public MeasureReplanLog findFirstByMeasureOrderByCreatedDate(Measure measure)
    {
        _semLogger.Info("Enter the method");
        var result = _measureReplanLogRepository.findFirstByMeasureOrderByCreatedDate(measure);
        _semLogger.Info("Exit method");
        return result;
    }
    public void createMeasureReplanLogs(NewPlanningDto newPlanningDto, MeasureReplanLogCreationDto measureReplanLogCreationDto)
    {
        var measureId = newPlanningDto.MeasureId ?? throw new ArgumentException("MeasureId is required.", nameof(newPlanningDto));
        var measurePeriod = newPlanningDto.MeasurePeriod ?? throw new ArgumentException("MeasurePeriod is required.", nameof(newPlanningDto));
        var measure = _entityManager.Set<Measure>().Find(measureId)
            ?? throw new InvalidOperationException($"Measure {measureId} was not found.");
        var log = new MeasureReplanLog
        {
            Measure = measure,
            MeasurePeriodOld = measureReplanLogCreationDto.MeasurePeriod,
            MeasurePeriodNew = measurePeriod,
            DesiredMethodOld = measureReplanLogCreationDto.DesiredMethod,
            DesiredMethodNew = newPlanningDto.DesiredMethod,
            MeasurePayeshTypeOld = measureReplanLogCreationDto.MeasurePayeshType,
            MeasurePayeshTypeNew = newPlanningDto.MeasurePayeshType,
            CurrentYearTarget = measureReplanLogCreationDto.CurrentYearTarget
        };
        _measureReplanLogRepository.SaveAsync(log).GetAwaiter().GetResult();
    }
    public MeasureReplanLog getLatestReplanLog(long? measureId)
    {
        return _measureReplanLogRepository.findFirstByMeasureIdOrderByCreatedDateAsc(measureId);
    }
    public bool existsPreviousReplanLog(long? measureId, long? logId)
    {
        // SQL comparisons with NULL do not match any row.
        return measureId.HasValue && logId.HasValue &&
            _measureActPlanLogRepository.measureId(measureId).Any(log => log.Id.HasValue && log.Id != logId);
    }
}
