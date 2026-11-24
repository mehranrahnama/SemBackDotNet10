// Mechanical conversion from Java (service/impl/ActionPlanActivityProgressServiceImpl.java). Complex bodies are marked with TODO.
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

using SemBackDotNet10.SecurityDomain.Model.Entity;
using Common.logging;


namespace SemBackDotNet10.actionDomain.service.impl;

/// <summary>معادل ActionPlanActivityProgressServiceImpl.java — تزریق وابستگی‌ها با constructor injection انجام شده است.</summary>
public class ActionPlanActivityProgressServiceImpl : IActionPlanActivityProgressService
{
    private readonly ActionPlanActivityProgressRepository _actionPlanActivityProgressRepository;
    private readonly IPlanPeriodService _planPeriodService;
    private readonly SemLogger _semLogger;
    private readonly global::SemBackDotNet10.SecurityDomain.Service.Interf.IUserRoleService _userRoleService;
    private readonly IActionActPlanService _actionActPlanService;
    private readonly ActionActPlanRepository _actionActPlanRepository;
    private readonly IActionPlanActivityProgressMapper _actionPlanActivityProgressMapper;

    public ActionPlanActivityProgressServiceImpl(ActionPlanActivityProgressRepository actionPlanActivityProgressRepository, IPlanPeriodService planPeriodService, SemLogger semLogger, global::SemBackDotNet10.SecurityDomain.Service.Interf.IUserRoleService userRoleService, IActionActPlanService actionActPlanService, ActionActPlanRepository actionActPlanRepository, IActionPlanActivityProgressMapper actionPlanActivityProgressMapper)
    {
        _actionPlanActivityProgressRepository = actionPlanActivityProgressRepository;
        _planPeriodService = planPeriodService;
        _semLogger = semLogger;
        _userRoleService = userRoleService;
        _actionActPlanService = actionActPlanService;
        _actionActPlanRepository = actionActPlanRepository;
        _actionPlanActivityProgressMapper = actionPlanActivityProgressMapper;
    }

    // TODO(ctor): سازنده‌ی جاوا (جز تزریق وابستگی) منتقل نشده است.
    // TODO(signature): IActionPlanActivityProgressMapper actionPlanActivityProgressMapper ) { this.actionPlanActivityProgressRepository = actionPlanActivityProgressRepository; this.pla
    //service functions
    public ActionPlanActivityProgressRepository GetRepository() => _actionPlanActivityProgressRepository;

    public ActionPlanActivityProgressRepository getRepository() => GetRepository();
    public void createNewActivityProgress(List<ActionPlanActivity> actionPlanActivities)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — actionPlanActivities)
        throw new NotImplementedException();
    }
    // @Transactional
    public void updateActivityProgressItems(List<ActionPlanActivity> activities)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — activities)
        throw new NotImplementedException();
    }
    public void deleteActivityProgress(List<long?> actionPlanActivityList)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — actionPlanActivityList)
        throw new NotImplementedException();
    }
    // TODO(signature): ) { semLogger.info("Enter the method"); if (tenantName.equals("SANGAN")) { String mappedOrder = actionActPlanService.mapOrderByToQueryOrderBy(orderBy); Pageable
    // TODO(signature): ) { semLogger.info("Enter the method"); if (tenantName.equals("SANGAN")) { for (UpdateActivityProgressRequestDto progress : actionPlanActivityList) { checkEnter
    // TODO(signature): Long baseCatalogId ) { if (baseCatalogId == null) throw new BusinessException("شناسه کاتالوگ اقدام کمیته فرزند نمیتواند خالی باشد!"); if (createdActivities.isEm
    // @Transactional(propagation = Propagation.MANDATORY)
    public void deleteAllActivityProgressesByActionPlanCatalogId(long? catalogId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — catalogId)
        throw new NotImplementedException();
    }
    void calculateActionActPlanBasedOnProgress(UpdateActivityProgressRequestDto progress)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — progress)
        throw new NotImplementedException();
    }
    // TODO(signature): ActionPlanActivity actionPlanActivity ) { List<ActionPlanActivityProgress> actionPlanActivityProgressList = new ArrayList<>(); for (int[] month : months) { Acti
    bool checkIfUserIsAdministrator(List<Role> roles)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — roles)
        throw new NotImplementedException();
    }
    // TODO(signature): List<Long> removalActivities ) { String[] startDate = actionPlanActivity.getDateStart().split("/"); String[] endDate = actionPlanActivity.getDateEnd().split("/"
    void checkEnteredValueIsValid(decimal? actualValue)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — actualValue)
        throw new NotImplementedException();
    }
}
