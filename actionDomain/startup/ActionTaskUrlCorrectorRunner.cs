// Mechanical conversion from Java (startup/ActionTaskUrlCorrectorRunner.java). Complex bodies are marked with TODO.
// وابستگی‌های اعلام‌شده: basicDomain/Common/base هنوز ارسال نشده‌اند.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SemBackDotNet10.actionDomain.model.entity;
using SemBackDotNet10.actionDomain.model.dto;
using SemBackDotNet10.actionDomain.controller.dto;
using SemBackDotNet10.actionDomain.mapper;
using SemBackDotNet10.actionDomain.repository;
using SemBackDotNet10.actionDomain.service.interf;
using SemBackDotNet10.actionDomain.domain;
using SemBackDotNet10.actionDomain.strategy.confirmStatusHandler.context;
using SemBackDotNet10.actionDomain.strategy.confirmStatusHandler.factory;
using SemBackDotNet10.actionDomain.strategy.confirmStatusHandler.handler;


namespace SemBackDotNet10.actionDomain.startup;

/// <summary>معادل ActionTaskUrlCorrectorRunner.java — ApplicationRunner جاوا در C# به IHostedService تبدیل می‌شود.</summary>
public class ActionTaskUrlCorrectorRunner : IHostedService
{
    private readonly IActionActPlanService _actionActPlanService;
    private readonly IActionPlanCatalogService _actionPlanCatalogService;
    private readonly IActionPlanService _actionPlanService;

    public ActionTaskUrlCorrectorRunner(IActionActPlanService actionActPlanService, IActionPlanCatalogService actionPlanCatalogService, IActionPlanService actionPlanService)
    {
        _actionActPlanService = actionActPlanService;
        _actionPlanCatalogService = actionPlanCatalogService;
        _actionPlanService = actionPlanService;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var plans = await _actionActPlanService.FindAllAsync(x => x.TaskUrl != null, cancellationToken);
        foreach (var plan in plans)
            plan.TaskUrl = CorrectUrl(plan.TaskUrl!, plan.Id);
        await _actionActPlanService.SaveAllAsync(plans, cancellationToken);

        var catalogs = await _actionPlanCatalogService.FindAllAsync(x => x.TaskUrl != null, cancellationToken);
        foreach (var catalog in catalogs)
            catalog.TaskUrl = CorrectUrl(catalog.TaskUrl!, catalog.Id);
        await _actionPlanCatalogService.SaveAllAsync(catalogs, cancellationToken);

        var plannings = await _actionPlanService.FindAllAsync(x => x.TaskUrl != null, cancellationToken);
        foreach (var planning in plannings)
            planning.TaskUrl = CorrectUrl(planning.TaskUrl!, planning.Id);
        await _actionPlanService.SaveAllAsync(plannings, cancellationToken);
    }

    private static string CorrectUrl(string url, long? id) =>
        System.Text.RegularExpressions.Regex.Replace(url, "entityId=[0-9]+", _ => $"entityId={id}");
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
