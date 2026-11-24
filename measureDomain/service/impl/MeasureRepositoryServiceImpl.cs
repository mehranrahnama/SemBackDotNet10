using Measure = global::SemBackDotNet10.measureDomain.model.entity.Measure;
using MeasureRepository = global::SemBackDotNet10.measureDomain.model.entity.MeasureRepository;
using SearchDto = global::SemBackDotNet10.Base.model.dto.SearchDto;
using UserRolesHelpers = global::Common.utils.userRoles.UserRolesHelpers;
// Mechanical conversion from Java (service/impl/MeasureRepositoryServiceImpl.java). Complex bodies are marked with TODO.
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
using SemBackDotNet10.baseDomain.repository;
using MeasureUnitRepository = global::SemBackDotNet10.basicDomain.Repositories.IMeasureUnitRepository;
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
using global::Common.repository;
using global::Common.service.interf;
using global::Common.service.impl;


namespace SemBackDotNet10.measureDomain.service.impl;

/// <summary>معادل MeasureRepositoryServiceImpl.java — تزریق وابستگی‌ها با constructor injection انجام شده است.</summary>
public class MeasureRepositoryServiceImpl : IMeasureRepositoryService
{
    private readonly MeasureRepoRepository _measureRepoRepository;
    private readonly MeasureCatalogRepository _measureCatalogRepository;
    private readonly MeasureActPlanRepository _measureActPlanRepository;
    private readonly global::SemBackDotNet10.measureDomain.repository.MeasureRepository _measure_Repository;
    private readonly MeasureUnitRepository _measureUnitRepository;
    private readonly PlaningLevelRepository _planingLevelRepository;
    private readonly IMeasureCatalogService _measureCatalogService;
    private readonly IMeasureCatalogStatusLogService _measureCatalogStatusLogService;
    private readonly global::SemBackDotNet10.basicDomain.service.IUserService _userService;
    private readonly global::SemBackDotNet10.SecurityDomain.Service.Interf.IUserRoleService _userRoleService;
    private readonly MeasureCatalogRelationRepository _measureCatalogRelationRepository;
    private readonly HistoryService _historyService;
    private readonly CartableStatusService _cartableStatusService;
    private readonly global::SemBackDotNet10.service.impl.PlaningLevelServiceImpl _planingLevelService;
    private readonly IMeasureService _measureService;
    private readonly UserRolesHelpers _userRolesHelpers;
    private readonly global::SemBackDotNet10.Data.SemDbContext _entityManager;
    private readonly global::SemBackDotNet10.evaluationDomain.service.interf.IEvaluationMeasureService _evaluationMeasureService;
    private readonly MeasureCatalogDomainService _measureCatalogDomainService;
    private readonly MeasureCatalogRelationDomainService _measureCatalogRelationDomainService;

    private readonly CommentService _commentService;

    public MeasureRepositoryServiceImpl(CommentService commentService, MeasureRepoRepository measureRepoRepository, MeasureCatalogRepository measureCatalogRepository, MeasureActPlanRepository measureActPlanRepository, global::SemBackDotNet10.measureDomain.repository.MeasureRepository measure_Repository, MeasureUnitRepository measureUnitRepository, PlaningLevelRepository planingLevelRepository, IMeasureCatalogService measureCatalogService, IMeasureCatalogStatusLogService measureCatalogStatusLogService, global::SemBackDotNet10.basicDomain.service.IUserService userService, global::SemBackDotNet10.SecurityDomain.Service.Interf.IUserRoleService userRoleService, MeasureCatalogRelationRepository measureCatalogRelationRepository, HistoryService historyService, CartableStatusService cartableStatusService, global::SemBackDotNet10.service.impl.PlaningLevelServiceImpl planingLevelService, IMeasureService measureService, UserRolesHelpers userRolesHelpers, global::SemBackDotNet10.Data.SemDbContext entityManager, global::SemBackDotNet10.evaluationDomain.service.interf.IEvaluationMeasureService evaluationMeasureService, MeasureCatalogDomainService measureCatalogDomainService, MeasureCatalogRelationDomainService measureCatalogRelationDomainService)
    {
        _commentService = commentService;
        _measureRepoRepository = measureRepoRepository;
        _measureCatalogRepository = measureCatalogRepository;
        _measureActPlanRepository = measureActPlanRepository;
        _measure_Repository = measure_Repository;
        _measureUnitRepository = measureUnitRepository;
        _planingLevelRepository = planingLevelRepository;
        _measureCatalogService = measureCatalogService;
        _measureCatalogStatusLogService = measureCatalogStatusLogService;
        _userService = userService;
        _userRoleService = userRoleService;
        _measureCatalogRelationRepository = measureCatalogRelationRepository;
        _historyService = historyService;
        _cartableStatusService = cartableStatusService;
        _planingLevelService = planingLevelService;
        _measureService = measureService;
        _userRolesHelpers = userRolesHelpers;
        _entityManager = entityManager;
        _evaluationMeasureService = evaluationMeasureService;
        _measureCatalogDomainService = measureCatalogDomainService;
        _measureCatalogRelationDomainService = measureCatalogRelationDomainService;
    }

    // TODO(signature): public MeasureRepositoryServiceImpl( MeasureRepoRepository measureRepoRepository,
    // TODO(signature): MeasureCatalogRelationDomainService measureCatalogRelationDomainService ) { this.measureRepoRepository = measureRepoRepository; this.measureCatalogRepository = 
    CommentRepository getCommentRepository()
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — getCommentRepository
        throw new NotImplementedException();
    }
    HistoryRepository getHistoryRepository()
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — getHistoryRepository
        throw new NotImplementedException();
    }
    public CommentRepository GetCommentRepository() => _commentService.GetRepository();
    public HistoryRepository GetHistoryRepository() => _historyService.GetRepository();

    public MeasureRepoRepository GetRepository() => _measureRepoRepository;

    public MeasureRepoRepository getRepository() => GetRepository();
    public MeasureRepository findByIdOrElseThrow(long? id)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — id)
        throw new NotImplementedException();
    }
    public MeasureRepository findMeasurePeriod(long? id)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — id)
        throw new NotImplementedException();
    }
    public SearchDto findAll(string? searchFilter, int page, int size, string? orderBy, SortDirection direction)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — direction)
        throw new NotImplementedException();
    }
    public MeasureUnitDto getMeasureUnit(long? repositoryId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — repositoryId)
        throw new NotImplementedException();
    }
    // @Transactional
    public void delete(long? id)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — id)
        throw new NotImplementedException();
    }
    // @Transactional
    public MeasureRepository create_V2(MeasureRepositoryDto measureRepositoryDto, global::Microsoft.AspNetCore.Http.HttpRequest request)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — request)
        throw new NotImplementedException();
    }
    // create measureRepository and measureCatalog sub Services for create_V2 servide
    void checkMeasureCatalogNotExists(MeasureRepositoryDto measureRepositoryDto)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureRepositoryDto)
        throw new NotImplementedException();
    }
    MeasureRepository createMeasureRepository(MeasureRepositoryDto measureRepositoryDto)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureRepositoryDto)
        throw new NotImplementedException();
    }
    List<MeasureCatalog> createMeasureCatalogs(List<long?> levelIds, MeasureCatalogDto measureCatalogDto, long? measureRepositoryId, string? measureCatalogOutCome, global::Microsoft.AspNetCore.Http.HttpRequest request)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — request)
        throw new NotImplementedException();
    }
    void setMeasureCatalogConfirmStatus(MeasureCatalog catalog, MeasureCatalogDto measureCatalogDto, long? measureRepositoryId, string? measureCatalogOutCome, global::Microsoft.AspNetCore.Http.HttpRequest request)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — request)
        throw new NotImplementedException();
    }
    bool isCamundaAvailable(global::Microsoft.AspNetCore.Http.HttpRequest request)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — request)
        throw new NotImplementedException();
    }
    /**
    * Creates a new Camunda process instance and sets up the associated task variables
    * for a measure catalog validation task in a cartable. The method integrates with various
    * services, including user management, Camunda BPMN process management, and measure catalog
    * handling.
    * <p>
    * The process is started with the appropriate assignees and owners based on user roles,
    * and the task is then created and updated with necessary variables such as the requester's
    * name, assignee, owner, task URL, and request number.
    * <p>
    * The method also handles assigning the task to the specified user (tashilgar) if the task
    * status is "draft" and updates the associated MeasureCatalog with the process-related
    * information, including the case instance ID, process ID, and business key.
    *
    * @param measureRepositoryId   The ID of the MeasureRepository to retrieve the measure name.
    * @param measureCatalogId      The ID of the MeasureCatalog associated with the task.
    * @param planningLevelId       The ID of the PlanningLevel associated with the catalog
    * @param code                  The unique code associated with the measure or task (currently not used in logic).
    * @param taskUrl               The URL associated with the task in the workflow.
    * @param tashilgarId           The ID of the user assigned as the task assignee (tashilgar).
    * @param modirId               The ID of the user assigned as the task owner (modir).
    * @param tashilgarHamkarId     The ID of the user assigned as the collaborator (optional, can be null or zero).
    * @param measureCatalogOutCome The outcome of the measure catalog (used to determine task status, either "draft" or "submit").
    * @throws IllegalArgumentException If any required entities (e.g., MeasureRepository, MeasureCatalog, User) cannot be found.
    * @throws RuntimeException         If there are issues starting the Camunda process or updating task variables.
    */
    void createNewInstanceAndSetTaskVariables(long? measureRepositoryId, long? measureCatalogId, long? planningLevelId, string? code, string? taskUrl, long? tashilgarId, long? modirId, long? tashilgarHamkarId, string? measureCatalogOutCome)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureCatalogOutCome)
        throw new NotImplementedException();
    }
    MeasureCatalog updateCatalogAndSetCamundaVariables(long? measureCatalogId, string? caseInstanceId, string? definitionKey, IActionResult processInstance, string? businessKey)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — businessKey)
        throw new NotImplementedException();
    }
    void createAddToSubLevelRelations(List<MeasureCatalog> measureCatalogs, MeasureRepositoryDto measureRepositoryDto)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureRepositoryDto)
        throw new NotImplementedException();
    }
    void createMeasureCatalogRelation(MeasureCatalog baseMeasureCatalog, MeasureCatalog relatedMeasureCatalog, long? planId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — planId)
        throw new NotImplementedException();
    }
    void createAddToSupremeComMeasureCatalogs(List<long?> levelIds, MeasureRepositoryDto measureRepositoryDto, long? measureRepositoryId, string? measureCatalogOutCome, global::Microsoft.AspNetCore.Http.HttpRequest request)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — request)
        throw new NotImplementedException();
    }
    void addUserRole(MeasureCatalogDto measureCatalogDto)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureCatalogDto)
        throw new NotImplementedException();
    }
    void addPlaningLevelAccess(MeasureCatalogDto measureCatalogDto)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureCatalogDto)
        throw new NotImplementedException();
    }
    //    @Override
    //    @Transactional(propagation = Propagation.REQUIRES_NEW, rollbackFor = Exception.class)
    //    public MeasureRepository edit(MeasureRepositoryDto measureRepositoryDto) throws Exception {
    //        semLogger.info("درخواست ویرایش شاخص با عنوان : " + measureRepositoryDto.getMeasureName() + " ارسال شد.");
    //        Assert.notNull(measureRepositoryDto, "measureRepositoryDto is null");
    //        MeasureRepository measureRepository = findByIdOrElseThrow(measureRepositoryDto.getId());
    //
    //        String supreme_committee_prefix = message_fa_Source.getMessage("supreme_committee_prefix", null, LocaleUtils.getInstance());
    //
    //        boolean isAddToSubLevel_DB = measureRepository.isAddToSubLevels();
    //
    //        //measureRepository.setCodKpiEpm(measureRepositoryDto.getCodKpiEpm());
    //        measureRepository.setMeasureName(measureRepositoryDto.getMeasureName());
    //        measureRepository.setDesiredMethod(measureRepositoryDto.getDesiredMethod());
    //        measureRepository.setMeasureType(measureRepositoryDto.getMeasureType());
    //        measureRepository.setConfirmstatus(measureRepositoryDto.getMeasureConfirmStatus());
    //        measureRepository.setMeasurePayeshType(measureRepositoryDto.getMeasurePayeshType());
    //        //measureRepository.setAddToSubLevels(measureRepositoryDto.isAddToSubLevels());
    //
    //        long unitId = measureRepositoryDto.getMeasureUnitId();
    //        Optional<MeasureUnit> mu = measureUnitRepository.findById(unitId);
    //        if (mu.isPresent())
    //            measureRepository.setMeasureUnit(mu.get());
    //
    //        MeasureCatalogDto catalogDto = measureRepositoryDto.getMeasureCatalogDto();
    //
    //        measureRepository = saveWithoutHistory(measureRepository);
    //
    //        catalogDto.setMeasureRepositoryId(measureRepositoryDto.getId());
    //        MeasureCatalog catalog = measureCatalogService.findById(catalogDto.getId()).get();
    //        List<Measure> measures = measureService.getRepository().findAllByMeasureCatalogIdAndDeletedIsFalse(catalog.getId());
    //
    //
    //        catalog.setWorstMin(catalogDto.getWorstMin());
    //        catalog.setTargetTheory(catalogDto.getTargetTheory());
    //        catalog.setZeroTarget(catalogDto.isZeroTarget());
    //        catalog.setZeroInitial(catalogDto.isZeroInitial());
    //        catalog.setDataType(catalogDto.getDataType());
    //        catalog.setMeasurePeriod(catalogDto.getMeasurePeriod());
    //        catalog.setParentId(catalogDto.getParentId());
    //        catalog.setDeleted(catalogDto.isDeleted());
    //        catalog.setConfirmstatus(catalogDto.getMeasureConfirmStatus());
    //        catalog.setMeasureMaxValue(catalogDto.getMeasureMaxValue());
    //        catalog.setMeasureStatus(catalogDto.getMeasureStatus());
    //        catalog.setOriginId(catalogDto.getOriginId());
    //
    //        if (catalogDto.getMeasureRepositoryId() != null) {
    //            MeasureRepository mr = measureRepoRepository.getReferenceById(catalogDto.getMeasureRepositoryId());
    //            catalog.setMeasureRepository(mr);
    //        }
    //        if (catalogDto.getModirId() != null) {
    //            User modir = userService.findById(catalogDto.getModirId()).get();
    //            catalog.setModir(modir);
    //            userRoleService.addModirUserRole(modir.getId());
    //            measures.stream().forEach(a -> a.setModir(modir));
    //
    //        }
    //        if (catalogDto.getTashilgarId() != null) {
    //            User tashilgar = userService.findById(catalogDto.getTashilgarId()).get();
    //            catalog.setTashilgar(tashilgar);
    //            measures.stream().forEach(a -> a.setTashilgar(tashilgar));
    //        }
    //        if (catalogDto.getTashilgarHamkarId() != null) {
    //            User tashilgarHamkar = userService.findById(catalogDto.getTashilgarHamkarId()).get();
    //            catalog.setTashilgarHamkar(tashilgarHamkar);
    //        }
    //
    //        boolean isPlanningLevelChanged = catalogDto.getPlaningLevelId() != null &&
    //                catalog.getPlaningLevel().getId() != null &&
    //                !catalogDto.getPlaningLevelId().equals(catalog.getPlaningLevel().getId());
    //
    //        if (isPlanningLevelChanged) {
    //            try{
    //                handlePlanningLevelChanging(catalog, catalogDto.getPlaningLevelId());
    //            } catch (BusinessException e) {
    //                throw e;
    //            } catch(Exception e){
    //                throw new BusinessException("خطایی در فرآیند تغییر کمیته برنامه ریزی رخ داده است");
    //            }
    //        }
    //
    //        if (catalogDto.getPlaningLevelId() != null) catalog.setPlaningLevel(
    //                entityManager.getReference(PlaningLevel.class, catalogDto.getPlaningLevelId())
    //        );
    //
    //        measureCatalogRepository.save(catalog);
    //
    //        if (measureRepositoryDto.isPlanChanged()) {
    //            List<Measure> measureList = measure_Repository.findAllByMeasureCatalogId(catalog.getId());
    //            if (measureList != null && measureList.size() > 0) {
    //                for (Measure measure : measureList) {
    //                    measure.setPlanned(false);
    //                    List<MeasureActPlan> measureActPlans = measureActPlanRepository.findByMeasure_Id(measure.getId());
    //
    //                    List<Long> measureActPlanIds = measureActPlans.stream().map(MeasureActPlan::getId).toList();
    //                    evaluationMeasureService.deleteEvaluationsForMeasureActPlanIds(measureActPlanIds);
    //                    measureActPlanRepository.deleteAll(measureActPlans);
    //                }
    //                measure_Repository.saveAll(measureList);
    //            }
    //        }
    //
    //        if (catalog.getTashilgar() != null && catalog.getTashilgar().getId() != null) {
    //            userRoleService.addTashilgarUserRole(catalog.getTashilgar().getId());
    //        }
    ////        if (catalog.getModir() != null && catalog.getModir().getId() != null) {
    ////            userRoleService.addModirUserRole(catalog.getModir().getId());
    ////        }
    //        semLogger.info("ویرایش شاخص با عنوان : " + measureRepositoryDto.getMeasureName() + " با موفقیت انجام شد.");
    //        return measureRepository;
    //    }
    // @Transactional(propagation = Propagation.REQUIRES_NEW, rollbackFor = Exception.class)
    public MeasureRepository edit(MeasureRepositoryDto dto)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — dto)
        throw new NotImplementedException();
    }
    void validate(MeasureRepositoryDto dto)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — dto)
        throw new NotImplementedException();
    }
    MeasureRepository loadMeasureRepository(MeasureRepositoryDto dto)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — dto)
        throw new NotImplementedException();
    }
    // TODO(signature): MeasureRepositoryDto dto) { repository.setMeasureName(dto.getMeasureName()); repository.setDesiredMethod(dto.getDesiredMethod()); repository.setMeasureType(dto.
    // TODO(signature): MeasureRepositoryDto dto) { MeasureCatalogDto catalogDto = dto.getMeasureCatalogDto(); catalogDto.setMeasureRepositoryId(repository.getId()); MeasureCatalog cat
    // TODO(signature): MeasureCatalogDto dto) { catalog.setWorstMin(dto.getWorstMin()); catalog.setTargetTheory(dto.getTargetTheory()); catalog.setZeroTarget(dto.isZeroTarget()); cata
    // TODO(signature): MeasureCatalogDto dto, List<Measure> measures) { updateModir(catalog, dto, measures); updateTashilgar(catalog, dto, measures); updateTashilgarHamkar(catalog, dt
    // TODO(signature): MeasureCatalogDto dto, List<Measure> measures) { if (dto.getModirId() == null) return; User modir = userService.findById(dto.getModirId()).get(); catalog.setMod
    // TODO(signature): MeasureCatalogDto dto, List<Measure> measures) { if (dto.getTashilgarId() == null) { return; } User tashilgar = userService.findById(dto.getTashilgarId()).get()
    // TODO(signature): MeasureCatalogDto dto) { if (dto.getTashilgarHamkarId() == null) { return; } User tashilgarHamkar = userService.findById(dto.getTashilgarHamkarId()).get(); cata
    // TODO(signature): MeasureRepositoryDto dto) { MeasureCatalogDto catalogDto = dto.getMeasureCatalogDto(); boolean changed = catalogDto.getPlaningLevelId() != null && catalog.getPl
    // TODO(signature): MeasureRepositoryDto dto) { if (!dto.isPlanChanged()) return; List<Measure> measures = measure_Repository.findAllByMeasureCatalogId(catalog.getId()); if (measur
    void resetMeasurePlan(Measure measure)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measure)
        throw new NotImplementedException();
    }
    void assignRoles(MeasureCatalog catalog)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — catalog)
        throw new NotImplementedException();
    }
    // @Transactional(propagation = Propagation.MANDATORY)
    void handlePlanningLevelChanging(MeasureCatalog measureCatalog, long? levelId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — levelId)
        throw new NotImplementedException();
    }
    //  MeasureRepository editMeasureRepositoryfromSanayeToNahaeeState(MeasureRepositoryDto measureRepositoryDto);
    public MeasureRepositoryDto mapToDto(MeasureRepository measureRepository)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureRepository)
        throw new NotImplementedException();
    }
    public MeasureRepository mapToEntity(MeasureRepositoryDto measureRepositoryDto)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureRepositoryDto)
        throw new NotImplementedException();
    }
    public MeasureRepositoryDto getMeasureRepositoryDto(long? id)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — id)
        throw new NotImplementedException();
    }
    public MeasureRepositoryDto findMeasureRepositoryById(long? measureRepositoryId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureRepositoryId)
        throw new NotImplementedException();
    }
    public MeasureRepositoryDto findMeasureRepositoryByMeasureCatalogId(long? MeasureCatalogId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — MeasureCatalogId)
        throw new NotImplementedException();
    }
    MeasureRepositoryDto findMeasureCatalogInfoStatusLogByMeasureCatalogId(long? MeasureCatalogId, long? MeasureConfirmStatus)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — MeasureConfirmStatus)
        throw new NotImplementedException();
    }
    public string? getAllMeasureRepositories_reportDataSet(string? filter)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — filter)
        throw new NotImplementedException();
    }
    public bool hasMeasureActPlan(long? measureRepositoryId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureRepositoryId)
        throw new NotImplementedException();
    }
    public MeasureRepository findOrInsertProcessMeasureRepository(System.Text.Json.Nodes.JsonObject measureProcess, MeasureUnit measureUnit)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureUnit)
        throw new NotImplementedException();
    }
    public void updateDesiredMethodAndPayeshType(int desiredMethod, byte measurePayeshType, long? id)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — id)
        throw new NotImplementedException();
    }
    public void restoreMeasureRepositoryFromLog(int desiredMethod, byte measurePayeshType, long? measureId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureId)
        throw new NotImplementedException();
    }
    public bool hasDesiredMethodOrPayeshTypeChanged(long? measureId, byte measurePayeshType, int desiredMethod)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — desiredMethod)
        throw new NotImplementedException();
    }
}
