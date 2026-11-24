using EvaluationMeasureService = global::SemBackDotNet10.evaluationDomain.service.interf.IEvaluationMeasureService;
using Measure = global::SemBackDotNet10.measureDomain.model.entity.Measure;
using MeasureRepository = global::SemBackDotNet10.measureDomain.model.entity.MeasureRepository;
using global::SemBackDotNet10.dashboardDomain.repository;
using SemLogger = global::Common.logging.SemLogger;
// Mechanical conversion from Java (service/impl/MeasureDeletionService.java). Complex bodies are marked with TODO.
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

/// <summary>معادل MeasureDeletionService.java — تزریق وابستگی‌ها با constructor injection انجام شده است.</summary>
public class MeasureDeletionService
{
    private readonly global::SemBackDotNet10.basicDomain.service.IUserService _userService;
    private readonly IMeasureCatalogRelationService _measureCatalogRelationService;
    private readonly MeasureCatalogRelationRepository _measureCatalogRelationRepository;
    private readonly MeasureRepository _measureRepository;
    private readonly MeasureActPlanRepository _measureActPlanRepository;
    private readonly MeasureReplanLogRepository _measureReplanLogRepository;
    private readonly CartableStatusService _cartableStatusService;
    private readonly IMeasureActPlanLogService _measureActPlanLogService;
    private readonly EvaluationMeasureService _evaluationMeasureService;
    private readonly SemLogger _semLogger;
    private readonly MeasureDomainService _measureDomainService;
    private readonly DashboardRepository _dashboardRepository;

    public MeasureDeletionService(global::SemBackDotNet10.basicDomain.service.IUserService userService, IMeasureCatalogRelationService measureCatalogRelationService, MeasureCatalogRelationRepository measureCatalogRelationRepository, MeasureRepository measureRepository, MeasureActPlanRepository measureActPlanRepository, MeasureReplanLogRepository measureReplanLogRepository, CartableStatusService cartableStatusService, IMeasureActPlanLogService measureActPlanLogService, EvaluationMeasureService evaluationMeasureService, SemLogger semLogger, MeasureDomainService measureDomainService, DashboardRepository dashboardRepository)
    {
        _userService = userService;
        _measureCatalogRelationService = measureCatalogRelationService;
        _measureCatalogRelationRepository = measureCatalogRelationRepository;
        _measureRepository = measureRepository;
        _measureActPlanRepository = measureActPlanRepository;
        _measureReplanLogRepository = measureReplanLogRepository;
        _cartableStatusService = cartableStatusService;
        _measureActPlanLogService = measureActPlanLogService;
        _evaluationMeasureService = evaluationMeasureService;
        _semLogger = semLogger;
        _measureDomainService = measureDomainService;
        _dashboardRepository = dashboardRepository;
    }

    // TODO(ctor): سازنده‌ی جاوا (جز تزریق وابستگی) منتقل نشده است.
    // TODO(signature): DashboardRepository dashboardRepository) { this.userService = userService; this.measureCatalogRelationService = measureCatalogRelationService; this.measureCatal
    void deleteMeasureRelationsAndPropagate(Measure measure)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measure)
        throw new NotImplementedException();
    }
    void softDeleteMeasure(Measure measure)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measure)
        throw new NotImplementedException();
    }
    void softDeleteMeasureDirectly(Measure measure)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measure)
        throw new NotImplementedException();
    }
    void removePreviousDeletedMeasure(Measure measure)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measure)
        throw new NotImplementedException();
    }
    CartableStatusPair findCartableStatusPairForDeletedMeasure(long? measureId, long? confirmStatus)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — confirmStatus)
        throw new NotImplementedException();
    }
    // @Transactional
    void findAndDeletePassiveMeasureActPlanLogs(Measure measure)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measure)
        throw new NotImplementedException();
    }
    // @Transactional
    void findAndDeletePassiveMeasureActPlans(Measure measure)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measure)
        throw new NotImplementedException();
    }
    void findAndPassiveMeasureActPlans(Measure measure, CartableStatusPair cartableStatusPair)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — cartableStatusPair)
        throw new NotImplementedException();
    }
    void sendMeasureToConfirmDelete(Measure measure, CartableStatusPair cartableStatusPair)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — cartableStatusPair)
        throw new NotImplementedException();
    }
}
