// Mechanical conversion from Java (strategy/measurePeriodHandler/ValueAssignmentStrategyFactory.java). Complex bodies are marked with TODO.
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

/// <summary>معادل ValueAssignmentStrategyFactory.java — تزریق وابستگی‌ها با constructor injection انجام شده است.</summary>
public class ValueAssignmentStrategyFactory
{
    private readonly AggregationStrategy _aggregationStrategy;
    private readonly AverageStrategy _averageStrategy;
    private readonly PeriodicStrategy _periodicStrategy;

    public ValueAssignmentStrategyFactory(AggregationStrategy aggregationStrategy, AverageStrategy averageStrategy, PeriodicStrategy periodicStrategy)
    {
        _aggregationStrategy = aggregationStrategy;
        _averageStrategy = averageStrategy;
        _periodicStrategy = periodicStrategy;
    }

    IValueAssignmentStrategy of(int type)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — type)
        throw new NotImplementedException();
    }
}
