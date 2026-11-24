// Mechanical conversion from Java (startup/MeasureActualValueCalculatorRunner.java). Complex bodies are marked with TODO.
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



namespace SemBackDotNet10.measureDomain.startup;

/// <summary>معادل MeasureActualValueCalculatorRunner.java — ApplicationRunner جاوا در C# به IHostedService تبدیل می‌شود.</summary>
public class MeasureActualValueCalculatorRunner : IHostedService
{
    private readonly IMeasureActPlanService _measureActPlanService;
    private readonly ILogger<MeasureActualValueCalculatorRunner> _logger;

    public MeasureActualValueCalculatorRunner(IMeasureActPlanService measureActPlanService, ILogger<MeasureActualValueCalculatorRunner> logger)
    {
        _measureActPlanService = measureActPlanService;
        _logger = logger;
    }

    // @Transactional
    public Task StartAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var measures = _measureActPlanService.findAllWithNullActualValue();
        var failures = 0;
        foreach (var measure in measures)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                var id = measure.Key ?? throw new InvalidOperationException("Measure ID is missing.");
                _measureActPlanService.calculateValuesForProgresses(new Dictionary<long?, byte> { [id] = measure.Value });
            }
            catch (Exception exception) when (!cancellationToken.IsCancellationRequested)
            {
                failures++;
                _logger.LogError(exception, "Calculating actual values failed for measure {MeasureId} (type {MeasureType}).", measure.Key, measure.Value);
            }
        }

        _logger.LogInformation("Actual-value calculation finished: {Updated} updated, {Failed} failed.", measures.Count - failures, failures);
        return Task.CompletedTask;
    }
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
