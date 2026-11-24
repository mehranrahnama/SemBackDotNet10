using global::SemBackDotNet10.Base.Service.Inter;
// Mechanical conversion from Java (service/interf/IActionPlanActivityService.java). Complex bodies are marked with TODO.
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

/// <summary>معادل IActionPlanActivityService.java (اینترفیس جاوا) — در C# با پیشوند I.</summary>
public interface IActionPlanActivityService : IBaseService<ActionPlanActivity,long?,ActionPlanActivityRepository>
{
    List<ActionPlanActivity> edit(long? actionPlanId, List<ActionPlanActivity> activities);
    List<ActionPlanActivity> findByActPlanCatalogId(long? id);
    // TODO(signature): );
    void addActivityFromProgress(long? actionActPlanId, ActionPlanActivity activity);
}
