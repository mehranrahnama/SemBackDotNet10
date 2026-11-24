using Microsoft.EntityFrameworkCore;
using GridSearchDto = global::SemBackDotNet10.Base.model.dto.SearchDto;
using TenantContext = global::SemBackDotNet10.SecurityDomain.Tenancy.TenantContext;
using EntityManager = global::SemBackDotNet10.Data.SemDbContext;
using PlaningLevelUserService = global::SemBackDotNet10.service.impl.PlaningLevelUserServiceImpl;
using PlaningLevelService = global::SemBackDotNet10.service.impl.PlaningLevelServiceImpl;
using StrategyObjectService = global::SemBackDotNet10.basicDomain.service.IStrategyObjectService;
using EvaluationMeasureService = global::SemBackDotNet10.evaluationDomain.service.interf.IEvaluationMeasureService;
using MeasureRepository = global::SemBackDotNet10.measureDomain.repository.MeasureRepository;
using global::SemBackDotNet10.Common.Pagination;
using global::SemBackDotNet10.dashboardDomain.repository;
using global::SemBackDotNet10.measureDomain.controller.dto.measureDtos;
using SemLogger = global::Common.logging.SemLogger;
using UserRolesHelpers = global::Common.utils.userRoles.UserRolesHelpers;
// Mechanical conversion from Java (service/impl/MeasureServiceImpl.java). Complex bodies are marked with TODO.
// وابستگی‌های اعلام‌شده: basicDomain/Common/base هنوز ارسال نشده‌اند.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SemBackDotNet10.measureDomain.model.entity;
using SemBackDotNet10.measureDomain.model.dto;
using SemBackDotNet10.measureDomain.controller.dto;
using SemBackDotNet10.measureDomain.mapper;
using SemBackDotNet10.measureDomain.repository;
using SemBackDotNet10.measureDomain.service.interf;
using SemBackDotNet10.measureDomain.domain;
using SemBackDotNet10.measureDomain.strategy.confirmStatusHandler.context;
using SemBackDotNet10.measureDomain.strategy.confirmStatusHandler.factory;
using SemBackDotNet10.measureDomain.strategy.confirmStatusHandler.handler;
using global::Common.model.entity;
using SemBackDotNet10.basicDomain.Repositories;
using SemBackDotNet10.basicDomain.service;
using SemBackDotNet10.SecurityDomain.Model.Entity;
using SemBackDotNet10.SecurityDomain.Infrastructure.Persistence;
using global::Common.repository;
using global::Common.service.interf;
using global::Common.model.dto;
using global::Common.controller.dto;
using SemBackDotNet10.Common.enums;
using SemBackDotNet10.Common.exception;
using global::Common.messages;
using global::Common.repository;
using global::Common.service.interf;
using global::Common.service.impl;


namespace SemBackDotNet10.measureDomain.service.impl;

/// <summary>معادل MeasureServiceImpl.java — تزریق وابستگی‌ها با constructor injection انجام شده است.</summary>
public class MeasureServiceImpl : IMeasureService
{
    private readonly global::SemBackDotNet10.basicDomain.service.ITenantService _tenantService;
    private readonly MeasureRepository _measureRepository;
    private readonly MeasureActPlanRepository _measureActPlanRepository;
    private readonly IMeasureCatalogService _measureCatalogService;
    private readonly StrategyObjectService _strategyObjectService;
    private readonly global::SemBackDotNet10.basicDomain.service.IPlanPeriodService _planPeriodService;
    private readonly CartableStatusService _cartableStatusService;
    private readonly CommentService _commentService;
    private readonly MeasureReplanLogRepository _measureReplanLogRepository;
    private readonly IMeasureActPlanService _measureActPlanService;
    private readonly UserRolesHelpers _userRolesHelpers;
    private readonly TenantContext _tenantContext;
    private readonly EntityManager _entityManager;
    private readonly IMeasureMapper _measureMapper;
    private readonly HistoryService _historyService;
    private readonly IMeasureActPlanLogService _measureActPlanLogService;
    private readonly global::SemBackDotNet10.basicDomain.service.ISettingService _settingService;
    private readonly IMeasureRepositoryService _measureRepositoryService;
    private readonly IMeasureReplanLogService _measureReplanLogService;
    private readonly global::SemBackDotNet10.basicDomain.service.IUserService _userService;
    private readonly PlaningLevelUserService _planingLevelUserService;
    private readonly SemSecurityUtilityService _semSecurityUtilityService;
    private readonly IMeasureBeneficiaryService _measureBeneficiaryService;
    private readonly IMeasureCoworkerService _measureCoworkerService;
    private readonly MeasureBeneficiaryRepository _measureBeneficiaryRepository;
    private readonly MeasureCoworkerRepository _measureCoworkerRepository;
    private readonly global::SemBackDotNet10.SecurityDomain.Service.Interf.IUserRoleService _userRoleService;
    private readonly IMeasureCatalogLogService _measureCatalogLogService;
    private readonly SemLogger _semLogger;
    private readonly EvaluationMeasureService _evaluationMeasureService;
    private readonly PlaningLevelService _planingLevelService;
    private readonly IMeasureCatalogRelationService _measureCatalogRelationService;
    private readonly global::SemBackDotNet10.baseDomain.resolver.IStrategyObjectResolver _strategyObjectResolver;
    private readonly MeasureCartableStatusHandlerFactory _measureCartableStatusHandlerFactory;
    private readonly MeasureCatalogRelationDomainService _measureCatalogRelationDomainService;
    private readonly MeasureCatalogDomainService _measureCatalogDomainService;
    private readonly IMeasureStrategyPropagationService _measureStrategyPropagationService;
    private readonly MeasureCatalogRelationRepository _measureCatalogRelationRepository;
    private readonly MeasureDeletionService _measureDeletionService;
    private readonly DashboardRepository _dashboardRepository;
    private readonly string? _expireDate;
    private readonly long _FIXED_CONFIRM_STATUS_FOR_SPECIAL_TENANTS;

    public MeasureServiceImpl(global::SemBackDotNet10.basicDomain.service.ITenantService tenantService, MeasureRepository measureRepository, MeasureActPlanRepository measureActPlanRepository, IMeasureCatalogService measureCatalogService, StrategyObjectService strategyObjectService, global::SemBackDotNet10.basicDomain.service.IPlanPeriodService planPeriodService, CartableStatusService cartableStatusService, CommentService commentService, MeasureReplanLogRepository measureReplanLogRepository, IMeasureActPlanService measureActPlanService, UserRolesHelpers userRolesHelpers, EntityManager entityManager, TenantContext tenantContext, IMeasureMapper measureMapper, HistoryService historyService, IMeasureActPlanLogService measureActPlanLogService, global::SemBackDotNet10.basicDomain.service.ISettingService settingService, IMeasureRepositoryService measureRepositoryService, IMeasureReplanLogService measureReplanLogService, global::SemBackDotNet10.basicDomain.service.IUserService userService, PlaningLevelUserService planingLevelUserService, SemSecurityUtilityService semSecurityUtilityService, IMeasureBeneficiaryService measureBeneficiaryService, IMeasureCoworkerService measureCoworkerService, MeasureBeneficiaryRepository measureBeneficiaryRepository, MeasureCoworkerRepository measureCoworkerRepository, global::SemBackDotNet10.SecurityDomain.Service.Interf.IUserRoleService userRoleService, IMeasureCatalogLogService measureCatalogLogService, SemLogger semLogger, EvaluationMeasureService evaluationMeasureService, PlaningLevelService planingLevelService, IMeasureCatalogRelationService measureCatalogRelationService, global::SemBackDotNet10.baseDomain.resolver.IStrategyObjectResolver strategyObjectResolver, MeasureCartableStatusHandlerFactory measureCartableStatusHandlerFactory, MeasureCatalogRelationDomainService measureCatalogRelationDomainService, MeasureCatalogDomainService measureCatalogDomainService, IMeasureStrategyPropagationService measureStrategyPropagationService, MeasureCatalogRelationRepository measureCatalogRelationRepository, MeasureDeletionService measureDeletionService, DashboardRepository dashboardRepository, string? expireDate, long FIXED_CONFIRM_STATUS_FOR_SPECIAL_TENANTS)
    {
        _measureRepository = measureRepository;
        _measureActPlanRepository = measureActPlanRepository;
        _measureCatalogService = measureCatalogService;
        _strategyObjectService = strategyObjectService;
        _planPeriodService = planPeriodService;
        _cartableStatusService = cartableStatusService;
        _commentService = commentService;
        _measureReplanLogRepository = measureReplanLogRepository;
        _measureActPlanService = measureActPlanService;
        _userRolesHelpers = userRolesHelpers;
        _entityManager = entityManager;
        _tenantService = tenantService;
        _tenantContext = tenantContext;
        _measureMapper = measureMapper;
        _historyService = historyService;
        _measureActPlanLogService = measureActPlanLogService;
        _settingService = settingService;
        _measureRepositoryService = measureRepositoryService;
        _measureReplanLogService = measureReplanLogService;
        _userService = userService;
        _planingLevelUserService = planingLevelUserService;
        _semSecurityUtilityService = semSecurityUtilityService;
        _measureBeneficiaryService = measureBeneficiaryService;
        _measureCoworkerService = measureCoworkerService;
        _measureBeneficiaryRepository = measureBeneficiaryRepository;
        _measureCoworkerRepository = measureCoworkerRepository;
        _userRoleService = userRoleService;
        _measureCatalogLogService = measureCatalogLogService;
        _semLogger = semLogger;
        _evaluationMeasureService = evaluationMeasureService;
        _planingLevelService = planingLevelService;
        _measureCatalogRelationService = measureCatalogRelationService;
        _strategyObjectResolver = strategyObjectResolver;
        _measureCartableStatusHandlerFactory = measureCartableStatusHandlerFactory;
        _measureCatalogRelationDomainService = measureCatalogRelationDomainService;
        _measureCatalogDomainService = measureCatalogDomainService;
        _measureStrategyPropagationService = measureStrategyPropagationService;
        _measureCatalogRelationRepository = measureCatalogRelationRepository;
        _measureDeletionService = measureDeletionService;
        _dashboardRepository = dashboardRepository;
        _expireDate = expireDate;
        _FIXED_CONFIRM_STATUS_FOR_SPECIAL_TENANTS = FIXED_CONFIRM_STATUS_FOR_SPECIAL_TENANTS;
    }

    // TODO(ctor): سازنده‌ی جاوا (جز تزریق وابستگی) منتقل نشده است.
    // TODO(signature): ) { this.measureRepository = measureRepository; this.measureActPlanRepository = measureActPlanRepository; this.measureCatalogService = measureCatalogService; th
    // TODO(signature): private static final Set<String> TENANTS_WITH_FIXED_CONFIRM_STATUS = Set.of("MSCHOL
    public MeasureRepository GetRepository() => _measureRepository;

    public MeasureRepository getRepository() => GetRepository();
    public CommentRepository GetCommentRepository() => _commentService.GetRepository();
    public HistoryRepository GetHistoryRepository() => _historyService.GetRepository();
    public Measure findByIdOrElseThrow(long? id)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — id)
        throw new NotImplementedException();
    }
    // TODO(signature): ) { semLogger.info("Enter the method"); Pageable pageable = PageUtils.getInstance(page, size, orderBy, direction); planingLevelUserService.hasAccessInLevelId(bo
    void setIsPlanned(PagedResult<MeasureDto> dataPage)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — dataPage)
        throw new NotImplementedException();
    }
    public MeasureDto mapToDto(Measure measure)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measure)
        throw new NotImplementedException();
    }
    Measure mapToEntity(RequestMeasureDto measureDto, long? resolvedMeasureCatalogId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — resolvedMeasureCatalogId)
        throw new NotImplementedException();
    }
    // @Transactional
    public Measure create(RequestMeasureDto measureDto)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureDto)
        throw new NotImplementedException();
    }
    // ---- Helpers ----
    long? resolveMeasureCatalogId(RequestMeasureDto dto)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — dto)
        throw new NotImplementedException();
    }
    void logMeasureAction(RequestMeasureDto dto, string? template)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — template)
        throw new NotImplementedException();
    }
    void validateNoExistingConnection(long? measureCatalogId, long? planId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — planId)
        throw new NotImplementedException();
    }
    void assignConfirmStatusAndTaskUrl(Measure measure, RequestMeasureDto measureDto)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureDto)
        throw new NotImplementedException();
    }
    string? buildMeasureManagementTaskUrl(long? entityId, long? oldStatus, long? newStatus)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — newStatus)
        throw new NotImplementedException();
    }
    void applyTenantSpecificAssociations(Measure measure, RequestMeasureDto measureDto)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureDto)
        throw new NotImplementedException();
    }
    void assignModirRoleIfPresent(RequestMeasureDto measureDto)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureDto)
        throw new NotImplementedException();
    }
    bool isParentOnlyTenant()
    {
        return global::Common.utils.TenantNameConstants.IsParentOnlyTenant(
            _tenantService.FindByIdOrElseThrow(_tenantContext.TenantId).TenantName);
    }
    void validateUpperLevelsMode(UpperLevelsSendMode mode)
    {
        if (mode == UpperLevelsSendMode.PARENT_ONLY && !isParentOnlyTenant())
            throw new global::SemBackDotNet10.Common.exception.BusinessException("ارسال فقط به یک سطح بالاتر در این سازمان فعال نیست.");
    }
    // TODO(signature): boolean isEditFlow ) { switch (mode) { case ALL -> sendToUpperPlanningLevels( measure, explicitUpperLevelStrategyObjectId, isEditFlow, mode); case PARENT_ONLY -
    // @Transactional(propagation = Propagation.MANDATORY)
    void removeSentToUpperLevelMeasure(Measure measure)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measure)
        throw new NotImplementedException();
    }
    //    private void setMeasureCatalogId(MeasureDto measureDto) {
    //        semLogger.info("Enter the method");
    //        if (measureDto.getMeasureRepositoryId() != null && measureDto.getLevelId() != null) {
    //            planingLevelUserService.hasAccessInLevelId(measureDto.getLevelId(), semSecurityUtilityService.getCurrentUserId());
    //            measureDto.setMeasureCatalogId(measureCatalogService.findIdByRepositoryIdAndLevelId(measureDto.getMeasureRepositoryId(), measureDto.getLevelId()));
    //        }
    //        semLogger.info("Exit method");
    //    }
    // @Transactional
    public void editConfirmStatus(long? id, string? outCome)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — outCome)
        throw new NotImplementedException();
    }
    string? generateTaskUrlForReplan(long? id, long? newStatus, long? oldStatus)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — oldStatus)
        throw new NotImplementedException();
    }
    public void editIsReplannedMeasure(long? measureCatalogId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureCatalogId)
        throw new NotImplementedException();
    }
    // @Transactional
    public Measure edit(RequestMeasureDto measureDto)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureDto)
        throw new NotImplementedException();
    }
    // ---- Helpers ----
    void applyEditableMeasureFields(Measure measure, RequestMeasureDto dto)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — dto)
        throw new NotImplementedException();
    }
    void applyResponsibleUsersOnEdit(Measure measure, RequestMeasureDto dto)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — dto)
        throw new NotImplementedException();
    }
    public void sendToUpperPlanningLevels(Measure measure, long? explicitUpperLevelStrategyObjectId,
        bool isEditFlow, UpperLevelsSendMode sendMode)
    {
        try
        {
            using var transaction = _entityManager.Database.CurrentTransaction is null
                ? _entityManager.Database.BeginTransaction() : null;
            var parentOnly = sendMode == UpperLevelsSendMode.PARENT_ONLY;
            measure.SendToParentOnly = parentOnly;
            var currentLevel = measure.MeasureCatalog.PlaningLevel;
            var planId = measure.PlanPeriod.PlanId;
            _entityManager.Entry(currentLevel).Reference(level => level.Parent).Load();
            var levels = parentOnly ? findBaseAndParentLevels(currentLevel)
                : findUpperLevels(currentLevel);
            var catalogs = _measureCatalogService.createMeasureCatalogForPlanningLevels(levels, measure.MeasureCatalog);
            var upperCatalogs = catalogs.Where(catalog => catalog.Id != measure.MeasureCatalog.Id).ToList();
            var targets = resolveStrategyObjectsForUpperLevels(levels.Where(level => level.Id != currentLevel.Id).ToList(),
                measure.StrategyObject, upperCatalogs, explicitUpperLevelStrategyObjectId, !isEditFlow, planId);

            measure.AddToSupremeLevel = true;
            _measureRepository.SaveAsync(measure).GetAwaiter().GetResult();
            var pending = findOrCloneForMeasureCatalogsAndStrategyObjects(upperCatalogs, targets, measure);
            if (pending.Count != 0)
            {
                var saved = _measureRepository.SaveAllAsync(pending).GetAwaiter().GetResult();
                foreach (var target in saved)
                    if (target is { Id: long id, TaskUrl: string url })
                        target.TaskUrl = System.Text.RegularExpressions.Regex.Replace(url, @"entityId=\d+",
                            "entityId=" + id.ToString(System.Globalization.CultureInfo.InvariantCulture));
                _measureRepository.SaveAllAsync(saved).GetAwaiter().GetResult();
            }

            _measureCatalogRelationService.createRelationForPlaningLevel(catalogs, levels, planId);
            _measureActPlanService.createAndCopyMeasureActPlans(measure);
            applySendFlags(measure, catalogs.Select(catalog => catalog.Id).ToHashSet(), parentOnly, planId);
            transaction?.Commit();
        }
        catch (global::SemBackDotNet10.Common.exception.BusinessException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _semLogger.Error($"Error sending measure to upper planning levels: {ex.Message}");
            throw new global::SemBackDotNet10.Common.exception.BusinessException("خطایی در فرآیند ارسال شاخص به کمیته بالاتر پیش آمد. ");
        }
    }
    private List<PlaningLevel> findUpperLevels(PlaningLevel? level)
    {
        var levels = new List<PlaningLevel>();
        var seen = new HashSet<long>();
        while (level is not null)
        {
            if (!seen.Add(level.Id))
                throw new global::SemBackDotNet10.Common.exception.BusinessException("چرخه در سلسله‌مراتب کمیته‌ها وجود دارد.");
            levels.Add(level);
            _entityManager.Entry(level).Reference(item => item.Parent).Load();
            level = level.Parent;
        }
        return levels;
    }

    List<PlaningLevel> findBaseAndParentLevels(PlaningLevel? baseLevel)
    {
        if (baseLevel?.Parent is not { } parent)
            throw new global::SemBackDotNet10.Common.exception.BusinessException("این کمیته سطح بالاتری ندارد.");

        return new List<PlaningLevel> { baseLevel, parent };
    }
    private Dictionary<long, StrategyObject> resolveStrategyObjectsForUpperLevels(
        List<PlaningLevel> upperLevels, StrategyObject source, List<MeasureCatalog> catalogs,
        long? explicitObjectId, bool skipExisting, long? planId)
    {
        var result = new Dictionary<long, StrategyObject>();
        if (upperLevels.Count == 0)
            return result;

        var explicitObject = explicitObjectId is long id ? _strategyObjectService.FindByIdOrElseThrow(id) : null;
        if (explicitObject is not null && explicitObject.PlaningLevel is null)
            throw new global::SemBackDotNet10.Common.exception.BusinessException("هدف انتخاب‌شده برای سطح بالاتر، به هیچ سطح برنامه‌ریزی متصل نیست.");

        try
        {
            foreach (var catalog in catalogs)
            {
                if (skipExisting && hasMeasureFoCatalogId(catalog.Id, planId))
                    continue;

                var level = catalog.PlaningLevel;
                var effectiveSource = explicitObject ?? source;
                result[level.Id] = explicitObject is not null && explicitObject.PlaningLevel!.Id == level.Id
                    ? explicitObject
                    : _strategyObjectService.FindOrCreateStrategyObject(effectiveSource, level, effectiveSource.PlanPeriod!);
            }
            return result;
        }
        catch (global::SemBackDotNet10.Common.exception.BusinessException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _semLogger.Error($"Exception resolving strategy objects for upper measure levels: {ex.Message}");
            throw new global::SemBackDotNet10.Common.exception.BusinessException("خطایی در فرآیند یافتن اهداف متناظر در کمیته های بالاتر رخ داد.");
        }
    }

    private bool hasMeasureFoCatalogId(long? catalogId, long? planId)
    {
        try
        {
            return _measureRepository.existsByMeasureCatalogId(catalogId, planId);
        }
        catch (Exception ex)
        {
            _semLogger.Error($"Exception checking measure catalog assignment: {ex.Message}");
            throw new global::SemBackDotNet10.Common.exception.BusinessException("خطایی در دریافت اطلاعات اتصال شاخص به هدف در کمیته مربوطه رخ داد!");
        }
    }
    private List<Measure> findOrCloneForMeasureCatalogsAndStrategyObjects(
        IReadOnlyList<MeasureCatalog> catalogs, IReadOnlyDictionary<long, StrategyObject> targets, Measure source)
    {
        var result = new List<Measure>();
        var planId = source.PlanPeriod.PlanId;
        foreach (var catalog in catalogs)
        {
            if (!targets.TryGetValue(catalog.PlaningLevel.Id, out var strategyObject))
                continue;
            try
            {
                var existing = _measureRepository.FindAllAsync(candidate => !candidate.Deleted &&
                        candidate.MeasureCatalog.Id == catalog.Id && candidate.PlanPeriod.PlanId == planId)
                    .GetAwaiter().GetResult().FirstOrDefault();
                if (existing is not null)
                {
                    _measureMapper.copyPlanningValuesForTargetMeasure(source, existing);
                    existing.StrategyObject = strategyObject;
                    existing.AddToSupremeLevel = true;
                    result.Add(existing);
                    continue;
                }

                result.Add(new Measure
                {
                    MeasureCatalog = catalog,
                    StrategyObject = strategyObject,
                    PlanPeriod = source.PlanPeriod,
                    Weight = 0f,
                    SendToParentOnly = false,
                    AddToSupremeLevel = source.AddToSupremeLevel,
                    ReplannedStatus = source.ReplannedStatus,
                    ReplannedForHistory = source.ReplannedForHistory,
                    TaskUrl = source.TaskUrl,
                    CurrentValue = source.CurrentValue,
                    PerfectExtreme = source.PerfectExtreme,
                    WarnExtreme = source.WarnExtreme,
                    WorstMin = source.WorstMin,
                    CurrentYearTarget = source.CurrentYearTarget,
                    LongTermTarget = source.LongTermTarget,
                    TargetTheory = source.TargetTheory,
                    ZeroTarget = source.ZeroTarget,
                    ShortTermTarget = source.ShortTermTarget,
                    FlgKpi = source.FlgKpi,
                    DummyUpdate = source.DummyUpdate,
                    ZeroInitial = source.ZeroInitial,
                    Confirmstatus = source.Confirmstatus,
                    OneYearLaterTarget = source.OneYearLaterTarget,
                    TwoYearLaterTarget = source.TwoYearLaterTarget,
                    Planned = source.Planned,
                    MeasureActPlans = new List<MeasureActPlan>(),
                    OutCome = source.OutCome,
                    BenchmarkValue = source.BenchmarkValue,
                    Modir = source.Modir,
                    Tashilgar = source.Tashilgar
                });
            }
            catch (Exception ex)
            {
                _semLogger.Error($"Error fetching or cloning upper-level measures: {ex.Message}");
                throw new global::SemBackDotNet10.Common.exception.BusinessException("خطایی در ایجاد اتصال های شاخص به هدف کمیته ی بالاتر رخ داد!");
            }
        }
        return result;
    }

    public Measure copyMeasureForUpperLevel(long? catalogId, Measure measure)
    {
        var planId = measure.PlanPeriod.PlanId;
        var candidates = _measureRepository.FindAllAsync(candidate =>
                candidate.MeasureCatalog.Id == catalogId && candidate.PlanPeriod.PlanId == planId && !candidate.Deleted)
            .GetAwaiter().GetResult();
        var target = candidates.Single();
        _measureMapper.copyPlanningValuesForTargetMeasure(measure, target);
        target.Planned = true;
        return _measureRepository.SaveAsync(target).GetAwaiter().GetResult();
    }
    public List<Measure> findMeasuresByMeasureCatalogId(List<long?> ancestorIds, long? planId)
    {
        if (ancestorIds.Count == 0)
            return new List<Measure>();
        return _measureRepository.FindAllAsync(measure => !measure.Deleted &&
                measure.PlanPeriod.PlanId == planId && ancestorIds.Contains(measure.MeasureCatalog.Id))
            .GetAwaiter().GetResult();
    }
    public List<Measure> findAllWithTaskUrlIsNotNull()
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — findAllWithTaskUrlIsNotNull
        throw new NotImplementedException();
    }
    // @Transactional(propagation = Propagation.MANDATORY)
    public void setStrategyObjectWithNewPlanningLevel(PlaningLevel planingLevel, long? catalogId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — catalogId)
        throw new NotImplementedException();
    }
    // @Transactional(propagation = Propagation.MANDATORY)
    public void updateAddToSupremeLevelByCatalog(long? catalogId, bool addToSupremeLevel)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — addToSupremeLevel)
        throw new NotImplementedException();
    }
    // @Transactional(propagation = Propagation.MANDATORY)
    public void updateAddToSupremeLevelByCatalog(long? catalogId, long? planId, bool addToSupremeLevel)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — addToSupremeLevel)
        throw new NotImplementedException();
    }
    public string? findMeasureNameByMeasureId(long? measureId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureId)
        throw new NotImplementedException();
    }
    // TODO(signature): ) { if (base == null) throw new BusinessException("اتصال شاخص به هدف نمیتواند خالی باشد!"); base.setAddToSupremeLevel(true); try { measureRepository.save(base);
    // TODO(signature): ) { List<Measure> toSave = new ArrayList<>(); for (MeasureCatalog catalog : measureCatalogs) { PlaningLevel planningLevel = catalog.getPlaningLevel(); StrategyO
    public void delete(long? id)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — id)
        throw new NotImplementedException();
    }
    public void editAllWeights(List<MeasureDto> measureDtos)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureDtos)
        throw new NotImplementedException();
    }
    public GridSearchDto loadObjectGrid(LoadStrategyObjectDto body, int page, int size, string? orderBy, SortDirection direction)
    {
        ArgumentNullException.ThrowIfNull(body);
        var levelId = body.LevelId ?? throw new ArgumentException("LevelId is required.");
        var planId = body.PlanId ?? throw new ArgumentException("PlanId is required.");
        var planned = body.IsPlanned ?? throw new ArgumentException("IsPlanned is required.");
        _planingLevelUserService.HasAccessInLevelIdAsync(levelId, _semSecurityUtilityService.GetCurrentUserId()).GetAwaiter().GetResult();
        var pagination = global::Common.utils.PageUtils.GetInstance(page, size, orderBy, direction);
        var tenantId = _tenantContext.TenantId;
        var tenantCode = _entityManager.Set<global::SemBackDotNet10.basicDomain.Model.Entity.Tenant>()
            .Where(t => t.Id == tenantId).Select(t => t.TenantCode).SingleOrDefault()
            ?? throw new InvalidOperationException("Current tenant code was not found.");
        var facilitator = _userRolesHelpers.GetUserIdIfIsFacilitator(tenantCode);
        var measures = _entityManager.Set<Measure>().AsNoTracking();
        var eligible = measures.Where(m => m.Confirmstatus != null && m.Confirmstatus != 50);
        var matching = eligible.Where(m => (!planned || m.Planned) && (body.Status == null || m.Confirmstatus == body.Status));
        if (body.FilterMeasure is string measureFilter)
            matching = matching.Where(m => EF.Functions.Like(m.MeasureCatalog.MeasureRepository.MeasureName!, "%" + measureFilter + "%"));
        if (body.FilterObject is string objectFilter)
            matching = matching.Where(m => EF.Functions.Like(m.StrategyObject.Title!, "%" + objectFilter + "%"));
        if (body.FilterFacilitator is string facilitatorFilter)
            matching = matching.Where(m => EF.Functions.Like(m.Tashilgar.NameF + " " + m.Tashilgar.FamilyF, "%" + facilitatorFilter + "%"));
        if (body.FilterResponsible is string responsibleFilter)
            matching = matching.Where(m => EF.Functions.Like(m.Modir.NameF + " " + m.Modir.FamilyF, "%" + responsibleFilter + "%"));

        var objects = _entityManager.Set<global::SemBackDotNet10.basicDomain.Model.Entity.StrategyObject>().AsNoTracking()
            .Where(o => o.PlaningLevel.Id == levelId && o.PlanPeriod.PlanId == planId
                && matching.Any(m => m.StrategyObject.Id == o.Id)
                && measures.Any(m => m.StrategyObject.Id == o.Id && (facilitator == null
                    || m.Tashilgar.UserId == facilitator || m.MeasureCatalog.TashilgarHamkar.UserId == facilitator)));
        var totals = eligible.Where(m => m.MeasureCatalog.PlaningLevel.Id == levelId && m.PlanPeriod.PlanId == planId);
        var count = objects.LongCount();
        var descending = direction == SortDirection.Desc;
        var ordered = pagination.OrderBy?.ToLowerInvariant() switch
        {
            null => objects.OrderBy(o => o.Id),
            "id" => descending ? objects.OrderByDescending(o => o.Id) : objects.OrderBy(o => o.Id),
            "title" => descending ? objects.OrderByDescending(o => o.Title).ThenBy(o => o.Id) : objects.OrderBy(o => o.Title).ThenBy(o => o.Id),
            _ => throw new ArgumentException("Unsupported strategy object sort field.", nameof(orderBy))
        };
        var rows = ordered.Skip(pagination.Offset).Take(pagination.Size).Select(o => new MeasureObjectDto
        {
            Id = o.Id,
            ObjectTitle = o.Title,
            MeasureCount = totals.LongCount(m => m.StrategyObject.Id == o.Id && (!planned || m.Planned)),
            TotalWeight = totals.Any(m => m.StrategyObject.Id == o.Id && m.Weight != null)
                ? totals.Where(m => m.StrategyObject.Id == o.Id).Sum(m => (double?)m.Weight) : null
        }).ToList();
        return new GridSearchDto.Builder().content(rows).totalElements(count)
            .totalPages((int)Math.Ceiling((double)count / pagination.Size)).currentPage(page).pageSize(size).build();
    }
    public Dictionary<string?,object?> getCounts(string? body)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — body)
        throw new NotImplementedException();
    }
    // @Transactional
    public void editDeleteStatus(string? body)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — body)
        throw new NotImplementedException();
    }
    // @Transactional
    void findAndDeletePassiveMeasureActPlanLogs(Measure measure)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measure)
        throw new NotImplementedException();
    }
    public MeasureDto findMeasureById(long? id)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — id)
        throw new NotImplementedException();
    }
    public List<Measure> findMeasureByMeasureCatalogId(long? MeasureCatalogId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — MeasureCatalogId)
        throw new NotImplementedException();
    }
    public MeasureDto findMeasureDtoByMeasureCatalogId(long? PlanId, long? MeasureCatalogId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — MeasureCatalogId)
        throw new NotImplementedException();
    }
    public long? CountActiveMeasures(long? planId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — planId)
        throw new NotImplementedException();
    }
    public long? CountActiveMeasuresByPlanIdLevelId(long? planId, long? levelId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — levelId)
        throw new NotImplementedException();
    }
    public bool expireMeasureTasks(long? tenantId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — tenantId)
        throw new NotImplementedException();
    }
    void setNextStep(List<Measure> expired)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — expired)
        throw new NotImplementedException();
    }
    // @Transactional(rollbackFor = Exception.class)
    public void measureReplanReject(long? measureId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureId)
        throw new NotImplementedException();
    }
    //Helpers
    void removeLogsForRejectedMeasure(long? replanLogId, long? measureId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureId)
        throw new NotImplementedException();
    }
    // TODO(signature): ) { measureMapper.updateRejectedReplannedMeasure( measure, taskUrl, newStatus, OUTCOME_REPLAN_REJECT, true, null ); measureRepository.save(measure); }
    // TODO(signature): ) { boolean isReplannedBefore = measureReplanLogService.existsPreviousReplanLog(measureId, logId); Long replannedStatus = isReplannedBefore ? FINAL_CREATION_CON
    void restoreMeasureValuesFromLog(MeasureReplanLog log)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — log)
        throw new NotImplementedException();
    }
    // @Transactional(rollbackFor = Exception.class)
    public Measure measureReplanConfirm(MeasureDto measureDto)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureDto)
        throw new NotImplementedException();
    }
    public void changeTashilgar(long? oldTashilgarId, long? newTashilgarId, long? planId)
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
    public void hardDeleteMeasure(List<StrategyObject> deletedStgList)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — deletedStgList)
        throw new NotImplementedException();
    }
    public void assignFacilitatorAndManager(long? planningLevelId, long? facilitatorId, long? managerId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — managerId)
        throw new NotImplementedException();
    }
    public void updatePlannedById(long? measureId, bool planned)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — planned)
        throw new NotImplementedException();
    }
    public List<Measure> getMeasureByParentLevelAndPlanId(long? planId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — planId)
        throw new NotImplementedException();
    }
    public Measure getFirstByMeasureCatalog(long? measureCatalogId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureCatalogId)
        throw new NotImplementedException();
    }
    public void updateYearTargetByMeasure(decimal? yearTarget, long? measureId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureId)
        throw new NotImplementedException();
    }
    public bool hasPreviousReplanNotConfirmed(long? measureId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureId)
        throw new NotImplementedException();
    }
    // @Transactional
    public MeasureReplanLogCreationDto findMeasureReplanLogCreationDto(long? measureId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureId)
        throw new NotImplementedException();
    }
    void validateStrategyObjectChangeAllowed(Measure foundMeasure, long? newStrategyObjectId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — newStrategyObjectId)
        throw new NotImplementedException();
    }
    void setTopLevelStrategyObject(PagedResult<MeasureDto> dataPage, long? planId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — planId)
        throw new NotImplementedException();
    }
    void validateCanDeleteOnlyThisLevel(Measure measure)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measure)
        throw new NotImplementedException();
    }
    // @Transactional(propagation = Propagation.MANDATORY)
    void removeUpperLevelsAboveParent(Measure measure)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measure)
        throw new NotImplementedException();
    }
    /**
    * فلگ‌های مقصدها را بعد از ساخته شدن relation تنظیم می‌کند (فقط همان سال).
    * در حالت «همه» مقصدها همان addToSupremeLevel = true قبلی را دارند و تغییری لازم نیست.
    * در حالت «فقط پدر»، مقصد فقط اگر خودش قبلاً به بالا ارسال داشته باشد true می‌ماند.
    * فلگ sendToParentOnly مقصد هیچ‌وقت تغییر نمی‌کند.
    */
    void applySendFlags(Measure origin, HashSet<long?> involvedCatalogIds, bool parentOnly, long? planId)
    {
        if (!parentOnly || !isParentOnlyTenant())
            return;
        _entityManager.SaveChanges();
        foreach (var catalogId in involvedCatalogIds.Where(id => id != origin.MeasureCatalog.Id))
        {
            var sentUp = _measureCatalogRelationService.existsByRelatedCatalogId(catalogId, planId);
            var targets = _measureRepository.FindAllAsync(measure => !measure.Deleted &&
                    measure.MeasureCatalog.Id == catalogId && measure.PlanPeriod.PlanId == planId)
                .GetAwaiter().GetResult();
            foreach (var target in targets)
                target.AddToSupremeLevel = sentUp;
            if (targets.Count != 0)
                _measureRepository.SaveAllAsync(targets).GetAwaiter().GetResult();
        }
    }
}
