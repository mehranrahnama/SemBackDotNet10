// Mechanical conversion from Java (mapper/IActionPlanCatalogMapper.java). Complex bodies are marked with TODO.
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

/// <summary>معادل IActionPlanCatalogMapper.java (اینترفیس جاوا) — در C# با پیشوند I.</summary>
public interface IActionPlanCatalogMapper
{
    // TODO(signature): }) }) ActionPlanCatalog clone(ActionPlanCatalog base, PlaningLevel level);
    // TODO(signature): default boolean calculateAddToSupreme(PlaningLevel level) { return level != null && level.getParent() != null && level.getParent().getParent() != null; }
    // @Mapping(target = "id", ignore = true)
    // @Mapping(target = "planingLevel", ignore = true)
    // @Mapping(target = "planned", source = "base.planned")
    // @Mapping(target = "description", source = "base.description")
    // @Mapping(target = "replannedStatus", source = "base.replannedStatus")
    // @Mapping(target = "replanned", source = "base.replanned")
    // @Mapping(target = "startDate", source = "base.startDate")
    // @Mapping(target = "endDate", source = "base.endDate")
    // @Mapping(target = "totalBudget", source = "base.totalBudget")
    // @Mapping(target = "confirmstatus", source = "base.confirmstatus")
    void copyValues(ActionPlanCatalog catalog, ActionPlanCatalog baseCatalog);
    // @Mapping(target = "id", ignore = true)
    // @Mapping(target = "taskUrl", source = "source.taskUrl")
    // @Mapping(target = "planingLevel", ignore = true)
    // @Mapping(target = "payeshgar", ignore = true)
    // @Mapping(target = "modir", ignore = true)
    ActionPlanCatalog setTaskUrl(ActionPlanCatalog target, ActionPlanCatalog source);
    // TODO(signature): ) { if (source.getTaskUrl() == null || target.getId() == null) { return; } target.setTaskUrl( source.getTaskUrl() .replaceAll("entityId=\\d+", "entityId=" + tar
}
