using Measure = global::SemBackDotNet10.measureDomain.model.entity.Measure;
using global::SemBackDotNet10.Common.Pagination;
// Mechanical conversion from Java (repository/MeasureActPlanRepository.java). Complex bodies are marked with TODO.
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


namespace SemBackDotNet10.measureDomain.repository;

/// <summary>معادل MeasureActPlanRepository.java (اینترفیس جاوا) — در C# با پیشوند I.</summary>
public interface MeasureActPlanRepository : global::SemBackDotNet10.Base.repository.ISemRepository<MeasureActPlan,long?>
{
    // TODO(signature): "order by  act.month asc ") Page<MeasureActPlanDto> findAllBydimMeasureId(@Param("dimMeasureId") Long dimMeasureId, Pageable pageable);
    PagedResult<MeasureActPlanDto> findCustomMeasureActPlanBydimMeasureId(long? dimMeasureId, Pageable pageable);
    PagedResult<MeasureActPlanDto> findCustomActiveMeasureActPlanByDimMeasureId(long? dimMeasureId, Pageable pageable);
    // TODO(signature): "order by  act.month asc ") Page<MeasureActPlanDto> findAllByDimMeasureIdAndDeletedIsTrue(@Param("dimMeasureId") Long dimMeasureId, Pageable pageable);
    // TODO(signature): "order by  act.month asc ") Page<MeasureActPlanDto> findAllCustomMeasureActByDimMeasureIdAndDeletedIsTrue(@Param("dimMeasureId") Long dimMeasureId, Pageable pag
    DetailedMeasureCatalogDto findAllBydimMeasureIdWithDetailes(long? dimMeasureId);
    // @Query(value = "select act from MeasureActPlan act  left join act.measure mes  left join mes.measureCatalog catalog where catalog.id =:measureCatalogId")
    List<MeasureActPlan> findAllByMeasureCatalogId(long? measureCatalogId);
    // TODO(signature): "order by  act.month asc ") List<MeasureActPlanDto> findAllByMeasureId(@Param("measuregId") Long measuregId);
    // TODO(signature): "order by  act.month asc ") List<MeasureActPlanDto> findAllMeasureActPlanDtos();
    // @Query(value = "select measureActPlanCalc from MeasureActPlanCalc measureActPlanCalc  where measureActPlanCalc.measureActPlan.id =:measureActPlanId")
    List<MeasureActPlanCalc> findMeasureActPlanCalcByMeasureActPlanId(long? measureActPlanId);
    // TODO(signature): );
    // TODO(signature): );
    List<MeasureActPlan> findByMeasure_Id(long? id);
    List<MeasureActPlan> findAllByMeasureAndDeletedIsFalse(Measure measure);
    List<MeasureActPlan> findAllByMeasureAndDeletedIsTrue(Measure measure);
    MeasureProgressDto findMeasureProgressByMeasureActPlanId(long? measureActPlanId);
    // MeasureProgressDto findMeasureProgressByMeasureActPlanId(Long measureActPlanId);
    List<MeasureActPlan> findAllByMeasureIdAndDeletedIsFalse(long? measureId);
    List<MeasureActPlan> findByMeasureIdAndMonthBetweenAndDeletedIsFalse(long? measureId, long? fromMonth, long? toMonth);
    // @Query(value = "select m from MeasureActPlan m where m.measure.measureCatalog.id in :measureCatalogIds and m.deleted = false")
    List<MeasureActPlan> findAllByMeasureCatalogIds(List<long?> measureCatalogIds);
    // TODO(signature): );
    MeasureActPlan findByMeasureIdAndMonthAndDeletedIsFalse(long? measureId, long? month);
    //    @Query(value = "select [dbo].[fn_CalcMeasurePCNNewByMeasureActPlanId](:measureActPlanId)  ",nativeQuery = true))
    // @Query(value = "exec [dbo].[sp_CalcMeasurePCNNewByMeasureActPlanId] :measureActPlanId ",nativeQuery = true)
    decimal? calcMeasurePCN(long? measureActPlanId);
    // @Query(value = "select act.month from MeasureActPlan act where act.measure.id = :measureId and act.dailyValue is not null")
    List<long?> getProgressActiveMonth(long? measureId);
    // TODO(signature): "WHERE act.confirmstatus = :confirmStatus " + "AND act.deleted = false " + "AND act.dailyValue IS NOT NULL " + "AND act.actualValue IS NOT NULL " + "and act.tas
    // TODO(signature): DELETE FROM MeasureActPlan map WHERE map.measure.id = :id """) DELETE FROM MeasureActPlan map WHERE map.measure.id = :id
    // TODO(signature): """) void deleteAllByMeasureId(Long id);
    void softDeleteItemsByMeasureId(long? measureId);
    void deleteExpiredItemsByMeasureId(long? measureId);
    // TODO(signature): Pageable pageable );
    // TODO(signature): """) Optional<List<MeasureActPlan>> findAllByMeasureIdAndDeletedIsTrueSorted(@Param("measureId") Long measureId);
    // TODO(signature): """) Optional<List<MeasureActPlan>> findAllByMeasureIdAndDeletedIsFalseSorted(@Param("measureId") Long measureId);
    // TODO(signature): """) long countActivePlansByMeasureId(@Param("measureId") Long measureId);
    // TODO(signature): """) void restoreByMeasureId(@Param("measureId") Long measureId);
    void deleteAllByMeasureIdAndDeletedFalse(long? measureId);
    List<long?> findIdsByMeasureIdAndDeletedFalse(long? measureId);
    List<MeasureActPlan> findAllByMeasureIdAndDeletedFalse(long? measureId);
    // TODO(signature): );
    // TODO(signature): );
    List<MeasureActPlan> findAllWithTaskUrlIsNotNull();
    List<projection.IKeyValueView> findAllWithNullActualValue();
    List<MeasureActPlan> findAllByMeasureIdsAndMonth(List<long?> measureIds, long? month);
    // TODO(signature): """) void restoreByMeasureCatalogId(@Param("catalogId") Long catalogId);
}
