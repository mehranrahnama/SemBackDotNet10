// Mechanical conversion from Java (mapper/IActionPlanMapper.java). Complex bodies are marked with TODO.
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



namespace SemBackDotNet10.actionDomain.mapper;

/// <summary>معادل IActionPlanMapper.java (اینترفیس جاوا) — در C# با پیشوند I.</summary>
public interface IActionPlanMapper
{
    // TODO(signature): );
    // TODO(signature): );
    // TODO(signature): StrategyObject strategyObject );
    // @Mapping(target = "id", ignore = true)
    // @Mapping(target = "taskUrl", source = "source.taskUrl")
    // @Mapping(target = "planingLevel", ignore = true)
    // @Mapping(target = "modir", ignore = true)
    // @Mapping(target = "payeshgar", ignore = true)
    // @Mapping(target = "strategyObject", ignore = true)
    // @Mapping(target = "actionPlanCatalog", ignore = true)
    // @Mapping(target = "planPeriod", ignore = true)
    // @Mapping(target = "weight", ignore = true)
    ActionPlan setTaskUrl(ActionPlan target, ActionPlan source);
    // TODO(signature): ) { if (source.getTaskUrl() == null || target.getId() == null) { return; } target.setTaskUrl( source.getTaskUrl() .replaceAll("entityId=\\d+", "entityId=" + tar
}
