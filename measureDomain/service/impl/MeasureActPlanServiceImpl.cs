using Measure = global::SemBackDotNet10.measureDomain.model.entity.Measure;
using global::SemBackDotNet10.Common.Pagination;
using global::SemBackDotNet10.actionDomain.controller.dto;
using global::SemBackDotNet10.measureDomain.controller.dto.measureActPlanDtos;
using global::SemBackDotNet10.measureDomain.strategy.measurePeriodHandler;
using global::SemBackDotNet10.measureDomain.strategy.progressDirectionHandler;
using SearchDto = global::SemBackDotNet10.Base.model.dto.SearchDto;
using SemLogger = global::Common.logging.SemLogger;
using UserRolesHelpers = global::Common.utils.userRoles.UserRolesHelpers;
// Mechanical conversion from Java (service/impl/MeasureActPlanServiceImpl.java). Complex bodies are marked with TODO.
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
using SemBackDotNet10.basicDomain.Model.Entity;
using SemBackDotNet10.basicDomain.Repositories;
using SemBackDotNet10.basicDomain.service;
using SemBackDotNet10.SecurityDomain.Model.Entity;
using SemBackDotNet10.SecurityDomain.Repository;
using global::Common.repository;
using global::Common.service.interf;
using global::Common.model.dto;
using global::Common.controller.dto;
using SemBackDotNet10.Common.enums;
using SemBackDotNet10.Common.exception;
using global::Common.messages;
using global::Common.service.impl;
using SemBackDotNet10.measureDomain.repository.projection;
using SemBackDotNet10.actionDomain.service.interf;


using CorrectionActionsByEMSService = global::SemBackDotNet10.actionDomain.service.interf.ICorrectionActionsByEMSService;
using CallActionRestApiService = global::SemBackDotNet10.actionDomain.service.interf.ICallActionRestApiService;
namespace SemBackDotNet10.measureDomain.service.impl;

/// <summary>معادل MeasureActPlanServiceImpl.java — تزریق وابستگی‌ها با constructor injection انجام شده است.</summary>
public class MeasureActPlanServiceImpl : IMeasureActPlanService
{
    private readonly MeasureActPlanRepository _measureActPlanRepository;
    private readonly IMeasureCatalogService _measureCatalogService;
    private readonly IMeasureRepositoryService _measureRepositoryService;
    private readonly MeasureActPlanCalcRepository _measureActPlanCalcRepository;
    private readonly global::SemBackDotNet10.SecurityDomain.Service.Interf.IUserRoleService _userRoleService;
    private readonly global::SemBackDotNet10.service.impl.PlaningLevelServiceImpl _planingLevelService;
    private readonly ValueAssignmentStrategyFactory _strategyFactory;
    private readonly global::SemBackDotNet10.Data.SemDbContext _entityManager;
    private readonly ValueValidationStrategyFactory _valueValidationStrategyFactory;
    private readonly IMeasureService _measureService;
    private readonly HistoryService _historyService;
    private readonly CommentService _commentService;
    private readonly CartableStatusService _cartableStatusService;
    private readonly ICallActionRestApiService _callActionRestApiService;
    private readonly IUserService _userService;
    private readonly global::SemBackDotNet10.basicDomain.service.ISettingService _settingService;
    private readonly MeasureRepoRepository _measureRepoRepository;
    private readonly MeasureCatalogRepository _measureCatalogRepository;
    private readonly IMeasureReplanLogService _measureReplanLogService;
    private readonly IMeasureActPlanLogService _measureActPlanLogService;
    private readonly IMeasureCatalogRelationService _measureCatalogRelationService;
    private readonly global::SemBackDotNet10.service.impl.PlaningLevelUserServiceImpl _planingLevelUserService;
    private readonly SemSecurityUtilityService _semSecurityUtilityService;
    private readonly CartableStatusRepository _cartableStatusRepository;
    private readonly SemLogger _semLogger;
    private readonly IMeasureActPlanMapper _measureActPlanMapper;
    private readonly CorrectionActionsByEMSService _correctionActionsByEMSService;
    private readonly MeasureDomainService _measureDomainService;
    private readonly MeasureCartableStatusHandlerFactory _measureCartableStatusHandlerFactory;
    private readonly MeasureCatalogRelationDomainService _measureCatalogRelationDomainService;
    private readonly MeasureCatalogDomainService _measureCatalogDomainService;
    private readonly global::SemBackDotNet10.basicDomain.service.IPlanPeriodService _planPeriodService;
    private readonly UserRolesHelpers _userRolesHelpers;
    private readonly StoredAttachmentService _storedAttachmentService;
    private readonly string? _expireDate;

    public MeasureActPlanServiceImpl(MeasureActPlanRepository measureActPlanRepository, IMeasureCatalogService measureCatalogService, IMeasureRepositoryService measureRepositoryService, MeasureActPlanCalcRepository measureActPlanCalcRepository, global::SemBackDotNet10.SecurityDomain.Service.Interf.IUserRoleService userRoleService, global::SemBackDotNet10.service.impl.PlaningLevelServiceImpl planingLevelService, ValueAssignmentStrategyFactory strategyFactory, global::SemBackDotNet10.Data.SemDbContext entityManager, ValueValidationStrategyFactory valueValidationStrategyFactory, IMeasureService measureService, HistoryService historyService, CommentService commentService, CartableStatusService cartableStatusService, CallActionRestApiService callActionRestApiService, global::SemBackDotNet10.basicDomain.service.IUserService userService, global::SemBackDotNet10.basicDomain.service.ISettingService settingService, MeasureRepoRepository measureRepoRepository, MeasureCatalogRepository measureCatalogRepository, IMeasureReplanLogService measureReplanLogService, IMeasureActPlanLogService measureActPlanLogService, IMeasureCatalogRelationService measureCatalogRelationService, global::SemBackDotNet10.service.impl.PlaningLevelUserServiceImpl planingLevelUserService, SemSecurityUtilityService semSecurityUtilityService, CartableStatusRepository cartableStatusRepository, SemLogger semLogger, IMeasureActPlanMapper measureActPlanMapper, CorrectionActionsByEMSService correctionActionsByEMSService, MeasureDomainService measureDomainService, MeasureCartableStatusHandlerFactory measureCartableStatusHandlerFactory, MeasureCatalogRelationDomainService measureCatalogRelationDomainService, MeasureCatalogDomainService measureCatalogDomainService, global::SemBackDotNet10.basicDomain.service.IPlanPeriodService planPeriodService, UserRolesHelpers userRolesHelpers, StoredAttachmentService storedAttachmentService, string? expireDate)
    {
        _measureActPlanRepository = measureActPlanRepository;
        _measureCatalogService = measureCatalogService;
        _measureRepositoryService = measureRepositoryService;
        _measureActPlanCalcRepository = measureActPlanCalcRepository;
        _userRoleService = userRoleService;
        _planingLevelService = planingLevelService;
        _strategyFactory = strategyFactory;
        _entityManager = entityManager;
        _valueValidationStrategyFactory = valueValidationStrategyFactory;
        _measureService = measureService;
        _historyService = historyService;
        _commentService = commentService;
        _cartableStatusService = cartableStatusService;
        _callActionRestApiService = callActionRestApiService;
        _userService = userService;
        _settingService = settingService;
        _measureRepoRepository = measureRepoRepository;
        _measureCatalogRepository = measureCatalogRepository;
        _measureReplanLogService = measureReplanLogService;
        _measureActPlanLogService = measureActPlanLogService;
        _measureCatalogRelationService = measureCatalogRelationService;
        _planingLevelUserService = planingLevelUserService;
        _semSecurityUtilityService = semSecurityUtilityService;
        _cartableStatusRepository = cartableStatusRepository;
        _semLogger = semLogger;
        _measureActPlanMapper = measureActPlanMapper;
        _correctionActionsByEMSService = correctionActionsByEMSService;
        _measureDomainService = measureDomainService;
        _measureCartableStatusHandlerFactory = measureCartableStatusHandlerFactory;
        _measureCatalogRelationDomainService = measureCatalogRelationDomainService;
        _measureCatalogDomainService = measureCatalogDomainService;
        _planPeriodService = planPeriodService;
        _userRolesHelpers = userRolesHelpers;
        _storedAttachmentService = storedAttachmentService;
        _expireDate = expireDate;
    }

    // TODO(ctor): سازنده‌ی جاوا (جز تزریق وابستگی) منتقل نشده است.
    // TODO(signature): ) { this.measureActPlanRepository = measureActPlanRepository; this.measureCatalogService = measureCatalogService; this.measureRepositoryService = measureReposit
    public CommentRepository GetCommentRepository() => _commentService.GetRepository();
    public HistoryRepository GetHistoryRepository() => _historyService.GetRepository();
    public MeasureActPlanRepository GetRepository()
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — getRepository
        throw new NotImplementedException();
    }
    // @Transactional(rollbackFor = Exception.class)
    public void newPlanningByMeasure(NewPlanningDto dto)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — dto)
        throw new NotImplementedException();
    }
    public List<MeasureActPlan> findAllWithTaskUrlIsNotNull()
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — findAllWithTaskUrlIsNotNull
        throw new NotImplementedException();
    }
    // IMPORTANT: match interface return type exactly
    // @Transactional(propagation = Propagation.MANDATORY)
    public List<IKeyValueView> findAllWithNullActualValue()
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — findAllWithNullActualValue
        throw new NotImplementedException();
    }
    public void createAndCopyMeasureActPlans(Measure sourceMeasure)
    {
        var planId = sourceMeasure.PlanPeriod.PlanId;
        var ancestors = _measureCatalogRelationService.findAncestorIds(sourceMeasure.MeasureCatalog.Id, planId);
        var targets = _measureService.findMeasuresByMeasureCatalogId(ancestors, planId);
        if (targets.Count == 0)
            return;

        var source = _measureActPlanRepository.findAllByMeasureIdAndDeletedFalse(sourceMeasure.Id);
        if (source.Count == 0)
            return;
        if (source.Count != 12)
            throw new global::SemBackDotNet10.Common.exception.BusinessException("تعداد رکورد های برنامه ریزی اشتباه است.");

        var pending = new List<MeasureActPlan>();
        foreach (var target in targets)
        {
            var plans = _measureActPlanRepository.findAllByMeasureIdAndDeletedFalse(target.Id);
            if (plans.Count == 0)
            {
                pending.AddRange(source.Select(plan => _measureActPlanMapper.clone(plan, target)));
                continue;
            }
            if (plans.Count != source.Count)
                throw new global::SemBackDotNet10.Common.exception.BusinessException("عدم تطابق تعداد برنامه ریزی بین شاخص فرزند و والد");
            for (var i = 0; i < source.Count; i++)
            {
                _measureActPlanMapper.copyPlanValueAndActualValueAndMonthPlanValue(source[i], plans[i]);
                pending.Add(plans[i]);
            }
        }

        if (pending.Count == 0)
            return;
        try
        {
            var saved = _measureActPlanRepository.SaveAllAsync(pending).GetAwaiter().GetResult();
            foreach (var plan in saved)
                if (plan is { Id: long id, TaskUrl: string url })
                    plan.TaskUrl = System.Text.RegularExpressions.Regex.Replace(url, @"entityId=\d+",
                        "entityId=" + id.ToString(System.Globalization.CultureInfo.InvariantCulture));
            _measureActPlanRepository.SaveAllAsync(saved).GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            _semLogger.Error($"Error saving MeasureActPlans for upper levels: {ex.Message}");
            throw new global::SemBackDotNet10.Common.exception.BusinessException("خطایی در ذخیره برنامه‌ریزی شاخص‌های بالادستی رخ داد.");
        }
    }
    public void createMeasureActPlanLogForNewPlanning(long? measureId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureId)
        throw new NotImplementedException();
    }
    public void createEmptyMeasureActPlans(int multipleNumber, long? measureId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureId)
        throw new NotImplementedException();
    }
    public int findMultipleNumberByMeasurePeriod(int measurePeriod)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measurePeriod)
        throw new NotImplementedException();
    }
    public void archiveAndCleanOldPlans(long? measureId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureId)
        throw new NotImplementedException();
    }
    // @Transactional(rollbackFor = Exception.class)
    public SearchDto createMeasureActPlan(MeasureActPlanRequestDto requestDto, int page, int size, string? orderBy, SortDirection direction, bool replan)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — replan)
        throw new NotImplementedException();
    }
    // TODO(signature): String outCome, boolean replan) throws Exception { switch (payeshType) { case (periodic): { switch (desiredMethod) { case (ascending): case (strongly_ascending)
    public SearchDto getCustomMeasureActPlan(string? body, int page, int size, string? orderBy, SortDirection direction, bool replan)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — replan)
        throw new NotImplementedException();
    }
    public SearchDto findAll(long? id, int page, int size, string? orderBy, SortDirection direction)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — direction)
        throw new NotImplementedException();
    }
    public SearchDto findAll_V2(long? id, int page, int size, string? orderBy, SortDirection direction)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — direction)
        throw new NotImplementedException();
    }
    public MeasureActPlan findByIdOrElseThrow(long? id)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — id)
        throw new NotImplementedException();
    }
    public void insertToMeasureActPlan(long? PlanId, long? month, Measure measure, bool? active, decimal? planValue, decimal? monthPlanValue, long? confirmStatus, string? outCome)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — outCome)
        throw new NotImplementedException();
    }
    // TODO(signature): }
    public void updateMeasureActPlan(long? PlanId, long? month, Measure measure, bool? active, decimal? planValue, decimal? monthPlanValue, long? confirmStatus, string? outCome)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — outCome)
        throw new NotImplementedException();
    }
    // @Transactional(rollbackFor = Exception.class)
    public bool? EditMeasureActPlan(MeasureActPlanMeasureDto paramMapList)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — paramMapList)
        throw new NotImplementedException();
    }
    //helper
    void copyMeasureActPlansForUpperLevels(long? catalogId, Measure measure)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measure)
        throw new NotImplementedException();
    }
    // helper
    bool isReplanPending(Measure measure)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measure)
        throw new NotImplementedException();
    }
    // TODO(signature): ) { if (measureActPlanDtoList == null || measureActPlanDtoList.isEmpty()) throw new BusinessException("لیست برنامه ریزی شاخص باید حاوی 12 رکورد باشد."); List<Me
    public void parseCalculateAchievedPCNFormula(string? formula)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — formula)
        throw new NotImplementedException();
    }
    public List<MeasureActPlanCalc> findMeasureActPlanCalcByMeasureActPlanId(long? measureActPlanId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureActPlanId)
        throw new NotImplementedException();
    }
    public List<MeasureActPlanDto> findAllMeasureActPlanDtos()
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — findAllMeasureActPlanDtos
        throw new NotImplementedException();
    }
    // was: SearchDto loadMeasureProgressGrid(...)
    public SearchDto loadMeasureProgressGrid(Dictionary<string?, string?> headers, string? body, int page, int size, string? orderBy, SortDirection direction)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — direction)
        throw new NotImplementedException();
    }
    void setHasUploadedFile(List<MeasureProgressDto> measureProgressList)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureProgressList)
        throw new NotImplementedException();
    }
    void toggleHasCorrectionAction(PagedResult<MeasureProgressDto> dataPage, Dictionary<string?,string?> headers)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — headers)
        throw new NotImplementedException();
    }
    // ثبت عنوان اقدام اصلاحی EMS در رکوردهای ماهانه و انتقال آن به کمیته‌های بالاتر
    void setCorrectionTitleOnMeasureActPlans(Dictionary<long?,string?> correctionTitles)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — correctionTitles)
        throw new NotImplementedException();
    }
    string? mapOrderToQueryParams(string? orderBy)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — orderBy)
        throw new NotImplementedException();
    }
    bool? userIsAdmin(long? userId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — userId)
        throw new NotImplementedException();
    }
    // @Transactional
    public void editDailyValues(List<MeasureProgressDto> measureProgressDtos)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureProgressDtos)
        throw new NotImplementedException();
    }
    // @Transactional(propagation = Propagation.MANDATORY)
    public void calculateValuesForProgresses(Dictionary<long?, byte> measureIds)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureIds)
        throw new NotImplementedException();
    }
    void propagateMeasureActPlanProgressToUpperLevels(Measure measure, long? measureCatalogId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureCatalogId)
        throw new NotImplementedException();
    }
    public void setTaskUrl(List<MeasureProgressDto> measureProgressDtos)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureProgressDtos)
        throw new NotImplementedException();
    }
    // @Transactional(rollbackFor = Exception.class)
    public MeasureActPlanGetDeviationDto editDeviation(MeasureProgressDto measureProgressDto)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureProgressDto)
        throw new NotImplementedException();
    }
    // @Transactional(rollbackFor = Exception.class)
    public MeasureActPlanDto editCorrection(MeasureProgressDto measureProgressDto)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureProgressDto)
        throw new NotImplementedException();
    }
    // انتقال دلیل انحراف و اقدام اصلاحی به رکورد همان ماه در شاخص‌های کمیته‌های بالاتر
    void propagateDeviationAndCorrectionToUpperLevels(MeasureActPlan sourceMeasureActPlan)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — sourceMeasureActPlan
        throw new NotImplementedException();
    }
    void checkCorrectionDateValidation(MeasureProgressDto measureProgressDto)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureProgressDto)
        throw new NotImplementedException();
    }
    void DateShouldNotBeNull(MeasureProgressDto measureProgressDto)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureProgressDto)
        throw new NotImplementedException();
    }
    StartActionDTO createBody(MeasureActPlan result)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — result)
        throw new NotImplementedException();
    }
    long? calculateImprovePrecent(MeasureActPlan measureActPlan)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureActPlan)
        throw new NotImplementedException();
    }
    public void editStatus(MeasureProgressDto measureProgressDto)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureProgressDto)
        throw new NotImplementedException();
    }
    public List<MeasureActPlanDto> getCorrectionByMeasureId(long? id)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — id)
        throw new NotImplementedException();
    }
    MeasureActPlanDto mapToDto(MeasureActPlan measureAct)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureAct)
        throw new NotImplementedException();
    }
    public MeasureWrapperDTO getMeasureActPlanByMeasureId(long? measureId, int page, int size, string? orderBy, SortDirection direction)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — direction)
        throw new NotImplementedException();
    }
    public MeasureWrapperDTO getCustomMeasureActPlanByMeasureId(long? measureId, int page, int size, string? orderBy, SortDirection direction)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — direction)
        throw new NotImplementedException();
    }
    public List<MeasureActPlanDto> findAllmeasureActPlansByMeasureId(long? measureId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureId)
        throw new NotImplementedException();
    }
    public List<MeasureActPlanDto> findAllMeasureActPlansOfMeasure(long? measureActPlanId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureActPlanId)
        throw new NotImplementedException();
    }
    public MeasureProgressDto findMeasureProgressByMeasureActPlanId(long? measureActPlanId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureActPlanId)
        throw new NotImplementedException();
    }
    public bool expireMeasureActPlanTasks(long? tenantId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — tenantId)
        throw new NotImplementedException();
    }
    void setNextStep(List<MeasureActPlan> expired)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — expired)
        throw new NotImplementedException();
    }
    // @Transactional(rollbackFor = Exception.class)
    public void MeasureActRePlan(MeasureActPlanRequestDto requestDto, int page, int size, string? orderBy, SortDirection direction)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — direction)
        throw new NotImplementedException();
    }
    public void expireMeasureActPlans(List<MeasureActPlan> measureActPlans)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureActPlans)
        throw new NotImplementedException();
    }
    // TODO(signature): MeasureReplanActionInfoDto actionInfo ) throws Exception { semLogger.info("Enter the method"); List<BigDecimal> planValues = measureActRePlanDto.stream() .sorte
    //helpers
    void replanUpperLevelMeasureActPlans(Measure measure)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measure)
        throw new NotImplementedException();
    }
    // TODO(signature): BigDecimal newYearTarget ) { boolean measurePeriodChanged = !Objects.equals(actionInfo.measurePeriod(), measurePeriod); if (measurePeriodChanged) throw new Busi
    void validateTheProgressDirection(List<decimal?> planValues, int desiredMethod)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — desiredMethod)
        throw new NotImplementedException();
    }
    // TODO(signature): ) { ValueAssignmentStrategy strategy = strategyFactory.of(type); List<MeasurePeriodCalculationDto> calculationDtos = measureActPlanMapper.mapToMeasurePeriodCalc
    public void removeExpiredMeasureActPlan(List<MeasureActPlan> expiredMeasureActPlans)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — expiredMeasureActPlans)
        throw new NotImplementedException();
    }
    public void restoreMeasureActPlans(long? measureId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureId)
        throw new NotImplementedException();
    }
    void removeActiveMeasureActPlans(long? measureId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureId)
        throw new NotImplementedException();
    }
    void restoreDeletedMeasureActPlan(long? measureId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureId)
        throw new NotImplementedException();
    }
    double sigma(long? month)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — month)
        throw new NotImplementedException();
    }
    List<long?> getActiveMonth(int measurePeriod, string? customMonth)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — customMonth)
        throw new NotImplementedException();
    }
    List<long?> getProgressActiveMonth(Measure measure)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measure)
        throw new NotImplementedException();
    }
    void insertPeriodicAscendingMeasureActPlan(Measure measure, List<long?> activeMonth, long? confirmStatus, string? outCome, bool? replan)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — replan)
        throw new NotImplementedException();
    }
    void insertPeriodicDescendingMeasureActPlan(Measure measure, List<long?> activeMonth, long? confirmStatus, string? outCome, bool? replan)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — replan)
        throw new NotImplementedException();
    }
    void insertCumulativeAscendingMeasureActPlan(Measure measure, List<long?> activeMonth, long? confirmStatus, string? outCome, bool? replan)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — replan)
        throw new NotImplementedException();
    }
    void insertCumulativeDescendingMeasureActPlan(Measure measure, List<long?> activeMonth, long? confirmStatus, string? outCome, bool? replan)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — replan)
        throw new NotImplementedException();
    }
    void insertAverageAscendingMeasureActPlan(Measure measure, List<long?> activeMonth, long? confirmStatus, string? outCome, bool? replan)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — replan)
        throw new NotImplementedException();
    }
    void insertAverageDescendingMeasureActPlan(Measure measure, List<long?> activeMonth, long? confirmStatus, string? outCome, bool? replan)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — replan)
        throw new NotImplementedException();
    }
    // @Transactional
    public SearchDto insertEmptyMeasureActPlan(MeasureActPlanRequestDto requestDto, int page, int size, string? orderBy, SortDirection direction)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — direction)
        throw new NotImplementedException();
    }
    public void insertOtherMeasureActPlan(Measure measure, List<long?> activeMonth, long? confirmStatus, string? outCome, bool? replan)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — replan)
        throw new NotImplementedException();
    }
    void insertKeepStateMeasureActPlan(Measure measure, List<long?> activeMonth, long? confirmStatus, string? outCome, bool? replan)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — replan)
        throw new NotImplementedException();
    }
    void insertEvolutionCommitteeMeasureActPlan(Measure measure, List<long?> activeMonth, long? confirmStatus, string? outCome)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — outCome)
        throw new NotImplementedException();
    }
    Dictionary<string?,List<decimal?>> calculateWeightedAveragePlanValue(MeasureCatalog mainMeasureCatalog, List<long?> activeMonth)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — activeMonth)
        throw new NotImplementedException();
    }
    List<decimal?> calculateWeightedAverageActualValue(MeasureCatalog mainMeasureCatalog, List<long?> activeMonth)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — activeMonth)
        throw new NotImplementedException();
    }
    List<decimal?> calculateWeightedAverageDailyValue(MeasureCatalog mainMeasureCatalog, List<long?> activeMonth)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — activeMonth)
        throw new NotImplementedException();
    }
    void calculateAndSaveActualValue(Measure measure)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measure)
        throw new NotImplementedException();
    }
    void calculatePeriodicActualValue(List<MeasureActPlan> measureActPlans, List<long?> activeMonth)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — activeMonth)
        throw new NotImplementedException();
    }
    void calculateCumulativeActualValue(List<MeasureActPlan> measureActPlans, List<long?> activeMonth)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — activeMonth)
        throw new NotImplementedException();
    }
    void calculateAverageActualValue(List<MeasureActPlan> measureActPlans, List<long?> activeMonth)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — activeMonth)
        throw new NotImplementedException();
    }
    // was: void calculateActualAndDailyValues(...)
    public void calculateActualAndDailyValues(string? body)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — body)
        throw new NotImplementedException();
    }
    // was: decimal? getCorrectionActualValueForHOSCO(...)
    public decimal? getCorrectionActualValueForHOSCO(long? measureActPlanId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureActPlanId)
        throw new NotImplementedException();
    }
    // was: decimal? calcMeasurePCN(...)
    public decimal? calcMeasurePCN(long? measureActPlanId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureActPlanId)
        throw new NotImplementedException();
    }
    // was: void deleteMeasureActPlan(...)
    public void deleteMeasureActPlan(long? measureId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureId)
        throw new NotImplementedException();
    }
    // فایل‌های پیوست برنامه ماهانه همراه با حذف فیزیکی آن حذف می‌شوند
    void deleteMeasureActPlanAttachments(List<MeasureActPlan> measureActPlans)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureActPlans)
        throw new NotImplementedException();
    }
    // TODO(signature): List<Long> activeMonths ) throws Exception { for (int i = 0
    void copyProgressValueToInactiveMonths(List<MeasureActPlan> measureActPlans)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureActPlans)
        throw new NotImplementedException();
    }
}
