using global::SemBackDotNet10.measureDomain.controller.dto.measureActPlanDtos;
// Mechanical conversion from Java (strategy/measurePeriodHandler/PeriodicStrategy.java). Complex bodies are marked with TODO.
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



namespace SemBackDotNet10.measureDomain.strategy.measurePeriodHandler;

/// <summary>معادل PeriodicStrategy.java — تزریق وابستگی‌ها با constructor injection انجام شده است.</summary>
public class PeriodicStrategy : IValueAssignmentStrategy
{
    private readonly MeasureActPlanRepository _measureActPlanRepository;

    public PeriodicStrategy(MeasureActPlanRepository measureActPlanRepository)
    {
        _measureActPlanRepository = measureActPlanRepository;
    }

    void applyPlan(List<MeasureActPlan> plans, List<MeasurePeriodCalculationDto> dtos, string? tenant)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — tenant)
        throw new NotImplementedException();
    }
    void applyReplan(List<MeasureActPlan> plans, List<MeasurePeriodCalculationDto> dtos, string? tenant)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — tenant)
        throw new NotImplementedException();
    }
    // TODO(signature): ) { int currentMonth = getCurrentJalaliMonth(); Map<Long, MeasurePeriodCalculationDto> dtoMap = dtos.stream().collect(Collectors.toMap( MeasurePeriodCalculation
    int getCurrentJalaliMonth()
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — getCurrentJalaliMonth
        throw new NotImplementedException();
    }
    decimal? valueOrNull(decimal? value, decimal? dailyValue)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — dailyValue)
        throw new NotImplementedException();
    }
}
