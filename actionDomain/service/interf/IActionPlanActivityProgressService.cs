using global::SemBackDotNet10.Base.Service.Inter;
// Mechanical conversion from Java (service/interf/IActionPlanActivityProgressService.java). Complex bodies are marked with TODO.
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
using global::Common.model.entity;
using global::Common.model.dto;
using global::Common.controller.dto;
using SemBackDotNet10.Common.enums;
using SemBackDotNet10.Common.exception;
using global::Common.messages;
using global::Common.repository;
using global::Common.service.interf;
using global::Common.service.impl;


namespace SemBackDotNet10.actionDomain.service.interf;

/// <summary>معادل IActionPlanActivityProgressService.java (اینترفیس جاوا) — در C# با پیشوند I.</summary>
public interface IActionPlanActivityProgressService : IBaseService<ActionPlanActivityProgress,long?,ActionPlanActivityProgressRepository>
{
    void createNewActivityProgress(List<ActionPlanActivity> actionPlanActivityList);
    void updateActivityProgressItems(List<ActionPlanActivity> activities);
    void deleteActivityProgress(List<long?> actionPlanActivityId);
    // TODO(signature): );
    // TODO(signature): );
    // TODO(signature): Long baseCatalogId );
    void deleteAllActivityProgressesByActionPlanCatalogId(long? id);
}
