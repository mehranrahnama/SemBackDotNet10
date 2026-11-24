// Mechanical conversion from Java (startup/MeasureTaskUrlCorrectorRunner.java). Complex bodies are marked with TODO.
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

/// <summary>معادل MeasureTaskUrlCorrectorRunner.java — ApplicationRunner جاوا در C# به IHostedService تبدیل می‌شود.</summary>
public class MeasureTaskUrlCorrectorRunner : IHostedService
{
    private readonly IMeasureService _measureService;
    private readonly IMeasureActPlanService _measureActPlanService;
    private readonly IMeasureCatalogService _measureCatalogService;

    public MeasureTaskUrlCorrectorRunner(IMeasureService measureService, IMeasureActPlanService measureActPlanService, IMeasureCatalogService measureCatalogService)
    {
        _measureService = measureService;
        _measureActPlanService = measureActPlanService;
        _measureCatalogService = measureCatalogService;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var plans = await _measureActPlanService.FindAllAsync(x => x.TaskUrl != null, cancellationToken);
        foreach (var plan in plans)
            plan.TaskUrl = CorrectUrl(plan.TaskUrl!, plan.Id);
        await _measureActPlanService.SaveAllAsync(plans, cancellationToken);

        var catalogs = await _measureCatalogService.FindAllAsync(x => x.TaskUrl != null, cancellationToken);
        foreach (var catalog in catalogs)
            catalog.TaskUrl = CorrectUrl(catalog.TaskUrl!, catalog.Id);
        await _measureCatalogService.SaveAllAsync(catalogs, cancellationToken);

        var measures = await _measureService.FindAllAsync(x => x.TaskUrl != null, cancellationToken);
        foreach (var measure in measures)
            measure.TaskUrl = CorrectUrl(measure.TaskUrl!, measure.Id);
        await _measureService.SaveAllAsync(measures, cancellationToken);
    }

    private static string CorrectUrl(string url, long? id) =>
        System.Text.RegularExpressions.Regex.Replace(url, "entityId=[0-9]+", _ => $"entityId={id}");
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
