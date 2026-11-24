using MeasureRepository = global::SemBackDotNet10.measureDomain.model.entity.MeasureRepository;
// Mechanical conversion from Java (service/interf/IMeasureService.java). Complex bodies are marked with TODO.
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

using SemBackDotNet10.Common.enums;
using SemBackDotNet10.Base.Service.Inter;
using SemBackDotNet10.measureDomain.controller.dto.measureDtos;
using SemBackDotNet10.Base.model.dto;



namespace SemBackDotNet10.measureDomain.service.interf;

/// <summary>معادل IMeasureService.java (اینترفیس جاوا) — در C# با پیشوند I.</summary>
public interface IMeasureService : IProcessService<Measure,long?,global::SemBackDotNet10.measureDomain.repository.MeasureRepository>
{
    Measure findByIdOrElseThrow(long? id);
    // TODO(signature): );
    MeasureDto mapToDto(Measure measure);
    Measure create(RequestMeasureDto measureDto);
    Measure edit(RequestMeasureDto measureDto);
    List<Measure> findMeasureByMeasureCatalogId(long? MeasureCatalogId);
    MeasureDto findMeasureDtoByMeasureCatalogId(long? PlanId, long? MeasureCatalogId);
    void editIsReplannedMeasure(long? measureCatalogId);
    void delete(long? id);
    void editAllWeights(List<MeasureDto> measureDtos);
    SearchDto loadObjectGrid(LoadStrategyObjectDto body, int page, int size, string? orderBy, SortDirection direction);
    void editDeleteStatus(string? body);
    Dictionary<string?,object?> getCounts(string? body);
    MeasureDto findMeasureById(long? id);
    void editConfirmStatus(long? id, string? outCome);
    long? CountActiveMeasures(long? planId);
    long? CountActiveMeasuresByPlanIdLevelId(long? planId, long? levelId);
    bool expireMeasureTasks(long? tenantId);
    Measure measureReplanConfirm(MeasureDto measureDto);
    void measureReplanReject(long? measureId);
    void changeTashilgar(long? oldTashilgarId, long? newTashilgarId, long? planId);
    void changeModir(long? oldModirId, long? newModirId, long? planId);
    // TODO(signature): );
    void hardDeleteMeasure(List<StrategyObject> deletedStgList);
    void assignFacilitatorAndManager(long? planningLevelId, long? facilitatorId, long? managerId);
    void updatePlannedById(long? measureId, bool planned);
    List<Measure> getMeasureByParentLevelAndPlanId(long? planId);
    Measure getFirstByMeasureCatalog(long? measureCatalogId);
    void updateYearTargetByMeasure(decimal? yearTarget, long? measureId);
    bool hasPreviousReplanNotConfirmed(long? measureId);
    MeasureReplanLogCreationDto findMeasureReplanLogCreationDto(long? measureId);
    //    void sendToUpperPlanningLevels(Measure measure, Long explicitUpperLevelStrategyObjectId, boolean isEditFlow);
    void sendToUpperPlanningLevels(Measure measure, long? explicitUpperLevelStrategyObjectId, bool isEditFlow, UpperLevelsSendMode sendMode);
    Measure copyMeasureForUpperLevel(long? ancestorCatalogId, Measure measure);
    List<Measure> findMeasuresByMeasureCatalogId(List<long?> ancestorIds, long? planId);
    List<Measure> findAllWithTaskUrlIsNotNull();
    void setStrategyObjectWithNewPlanningLevel(PlaningLevel planingLevel, long? catalogId);
    void updateAddToSupremeLevelByCatalog(long? catalogId, bool addToSupremeLevel);
    void updateAddToSupremeLevelByCatalog(long? catalogId, long? planId, bool addToSupremeLevel);
    string? findMeasureNameByMeasureId(long? measureId);
}
