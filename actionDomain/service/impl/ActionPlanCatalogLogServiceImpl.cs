// Mechanical conversion from Java (service/impl/ActionPlanCatalogLogServiceImpl.java). Complex bodies are marked with TODO.
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



namespace SemBackDotNet10.actionDomain.service.impl;

/// <summary>معادل ActionPlanCatalogLogServiceImpl.java — تزریق وابستگی‌ها با constructor injection انجام شده است.</summary>
public class ActionPlanCatalogLogServiceImpl(ActionPlanCatalogLogRepository repository) : IActionPlanCatalogLogService
{
    public ActionPlanCatalogLogRepository GetRepository() => repository;
    public ActionPlanCatalogLogRepository getRepository() => GetRepository();
    public ActionPlanCatalogLog createActionPlanCatalogLog(ActionPlan actionPlan, long? newPayeshgarId, long? newModirId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — newModirId)
        throw new NotImplementedException();
    }
    public ActionPlanCatalogLog createActionPlanCatalogLog(ActionPlanCatalog actionPlanCatalog, long? newPayeshgarId, long? newModirId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — newModirId)
        throw new NotImplementedException();
    }
}
