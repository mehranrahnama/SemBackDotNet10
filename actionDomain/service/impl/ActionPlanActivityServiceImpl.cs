// Mechanical conversion from Java (service/impl/ActionPlanActivityServiceImpl.java). Complex bodies are marked with TODO.
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

/// <summary>معادل ActionPlanActivityServiceImpl.java — تزریق وابستگی‌ها با constructor injection انجام شده است.</summary>
public class ActionPlanActivityServiceImpl : IActionPlanActivityService
{
    private readonly ActionPlanActivityRepository _actionPlanActivityRepository;
    private readonly IActionPlanActivityMapper _actionPlanActivityMapper;
    private readonly ActionPlanCatalogRepository _actionPlanCatalogRepository;
    private readonly IActionPlanActivityProgressService _actionPlanActivityProgressService;

    public ActionPlanActivityServiceImpl(ActionPlanActivityRepository actionPlanActivityRepository, IActionPlanActivityMapper actionPlanActivityMapper, ActionPlanCatalogRepository actionPlanCatalogRepository, IActionPlanActivityProgressService actionPlanActivityProgressService)
    {
        _actionPlanActivityRepository = actionPlanActivityRepository;
        _actionPlanActivityMapper = actionPlanActivityMapper;
        _actionPlanCatalogRepository = actionPlanCatalogRepository;
        _actionPlanActivityProgressService = actionPlanActivityProgressService;
    }

    // TODO(ctor): سازنده‌ی جاوا (جز تزریق وابستگی) منتقل نشده است.
    // TODO(signature): IActionPlanActivityProgressService actionPlanActivityProgressService ) { this.actionPlanActivityRepository = actionPlanActivityRepository; this.actionPlanActivit
    public ActionPlanActivityRepository GetRepository() => _actionPlanActivityRepository;

    public ActionPlanActivityRepository getRepository() => GetRepository();
    //    @Override
    //    public List<ActionPlanActivityDto> getActivityDtoListById(List<Long> activityIds) {
    //        semLogger.info("Enter the method");
    //        try {
    //        List<ActionPlanActivityDto> result = actionPlanActivityRepository.getActivityDtoListById(activityIds);
    //        semLogger.info("Exit method");
    //        return result;
    //    } catch (Exception e) {
    //            semLogger.error("throw exception: " + e.getMessage());
    //            throw new BusinessException(e.getMessage());
    //        }
    //    }
    public List<ActionPlanActivity> edit(long? actionPlanId, List<ActionPlanActivity> activities)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — activities)
        throw new NotImplementedException();
    }
    public List<ActionPlanActivity> findByActPlanCatalogId(long? actPlanCatalogId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — actPlanCatalogId)
        throw new NotImplementedException();
    }
    // @Transactional(rollbackFor = Exception.class)
    public void addActivityFromProgress(long? actionActPlanId, ActionPlanActivity activity)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — activity)
        throw new NotImplementedException();
    }
    void checkDateInRange(ActionPlanActivity activity, string? catalogStartDate, string? catalogEndDate)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — catalogEndDate)
        throw new NotImplementedException();
    }
    // yyyy/MM/dd -> yyyyMMdd so dates with or without zero padding compare correctly
    int toDateNumber(string? date)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — date)
        throw new NotImplementedException();
    }
    // TODO(signature): ) { if (baseCatalogId == null) throw new BusinessException("شناسه کاتالوگ اقدام کمیته فرزند نمیتواند خالی باشد!"); boolean skip = actionPlanCatalogs == null || 
    ActionPlanActivity updateFoundActivity(ActionPlanActivity foundActivity, ActionPlanActivity activity)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — activity)
        throw new NotImplementedException();
    }
}
