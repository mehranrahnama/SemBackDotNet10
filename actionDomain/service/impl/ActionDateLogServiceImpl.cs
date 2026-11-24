// Mechanical conversion from Java (service/impl/ActionDateLogServiceImpl.java). Complex bodies are marked with TODO.
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

/// <summary>معادل ActionDateLogServiceImpl.java — تزریق وابستگی‌ها با constructor injection انجام شده است.</summary>
public class ActionDateLogServiceImpl : IActionDateLogService
{
    private readonly ActionDateLogRepository _actionDateLogRepository;

    public ActionDateLogServiceImpl(ActionDateLogRepository actionDateLogRepository)
    {
        _actionDateLogRepository = actionDateLogRepository;
    }

    public ActionDateLogRepository GetRepository()
        => _actionDateLogRepository;
    public ActionDateLogRepository getRepository() => GetRepository();
    public ActionDateLog? findByActionPlanCatalogOrElseThrow(ActionPlanCatalog actionPlanCatalog)
    {
        // Despite its name, Java returns the repository result without a not-found check.
        try
        {
            return _actionDateLogRepository
                .findFirstByActionPlanCatalogOrderByModifiedDateDesc(actionPlanCatalog);
        }
        catch (Exception error)
        {
            throw new SemBackDotNet10.Common.exception.BusinessException(error.Message);
        }
    }
    public ActionDateLog? getOldestEndDateByActionPlanCatalogId(long? actionPlanCatalogId)
        => _actionDateLogRepository
            .findFirstByActionPlanCatalogIdOrderByModifiedDateAsc(actionPlanCatalogId);
}
