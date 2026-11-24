// Mechanical conversion from Java (service/impl/ActionActPlanServiceImpl.java). Complex bodies are marked with TODO.
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
using SemBackDotNet10.basicDomain.Repositories;
using SemBackDotNet10.basicDomain.service;
using SemBackDotNet10.SecurityDomain.Service.Interf;
using SemBackDotNet10.SecurityDomain.Tenancy;
using SemBackDotNet10.Common.exception;
using SemBackDotNet10.Base.model.dto;
using SemBackDotNet10.Common.enums;
using SortDirection = global::SemBackDotNet10.Common.enums.SortDirection;
using SemBackDotNet10.actionDomain.persistence;
using SemBackDotNet10.actionDomain.controller.dto.actionActPlanDtos;
using SemBackDotNet10.service.impl;
using Common.repository;
using Common.service.interf;


namespace SemBackDotNet10.actionDomain.service.impl;

/// <summary>معادل ActionActPlanServiceImpl.java — تزریق وابستگی‌ها با constructor injection انجام شده است.</summary>
public partial class ActionActPlanServiceImpl : IActionActPlanService
{
    private readonly ActionActPlanRepository _actionActPlanRepository;
    private readonly IActionPlanCatalogService _actionPlanCatalogService;
    private readonly IPlanPeriodService _planPeriodService;
    private readonly IActionActPlanLogService _actionActPlanLogService;
    private readonly IUserService _userService;
    private readonly IActionPlanService _actionPlanService;
    private readonly IUserRoleService _userRoleService;
    private readonly CommentRepository _commentRepository;
    private readonly HistoryRepository _historyRepository;
    private readonly IActionDateLogService _actionDateLogService;
    private readonly ActionPlanCatalogRepository _actionPlanCatalogRepository;
    private readonly ActionDateLogRepository _actionDateLogRepository;
    private readonly IActionActPlanMapper _actionActPlanMapper;
    private readonly IActionPlanCatalogRelationService _actionPlanCatalogRelationService;
    private readonly ICorrectionActionsByEMSService _correctionActionsByEMSService;
    private readonly ActionActPlanDomainService _actionActPlanDomainService;
    private readonly ActionPlanDomainService _actionPlanDomainService;
    private readonly ActionConfirmStatusHandlerFactory _actionConfirmStatusHandlerFactory;
    private readonly ActionPlanCatalogRelationDomainService _actionPlanCatalogRelationDomainService;
    private readonly ActionPlanCatalogDomainService _actionPlanCatalogDomainService;
    private readonly ActionPlanActivityRepository _actionPlanActivityRepository;
    private readonly StoredAttachmentRepository _storedAttachmentRepository;
    private readonly StoredAttachmentService _storedAttachmentService;
    private readonly string? _expireDate;
    private readonly ActionPlanningUnitOfWork _planningUnitOfWork;
    private readonly ActionPlanningStrategyObjectReader _planningStrategyObjects;
    private readonly PlaningLevelUserServiceImpl _planningLevelUsers;
    private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;
    private readonly CartableStatusService _cartableStatusService;
    private readonly ICallActionRestApiService _callActionRestApiService;

    private readonly TenantContext _tenant;

    public ActionActPlanServiceImpl(TenantContext tenant, ActionActPlanRepository actionActPlanRepository, IActionPlanCatalogService actionPlanCatalogService, IPlanPeriodService planPeriodService, IActionActPlanLogService actionActPlanLogService, IUserService userService, IActionPlanService actionPlanService, IUserRoleService userRoleService, IActionDateLogService actionDateLogService, ActionPlanCatalogRepository actionPlanCatalogRepository, ActionDateLogRepository actionDateLogRepository, IActionActPlanMapper actionActPlanMapper, IActionPlanCatalogRelationService actionPlanCatalogRelationService, ICorrectionActionsByEMSService correctionActionsByEMSService, ActionActPlanDomainService actionActPlanDomainService, ActionPlanDomainService actionPlanDomainService, ActionConfirmStatusHandlerFactory actionConfirmStatusHandlerFactory, ActionPlanCatalogRelationDomainService actionPlanCatalogRelationDomainService, ActionPlanCatalogDomainService actionPlanCatalogDomainService, ActionPlanActivityRepository actionPlanActivityRepository, StoredAttachmentRepository storedAttachmentRepository, StoredAttachmentService storedAttachmentService, string? expireDate, CommentRepository commentRepository, HistoryRepository historyRepository, ActionPlanningUnitOfWork planningUnitOfWork, ActionPlanningStrategyObjectReader planningStrategyObjects, PlaningLevelUserServiceImpl planningLevelUsers, Microsoft.Extensions.Configuration.IConfiguration configuration, CartableStatusService cartableStatusService, ICallActionRestApiService callActionRestApiService)
    {
        _tenant = tenant;
        _actionActPlanRepository = actionActPlanRepository;
        _actionPlanCatalogService = actionPlanCatalogService;
        _planPeriodService = planPeriodService;
        _actionActPlanLogService = actionActPlanLogService;
        _userService = userService;
        _actionPlanService = actionPlanService;
        _userRoleService = userRoleService;
        _actionDateLogService = actionDateLogService;
        _actionPlanCatalogRepository = actionPlanCatalogRepository;
        _actionDateLogRepository = actionDateLogRepository;
        _actionActPlanMapper = actionActPlanMapper;
        _actionPlanCatalogRelationService = actionPlanCatalogRelationService;
        _correctionActionsByEMSService = correctionActionsByEMSService;
        _actionActPlanDomainService = actionActPlanDomainService;
        _actionPlanDomainService = actionPlanDomainService;
        _actionConfirmStatusHandlerFactory = actionConfirmStatusHandlerFactory;
        _actionPlanCatalogRelationDomainService = actionPlanCatalogRelationDomainService;
        _actionPlanCatalogDomainService = actionPlanCatalogDomainService;
        _actionPlanActivityRepository = actionPlanActivityRepository;
        _storedAttachmentRepository = storedAttachmentRepository;
        _storedAttachmentService = storedAttachmentService;
        _expireDate = expireDate;
        _commentRepository = commentRepository;
        _historyRepository = historyRepository;
        _planningUnitOfWork = planningUnitOfWork;
        _planningStrategyObjects = planningStrategyObjects;
        _planningLevelUsers = planningLevelUsers;
        _configuration = configuration;
        _cartableStatusService = cartableStatusService;
        _callActionRestApiService = callActionRestApiService;
    }

    // TODO(ctor): سازنده‌ی جاوا (جز تزریق وابستگی) منتقل نشده است.
    // TODO(signature): ) { this.actionActPlanRepository = actionActPlanRepository; this.actionPlanCatalogService = actionPlanCatalogService; this.planPeriodService = planPeriodService
    public CommentRepository GetCommentRepository() => _commentRepository;
    public HistoryRepository GetHistoryRepository() => _historyRepository;
    public CommentRepository getCommentRepository() => GetCommentRepository();
    public HistoryRepository getHistoryRepository() => GetHistoryRepository();

    public ActionActPlan findByIdOrElseThrow(long? id)
    {
        if (id is null)
            throw new BusinessException("error.actionActPlan.idIsNotValid");
        return _actionActPlanRepository.FindById(id)
            ?? throw new BusinessException("error.actionActPlan.idIsNotValid");
    }
    public ActionActPlanRepository GetRepository() => _actionActPlanRepository;

    // Preserve translated internal callers until their naming is standardized.
    public ActionActPlanRepository getRepository() => GetRepository();
    // @Transactional
    public void createActionActPlan(ActionPlanCatalog actionPlanCatalog)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — actionPlanCatalog)
        throw new NotImplementedException();
    }
    // @Transactional
    public void updatePlanValues(List<UpdateActionActPlanDto> dtos, long? actionPlanCatalogId, long? strategyObjectId)
    {
        _planningUnitOfWork.Execute(() =>
        {
            validateActionActPlanValues(dtos);
            _actionActPlanRepository.StagePlanValues(dtos);
            // Java flush occurs before the calculation and outside its catch block.
            // SaveChanges flushes here; the enclosing transaction is not committed.
            _actionActPlanRepository.CommitChanges();
            var yearPcnMap = calculateYearPcnFromPlanValues(actionPlanCatalogId);

            try
            {
                applyYearPcn(actionPlanCatalogId, yearPcnMap);
                _actionPlanCatalogService.updatePlanned(true, actionPlanCatalogId);
                var planningLevelId = _actionPlanCatalogService
                    .findCatalogPlanningLevelIdById(actionPlanCatalogId);
                var strategyIds = _planningStrategyObjects
                    .findStrategyObjectByTitleAndLevelId(strategyObjectId, planningLevelId);
                foreach (var id in strategyIds)
                    _actionPlanService.updateConfirmStatusAndTaskUrlAfterPlanning(actionPlanCatalogId, id);

                // Deliberately retain this additional call even when the source ID
                // already occurs in strategyIds, matching Java's call order.
                _actionPlanService.updateConfirmStatusAndTaskUrlAfterPlanning(
                    actionPlanCatalogId, strategyObjectId);

                if (_actionPlanService.isAddToSupremeLevel(actionPlanCatalogId))
                {
                    var ancestors = _actionPlanCatalogRelationService.findAncestorEntity(actionPlanCatalogId);
                    copyActionActPlansForUpperLevels(actionPlanCatalogId,
                        ancestors.Select(x => (long?)x.Id).ToList());
                    foreach (var catalog in ancestors)
                    {
                        applyYearPcn(catalog.Id, yearPcnMap);
                        _actionPlanCatalogService.updatePlanned(true, catalog.Id);
                        var upperStrategyIds = _planningStrategyObjects.findStrategyObjectByTitleAndLevelId(
                            strategyObjectId, catalog.PlaningLevel!.Id);
                        foreach (var id in upperStrategyIds)
                            _actionPlanService.updateConfirmStatusAndTaskUrlAfterPlanning(catalog.Id, id);
                    }
                }
            }
            catch (BusinessException)
            {
                throw;
            }
            catch (Exception error)
            {
                throw new BusinessException(error.Message);
            }
        });
    }
    //helper
    void validateActionActPlanValues(List<UpdateActionActPlanDto> dtos)
    {
        if (dtos is null || dtos.Count == 0)
            throw new BusinessException("لیست برنامه‌ریزی خالی است.");

        if (dtos.Any(dto => dto.PlanValue is null))
            throw new BusinessException("برخی از مقادیر برنامه‌ریزی خالی هستند. لطفاً همه مقادیر را وارد کنید.");
    }
    // @Transactional
    public void updateActionActPlan(ActionPlanCatalog actionPlanCatalog)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — actionPlanCatalog)
        throw new NotImplementedException();
    }
    // TODO(signature): List<int[]> monthBetween ) { List<ActionActPlan> actionActPlanSet = new ArrayList<>(); for (int[] month : monthBetween) { ActionActPlan actionActPlanTemplate = 
    // @Transactional(rollbackFor = Exception.class)
    SearchDto CreateAndGetDataActionActPlan(string? body, int page, int size, string? orderBy, SortDirection direction)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — direction)
        throw new NotImplementedException();
    }
    // @Transactional
    public void calculationDefaultPlanValue(string? body)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — body)
        throw new NotImplementedException();
    }
    public SearchDto findAllActionActPlan(string? body, int page, int size, string? orderBy, SortDirection direction)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — direction)
        throw new NotImplementedException();
    }
    //    @Transactional(rollbackFor = Exception.class)
    // تغییر برنامه ریزی اقدام replan
    // @Transactional(rollbackFor = Exception.class)
    public SearchDto CreateOrGetDataActionActPlan(string? body, int page, int size, string? orderBy, SortDirection direction)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — direction)
        throw new NotImplementedException();
    }
    void createActionActPlan(ActionPlanCatalog actionPlanCatalog, ActionPlan actionPlan, long? actionActPlanConfirmStatus, string? outCome, string? dateEnd, long? startPlanPeriod)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — startPlanPeriod)
        throw new NotImplementedException();
    }
    // @Transactional(rollbackFor = Throwable.class)
    // @Transactional(rollbackFor = Exception.class)
    // @Transactional(rollbackFor = Exception.class)
    // @Transactional
    // @Transactional
    // @Transactional
    // @Transactional(rollbackFor = Exception.class)
    // @Transactional(rollbackFor = Exception.class)
    // @Value("${action.user}")
    // @Value("${action.pass}")
    // @Transactional(rollbackFor = Exception.class)
    // @Transactional(rollbackFor = Exception.class)
    // @Transactional(propagation = Propagation.MANDATORY)
}
