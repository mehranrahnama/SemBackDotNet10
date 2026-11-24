using global::SemBackDotNet10.webserviceDomain.domain.dto;
using global::SemBackDotNet10.Common.Pagination;
using global::Common.model.dto;
using global::Common.repository;
using global::Common.service.interf;
using global::SemBackDotNet10.dashboardDomain.repository;
using SearchDto = global::SemBackDotNet10.Base.model.dto.SearchDto;
using SemLogger = global::Common.logging.SemLogger;
using UserRolesHelpers = global::Common.utils.userRoles.UserRolesHelpers;
// Mechanical conversion from Java (service/impl/ActionPlanServiceImpl.java). Complex bodies are marked with TODO.
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
using SemBackDotNet10.service.impl;
using SemBackDotNet10.basicDomain.service;
using SemBackDotNet10.SecurityDomain.Service.Interf;
using CartableStatusServiceImpl = global::Common.service.impl.CartableStatusServiceImpl;
using static global::Common.utils.cartable.CartableStatusConstants;
using BusinessException = SemBackDotNet10.Common.exception.BusinessException;
using EvaluationActionService = global::SemBackDotNet10.evaluationDomain.service.interf.IEvaluationActionService;
using StrategyObjectService = global::SemBackDotNet10.basicDomain.service.IStrategyObjectService;
using PlaningLevelService = global::SemBackDotNet10.service.impl.IPlaningLevelService;
using PlaningLevelUserService = global::SemBackDotNet10.service.impl.PlaningLevelUserServiceImpl;
using DashboardRepository = global::SemBackDotNet10.dashboardDomain.repository.IDashboardRepository;
using SemSecurityUtilityService = global::SemBackDotNet10.SecurityDomain.Service.Impl.SemSecurityUtilityService;



namespace SemBackDotNet10.actionDomain.service.impl;

/// <summary>معادل ActionPlanServiceImpl.java — تزریق وابستگی‌ها با constructor injection انجام شده است.</summary>
public class ActionPlanServiceImpl : IActionPlanService
{
    private readonly ActionPlanRepository _actionPlanRepository;
    private readonly IActionPlanCatalogService _actionPlanCatalogService;
    private readonly global::SemBackDotNet10.basicDomain.service.IStrategyObjectService _strategyObjectService;
    private readonly IUserService _userService;
    private readonly IPlanPeriodService _planPeriodService;
    private readonly IPlaningLevelService _planingLevelService;
    private readonly IUserRoleService _userRoleService;
    private readonly PlaningLevelUserService _planingLevelUserService;
    private readonly IActionPlanActivityService _actionPlanActivityService;
    private readonly ActionPlanCatalogRepository _actionPlanCatalogRepository;
    private readonly IActionPlanActivityProgressService _actionPlanActivityProgressService;
    private readonly ActionPlanActivityRepository _actionPlanActivityRepository;
    private readonly IActionActPlanService _actionActPlanService;
    private readonly HistoryService _historyService;
    private readonly CommentService _commentService;
    private readonly CartableStatusServiceImpl _cartableStatusService;
    private readonly ActionActPlanRepository _actionActPlanRepository;
    private readonly global::SemBackDotNet10.basicDomain.service.ISettingService _settingService;
    private readonly SemSecurityUtilityService _semSecurityUtilityService;
    private readonly IActionPlanCoworkerService _actionPlanCoworkerService;
    private readonly IActionPlanBeneficiaryService _actionPlanBeneficiaryService;
    private readonly ActionPlanBeneficiaryRepository _actionPlanBeneficiaryRepository;
    private readonly ActionPlanCoworkerRepository _actionPlanCoworkerRepository;
    private readonly IActionPlanCatalogLogService _actionPlanCatalogLogService;
    private readonly UserRolesHelpers _userRolesHelpers;
    private readonly SemLogger _semLogger;
    private readonly EvaluationActionService _evaluationActionService;
    private readonly IActionPlanCatalogRelationService _actionPlanCatalogRelationService;
    private readonly IActionPlanMapper _actionPlanMapper;
    private readonly global::SemBackDotNet10.baseDomain.resolver.IStrategyObjectResolver _strategyObjectResolver;
    private readonly ActionConfirmStatusHandlerFactory _actionConfirmStatusHandlerFactory;
    private readonly ActionPlanDomainService _actionPlanDomainService;
    private readonly ActionPlanCatalogRelationDomainService _actionPlanCatalogRelationDomainService;
    private readonly ActionPlanCatalogDomainService _actionPlanCatalogDomainService;
    private readonly StoredAttachmentService _storedAttachmentService;
    private readonly DashboardRepository _dashboardRepository;
    private readonly ActionPlanCatalogRelationRepository _actionPlanCatalogRelationRepository;
    private readonly ActionPlanPlanningWriter _planningWriter;
    private readonly ActionTaskUrlBuilder _taskUrls;
    private readonly string? _expireDate;

    public ActionPlanServiceImpl(ActionPlanRepository actionPlanRepository, IActionPlanCatalogService actionPlanCatalogService, StrategyObjectService strategyObjectService, global::SemBackDotNet10.basicDomain.service.IUserService userService, IPlanPeriodService planPeriodService, PlaningLevelService planingLevelService, global::SemBackDotNet10.SecurityDomain.Service.Interf.IUserRoleService userRoleService, PlaningLevelUserService planingLevelUserService, IActionPlanActivityService actionPlanActivityService, ActionPlanCatalogRepository actionPlanCatalogRepository, IActionPlanActivityProgressService actionPlanActivityProgressService, ActionPlanActivityRepository actionPlanActivityRepository, IActionActPlanService actionActPlanService, HistoryService historyService, CommentService commentService, CartableStatusServiceImpl cartableStatusService, ActionActPlanRepository actionActPlanRepository, global::SemBackDotNet10.basicDomain.service.ISettingService settingService, SemSecurityUtilityService semSecurityUtilityService, IActionPlanCoworkerService actionPlanCoworkerService, IActionPlanBeneficiaryService actionPlanBeneficiaryService, ActionPlanBeneficiaryRepository actionPlanBeneficiaryRepository, ActionPlanCoworkerRepository actionPlanCoworkerRepository, IActionPlanCatalogLogService actionPlanCatalogLogService, UserRolesHelpers userRolesHelpers, SemLogger semLogger, EvaluationActionService evaluationActionService, IActionPlanCatalogRelationService actionPlanCatalogRelationService, IActionPlanMapper actionPlanMapper, global::SemBackDotNet10.baseDomain.resolver.IStrategyObjectResolver strategyObjectResolver, ActionConfirmStatusHandlerFactory actionConfirmStatusHandlerFactory, ActionPlanDomainService actionPlanDomainService, ActionPlanCatalogRelationDomainService actionPlanCatalogRelationDomainService, ActionPlanCatalogDomainService actionPlanCatalogDomainService, StoredAttachmentService storedAttachmentService, DashboardRepository dashboardRepository, ActionPlanCatalogRelationRepository actionPlanCatalogRelationRepository, string? expireDate, ActionPlanPlanningWriter planningWriter, ActionTaskUrlBuilder taskUrls, global::SemBackDotNet10.Data.SemDbContext db)
    {
        _db = db;
        _actionPlanRepository = actionPlanRepository;
        _actionPlanCatalogService = actionPlanCatalogService;
        _strategyObjectService = strategyObjectService;
        _userService = userService;
        _planPeriodService = planPeriodService;
        _planingLevelService = planingLevelService;
        _userRoleService = userRoleService;
        _planingLevelUserService = planingLevelUserService;
        _actionPlanActivityService = actionPlanActivityService;
        _actionPlanCatalogRepository = actionPlanCatalogRepository;
        _actionPlanActivityProgressService = actionPlanActivityProgressService;
        _actionPlanActivityRepository = actionPlanActivityRepository;
        _actionActPlanService = actionActPlanService;
        _historyService = historyService;
        _commentService = commentService;
        _cartableStatusService = cartableStatusService;
        _actionActPlanRepository = actionActPlanRepository;
        _settingService = settingService;
        _semSecurityUtilityService = semSecurityUtilityService;
        _actionPlanCoworkerService = actionPlanCoworkerService;
        _actionPlanBeneficiaryService = actionPlanBeneficiaryService;
        _actionPlanBeneficiaryRepository = actionPlanBeneficiaryRepository;
        _actionPlanCoworkerRepository = actionPlanCoworkerRepository;
        _actionPlanCatalogLogService = actionPlanCatalogLogService;
        _userRolesHelpers = userRolesHelpers;
        _semLogger = semLogger;
        _evaluationActionService = evaluationActionService;
        _actionPlanCatalogRelationService = actionPlanCatalogRelationService;
        _actionPlanMapper = actionPlanMapper;
        _strategyObjectResolver = strategyObjectResolver;
        _actionConfirmStatusHandlerFactory = actionConfirmStatusHandlerFactory;
        _actionPlanDomainService = actionPlanDomainService;
        _actionPlanCatalogRelationDomainService = actionPlanCatalogRelationDomainService;
        _actionPlanCatalogDomainService = actionPlanCatalogDomainService;
        _storedAttachmentService = storedAttachmentService;
        _dashboardRepository = dashboardRepository;
        _actionPlanCatalogRelationRepository = actionPlanCatalogRelationRepository;
        _expireDate = expireDate;
        _planningWriter = planningWriter;
        _taskUrls = taskUrls;
    }

    public CommentRepository GetCommentRepository() => _commentService.GetRepository();
    public HistoryRepository GetHistoryRepository() => _historyService.GetRepository();
    public ActionPlanRepository GetRepository() => _actionPlanRepository;

    public ActionPlanRepository getRepository() => GetRepository();
    // TODO(ctor): سازنده‌ی جاوا (جز تزریق وابستگی) منتقل نشده است.
    // TODO(signature): DashboardRepository dashboardRepository, ActionPlanCatalogRelationRepository actionPlanCatalogRelationRepository) { this.actionPlanRepository = actionPlanReposi
    SearchDto loadGrid(string? body, int page, int size, string? orderBy, SortDirection direction)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — direction)
        throw new NotImplementedException();
    }
    // TODO(signature): ) { planingLevelUserService.hasAccessInLevelId(body.levelId(), semSecurityUtilityService.getCurrentUserId()); Pageable pageable = PageUtils.getInstance(page, si
    // TODO(signature): ) { log.info("returning the actionPlan data based on the strategy object."); planingLevelUserService.hasAccessInLevelId(body.levelId(), semSecurityUtilityServic
    void setActivitiesForPage(PagedResult<ActionPlanDto> dataPage)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — dataPage)
        throw new NotImplementedException();
    }
    void setActivitiesForDto(ActionPlanDto actionPlanDto)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — actionPlanDto)
        throw new NotImplementedException();
    }
    void setIsPlannedForPage(PagedResult<ActionPlanDto> dataPage)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — dataPage)
        throw new NotImplementedException();
    }
    void setIsPlannedForDto(ActionPlanDto actionPlanDto)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — actionPlanDto)
        throw new NotImplementedException();
    }
    // TODO(signature): ) { create_V2(baseActionPlan, baseActivities, sendToUpperLevels ? UpperLevelsSendMode.ALL : UpperLevelsSendMode.NONE); }
    // TODO(signature): ) { semLogger.info("Enter the method"); validateUpperLevelsMode(sendMode); ActionPlanCatalog catalog = actionPlanCatalogService.findByIdOrElseThrow(baseActionPl
    void validateActionPlanExist(RequestActionPlanDto baseActionPlan)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — baseActionPlan)
        throw new NotImplementedException();
    }
    void setActionPlanStatus(ActionPlan actionPlanInstance)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — actionPlanInstance)
        throw new NotImplementedException();
    }
    List<ActionPlanActivity> setActivity(List<ActionPlanActivity> baseActivities, ActionPlanCatalog actionPlanCatalog)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — actionPlanCatalog)
        throw new NotImplementedException();
    }
    void addPayeshgarUserRole(long? payeshgarId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — payeshgarId)
        throw new NotImplementedException();
    }
    void addModirUserRole(long? modirId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — modirId)
        throw new NotImplementedException();
    }
    Dictionary<string?,string?> getActionPlanCatalogDates(RequestActionPlanDto actionPlan)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — actionPlan)
        throw new NotImplementedException();
    }
    // TODO(signature): ) { semLogger.info("Enter the method"); List<String> activityStartDateList = new ArrayList<>(); List<String> activityEndDateList = new ArrayList<>(); actionPlan
    // @Transactional
    public ActionPlan edit(RequestActionPlanDto actionPlan)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — actionPlan)
        throw new NotImplementedException();
    }
    // TODO(signature): ) throws Exception { semLogger.info("Enter the method"); validateUpperLevelsMode(sendMode); ActionPlan existingBeforeEdit = findByIdOrElseThrow(actionPlan.id())
    // @Transactional(propagation = Propagation.MANDATORY)
    void removeSentToUpperLevelActionPlan(ActionPlan actionPlan)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — actionPlan)
        throw new NotImplementedException();
    }
    // TODO(signature): boolean parentOnly ) { try { actionPlan.setSendToParentOnly(parentOnly); List<PlaningLevel> planingLevelList = parentOnly ? findBaseAndParentLevels(actionPlan.g
    /**
    * قابلیت "فقط پدر" فقط برای HOSCO فعال است.
    */
    bool isParentOnlyTenant()
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — isParentOnlyTenant
        throw new NotImplementedException();
    }
    /**
    * حالت "فقط پدر" فقط برای HOSCO مجاز است.
    */
    void validateUpperLevelsMode(UpperLevelsSendMode mode)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — mode)
        throw new NotImplementedException();
    }
    /**
    * فهرست سطوح برای ارسال به پدر: همان سطح پایه + والد مستقیمش.
    */
    List<PlaningLevel> findBaseAndParentLevels(PlaningLevel baseLevel)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — baseLevel)
        throw new NotImplementedException();
    }
    // TODO(signature): UpperLevelsSendMode mode ) { switch (mode) { case ALL -> sendToUpperPlanningLevels( actionPlan, activities, explicitUpperLevelStrategyObjectIdByObjectId, edited
    void removeUpperLevelsAboveParent(ActionPlan actionPlan)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — actionPlan)
        throw new NotImplementedException();
    }
    void deleteActionPlansDirectly(List<ActionPlan> actionPlans)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — actionPlans)
        throw new NotImplementedException();
    }
    // TODO(signature): ) { try { List<StrategyObject> baseStrategyObjects = fetchBaseStrategyObjects(baseCatalog); List<ActionPlanCatalog> upperCatalogs = excludeBaseCatalog(catalogs,
    Dictionary<string?,StrategyObject> resolveExplicitTargetsMap(Dictionary<long?,long?> explicitByObjectId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — explicitByObjectId)
        throw new NotImplementedException();
    }
    // TODO(signature): ) { return actionPlanRepository .findStrategyObjectByActionPlanCatalogId( baseCatalog.getId() ); }
    // TODO(signature): ActionPlanCatalog baseCatalog ) { List<ActionPlanCatalog> result = new ArrayList<>(catalogs); result.removeIf(c -> c.getId().equals(baseCatalog.getId())); retur
    // TODO(signature): ) { Map<Long, List<ResolvedStrategyObjectTarget>> resultByLevel = new HashMap<>(); for (ActionPlanCatalog catalog : upperCatalogs) { PlaningLevel level = catalo
    // TODO(signature): Long previousStrategyObjectId ) { List<ResolvedStrategyObjectTarget> resolved = new ArrayList<>(baseStrategyObjects.size()); log.info("editedObjectId={}, previo
    public bool isAddToSupremeLevel(long? actionPlanCatalogId)
        => _planningWriter.findAddToSupremeLevelByActionPlanCatalogIdAndStrategyObjectId(actionPlanCatalogId);
    public List<ActionPlan> findAllWithTaskUrlIsNotNull()
        => _actionPlanRepository.findAllWithTaskUrlIsNotNull();
    // TODO(signature): ) { int startYear = Integer.parseInt(startDate.substring(0, 4)); int newEndYear = Integer.parseInt(newEndDate.substring(0, 4)); if (newEndYear <= startYear) { r
    private readonly global::SemBackDotNet10.Data.SemDbContext _db;

    public List<ActionPlanWebServiceDto> findActionPlanForWebService(long? planId)
    {
        if (!planId.HasValue) return null!; // Java returns null for an empty result.

        var plans = _db.DomainActionPlans.Where(plan => plan.PlanPeriod != null && plan.PlanPeriod.PlanId == planId);
        var rows = _db.Set<ActionPlanCatalog>()
            .Where(catalog => catalog.PlaningLevel != null && plans.Any(plan => plan.ActionPlanCatalogId == catalog.Id))
            .Select(catalog => new
            {
                catalog.Id, catalog.Description, catalog.StartDate, catalog.EndDate,
                SupervisorId = catalog.Payeshgar == null ? (long?)null : catalog.Payeshgar.UserId,
                SupervisorName = (catalog.Payeshgar!.NameF ?? "") + " " + (catalog.Payeshgar.FamilyF ?? ""),
                ManagerId = catalog.Modir == null ? (long?)null : catalog.Modir.UserId,
                ManagerName = (catalog.Modir!.NameF ?? "") + " " + (catalog.Modir.FamilyF ?? ""),
                ChartName = catalog.PlaningLevel!.DesLevel,
                ChartCode = catalog.PlaningLevel.DesThreeChar,
                Weight = plans.Where(plan => plan.ActionPlanCatalogId == catalog.Id).Min(plan => plan.Weight)
            }).ToList();
        if (rows.Count == 0) return null!;

        var ids = rows.Select(row => row.Id).ToArray();
        var objectives = plans.Where(plan => plan.ActionPlanCatalogId.HasValue &&
                ids.Contains(plan.ActionPlanCatalogId.Value) && plan.StrategyObject != null)
            .Select(plan => new { CatalogId = plan.ActionPlanCatalogId!.Value, plan.StrategyObject!.Title })
            .Distinct().ToList().ToLookup(item => item.CatalogId, item => item.Title!);

        return rows.Select(row => new ActionPlanWebServiceDto(row.Id, row.Description, objectives[row.Id].ToList(),
            row.StartDate, row.EndDate, row.SupervisorId, row.SupervisorName, row.ManagerId, row.ManagerName,
            row.ChartName, row.ChartCode, convertWeight(row.Weight) ??
                throw new InvalidOperationException("An action plan catalog has no weight for this period."))).ToList();
    }

    private static int? convertWeight(float? weight) => weight switch
    {
        null => null,
        <= 20 => 1,
        <= 40 => 2,
        <= 60 => 3,
        <= 80 => 4,
        _ => 5
    };
    // TODO(signature): ) { if (base == null) throw new BusinessException("اتصال اقدام به هدف نمیتواند خالی باشد!"); base.setAddToSupremeLevel(true); try { if (catalogs == null || cata
    // TODO(signature): ) { List<ActionPlan> result = new ArrayList<>(); try { for (ActionPlanCatalog catalog : catalogs) { PlaningLevel level = catalog.getPlaningLevel(); List<Resolve
    // TODO(signature): ) { try { StrategyObject newTarget = resolvedTarget.newTarget(); Optional<ActionPlan> existing = actionPlanRepository.findByActionPlanCatalogIdAndStrategyObject
    // TODO(signature): ActionPlan base ) { try { actionPlanMapper.copySupremeLevelValues(base, existing); existing.setAddToSupremeLevel(true); actionPlanRepository.updateAddToSupremeL
    // TODO(signature): ) { try { PlanPeriod planPeriod = planPeriodService.findByPlanId(year); return actionPlanMapper.clone( base, catalog, planPeriod, strategyObject, level ); } cat
    // TODO(signature): List<ActionPlan> actionPlans ) { List<ActionPlan> saved = actionPlanRepository.saveAllAndFlush(actionPlans); saved.forEach(actionPlan -> actionPlanMapper.setTas
    List<ActionPlanActivity> checkForDeleteActivities(List<ActionPlanActivity> activities)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — activities)
        throw new NotImplementedException();
    }
    // @Transactional
    public void editConfirmStatus(long? id, string? outCome)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — outCome)
        throw new NotImplementedException();
    }
    // @Transactional
    public void editConfirmStatusForDelete(ActionPlan actionPlan)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — actionPlan)
        throw new NotImplementedException();
    }
    public void delete(long? id)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — id)
        throw new NotImplementedException();
    }
    // @Transactional
    public void editDeleteStatus(string? body)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — body)
        throw new NotImplementedException();
    }
    void deleteAllExpiredDataAndSoftDeleteActionActPlans(ActionPlan foundActionPlan)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — foundActionPlan)
        throw new NotImplementedException();
    }
    CartableStatusPair findStatusPairBasedOnTenantAndPlannedStatus(ActionPlan foundActionPlan)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — foundActionPlan)
        throw new NotImplementedException();
    }
    public List<long?> getUserLevelIds(long? userId)
    {
        if (userId is null) throw new ArgumentNullException(nameof(userId));
        return _planingLevelUserService.GetUserLevelIdsAsync(userId.Value).GetAwaiter().GetResult()
            .Select(id => (long?)id).Union(_actionPlanRepository.getCurrentUserLevelIds(userId)).ToList();
    }
    public void editAllWeights(List<ActionPlanDto> actionPlanDtos)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — actionPlanDtos)
        throw new NotImplementedException();
    }
    public ActionPlanDto mapToDto(ActionPlan actionPlan)
        => new() { Id = actionPlan.Id, ActionPlanCatalogId = actionPlan.ActionPlanCatalog!.Id };
    public ActionPlan findByIdOrElseThrow(long? id)
    {
        if (id is null) throw new BusinessException("error.actionPlan.idIsNotValid");
        return _actionPlanRepository.FindByIdAsync(id).GetAwaiter().GetResult()
            ?? throw new BusinessException("error.actionPlan.idIsNotValid");
    }
    public Dictionary<string?,object?> getCounts(string? body)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — body)
        throw new NotImplementedException();
    }
    public List<ActionPlan> findByCatalogId(long? catalogId)
    {
        return _actionPlanRepository.findByCatalogId(catalogId);
    }
    public ActionPlanDto findActionPlanById(long? id)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — id)
        throw new NotImplementedException();
    }
    void setActionActPlans(ActionPlanDto actionPlanDto)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — actionPlanDto)
        throw new NotImplementedException();
    }
    public long? CountActiveActions(long? planId)
    {
        return _actionPlanRepository.CountActiveActions(planId);
    }
    public long? CountActiveActionsByPlanIdLevelId(long? planId, long? levelId)
    {
        _planingLevelUserService.HasAccessInLevelIdAsync(levelId, _semSecurityUtilityService.GetCurrentUserId())
            .GetAwaiter().GetResult();
        return _actionPlanRepository.CountActiveActionsByPlanIdLevelId(planId, levelId);
    }
    public List<ActionPlan> findByActionPlanCatalogIdAndPlanId(long? actionPlanCatalogId, long? planId)
    {
        return _actionPlanRepository.findByActionPlanCatalogIdAndPlanId(actionPlanCatalogId, planId);
    }
    public List<ActionPlan> findByActionPlanCatalogIdAndPlanIAndDeletedIsFalse(long? actionPlanCatalogId, long? planId)
    {
        return _actionPlanRepository.findByActionPlanCatalogIdAndPlanIAndDeletedIsFalse(actionPlanCatalogId, planId);
    }
    public ActionPlan insertNewActionPlanFromPreviewsActionPlan(StrategyObject strategyObject, long? planId, ActionPlan actionPlan)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — actionPlan)
        throw new NotImplementedException();
    }
    public bool expireActionPlanTasks(long? tenantId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — tenantId)
        throw new NotImplementedException();
    }
    void setNextStep(List<ActionPlan> expired)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — expired)
        throw new NotImplementedException();
    }
    public void deleteActionPlanByActionPlanCatalogId(long? actionPlanCatalogId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — actionPlanCatalogId)
        throw new NotImplementedException();
    }
    public void createNewActionPlanFromOldActionPlan(StrategyObject strategyObject, ActionPlanCatalog actionPlanCatalog, long? year)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — year)
        throw new NotImplementedException();
    }
    public void changeTashilgar(long? oldPayeshgarId, long? newPayeshgarId, long? planId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — planId)
        throw new NotImplementedException();
    }
    public void changeModir(long? oldModirId, long? newModirId, long? planId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — planId)
        throw new NotImplementedException();
    }
    // TODO(signature): ) { SearchDto searchDto = loadGridByObjectId(body, page, size, orderBy, direction); Pageable pageable = PageUtils.getInstance(page, size, orderBy, direction); v
    public void hardDeleteActionPlan(List<StrategyObject> deletedStgList)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — deletedStgList)
        throw new NotImplementedException();
    }
    public void assignFacilitatorAndManager(long? planningLevelId, long? facilitatorId, long? managerId)
        => _actionPlanRepository.updateFacilitatorAndManagerByPlanningLevelId(planningLevelId, facilitatorId, managerId);
    public void updateConfirmStatusAndTaskUrlAfterPlanning(long? actionPlanCatalogId, long? strategyObjectId)
    {
        try
        {
            var pair = _cartableStatusService.FindCartableStatusPair(
                nameof(ActionPlan), OUTCOME_CONFIRM, START_CREATION_CONFIRMATION_STATUS, ACTION_CREATE);
            _planningWriter.StageConfirmStatusAndTaskUrlAfterPlanning(
                actionPlanCatalogId, strategyObjectId, pair.NewStatus, pair.OldStatus,
                OUTCOME_CONFIRM, _taskUrls);
            // Java saveAll joins the enclosing transaction. The planning unit of
            // work flushes these tracked changes without an independent commit.
        }
        catch (BusinessException)
        {
            throw;
        }
        catch (Exception)
        {
            throw new BusinessException("خطایی در ارسال مقادیر برنامه ریزی به کمیته بالاتر پیش آمد!");
        }
    }
    void updateAndValidateYearCompletePcn(RequestActionPlanDto actionPlan)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — actionPlan)
        throw new NotImplementedException();
    }
    void updateAndValidateYearStartPcn(RequestActionPlanDto actionPlan)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — actionPlan)
        throw new NotImplementedException();
    }
    // TODO(signature): StrategyObject previousTarget ) {}
    void validateTenantStrategyObjRules(RequestActionPlanDto baseActionPlan)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — baseActionPlan)
        throw new NotImplementedException();
    }
    void setTopLevelStrategyObjectForPage(PagedResult<ActionPlanDto> dataPage)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — dataPage)
        throw new NotImplementedException();
    }
    List<ActionPlan> resolveActionPlansToDelete(ActionPlan foundActionPlan)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — foundActionPlan)
        throw new NotImplementedException();
    }
    // TODO(signature): ) { ActionPlanCatalog catalog = source.getActionPlanCatalog(); Optional<ActionPlan> existing = actionPlanRepository.findByActionPlanCatalogIdAndStrategyObjectId
    string? explicitTargetKey(StrategyObject source)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — source)
        throw new NotImplementedException();
    }
    void validateCanDeleteOnlyThisLevel(ActionPlan actionPlan)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — actionPlan)
        throw new NotImplementedException();
    }
    /**
    * فلگ‌های هر کاتالوگ فقط نشان می‌دهند که خودِ همان کاتالوگ چه چیزی به بالا ارسال کرده.
    * باید بعد از ساخته‌شدن Relation اجرا شود (existsByRelated... به آن نیاز دارد).
    */
    void applySendFlags(long? originCatalogId, HashSet<long?> involvedCatalogIds, bool parentOnly)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — parentOnly)
        throw new NotImplementedException();
    }
    void detachFromLowerCatalogs(long? catalogId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — catalogId)
        throw new NotImplementedException();
    }
}
