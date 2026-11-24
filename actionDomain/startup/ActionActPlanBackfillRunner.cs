// Mechanical conversion from Java (startup/ActionActPlanBackfillRunner.java). Complex bodies are marked with TODO.
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

/// <summary>معادل ActionActPlanBackfillRunner.java — ApplicationRunner جاوا در C# به IHostedService تبدیل می‌شود.</summary>
public class ActionActPlanBackfillRunner : IHostedService
{
    private readonly IActionActPlanService _actionActPlanService;
    private readonly IActionPlanCatalogService _actionPlanCatalogService;

    public ActionActPlanBackfillRunner(IActionActPlanService actionActPlanService, IActionPlanCatalogService actionPlanCatalogService)
    {
        _actionActPlanService = actionActPlanService;
        _actionPlanCatalogService = actionPlanCatalogService;
    }

    // @Transactional
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var catalogs = _actionPlanCatalogService.findPlannedCatalogsWithCorruptedPlanningState();
        foreach (var catalog in catalogs)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _actionActPlanService.createActionActPlan(catalog);
            catalog.IsPlanned = false;
            catalog.IsReplanned = false;
        }

        await _actionPlanCatalogService.SaveAllAsync(catalogs, cancellationToken);
    }
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
