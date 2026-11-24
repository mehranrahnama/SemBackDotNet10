using SearchDto = global::SemBackDotNet10.Base.model.dto.SearchDto;
// Mechanical conversion from Java (service/impl/PCNCalculationFormulaServiceImpl.java). Complex bodies are marked with TODO.
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

/// <summary>معادل PCNCalculationFormulaServiceImpl.java — تزریق وابستگی‌ها با constructor injection انجام شده است.</summary>
public class PCNCalculationFormulaServiceImpl : IPCNCalculationFormulaService
{
    private readonly PCNCalculationFormulaRepository _pCNCalculationFormulaRepository;
    private readonly MeasureActPlanCalcRepository _measureActPlanCalcRepository;
    private readonly IMeasureActPlanService _measureActPlanService;
    private readonly global::SemBackDotNet10.basicDomain.service.ITenantService _tenantService;

    public PCNCalculationFormulaServiceImpl(PCNCalculationFormulaRepository pCNCalculationFormulaRepository, MeasureActPlanCalcRepository measureActPlanCalcRepository, IMeasureActPlanService measureActPlanService, global::SemBackDotNet10.basicDomain.service.ITenantService tenantService)
    {
        _pCNCalculationFormulaRepository = pCNCalculationFormulaRepository;
        _measureActPlanCalcRepository = measureActPlanCalcRepository;
        _measureActPlanService = measureActPlanService;
        _tenantService = tenantService;
    }

    public PCNCalculationFormulaRepository GetRepository() => _pCNCalculationFormulaRepository;

    public PCNCalculationFormulaRepository getRepository() => GetRepository();
    // @Transactional(rollbackFor = Throwable.class)
    public PCNCalculationFormula create(PCNCalculationFormulaDto pCNCalculationFormulaDto)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — pCNCalculationFormulaDto)
        throw new NotImplementedException();
    }
    public PCNCalculationFormulaDto mapToDto(PCNCalculationFormula pCNCalculationFormula)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — pCNCalculationFormula)
        throw new NotImplementedException();
    }
    public PCNCalculationFormula mapToEntity(PCNCalculationFormulaDto pCNCalculationFormulaDto)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — pCNCalculationFormulaDto)
        throw new NotImplementedException();
    }
    public SearchDto findAll(string? filter, int page, int size, string? orderBy, SortDirection direction)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — direction)
        throw new NotImplementedException();
    }
    public MeasureActPlan calculatePCNAchievedValue(MeasureActPlan measureActPlan)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureActPlan)
        throw new NotImplementedException();
    }
    public void calculatePCNForAllMeasureActPlans(string? filter)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — filter)
        throw new NotImplementedException();
    }
}
