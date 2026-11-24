using Measure = global::SemBackDotNet10.measureDomain.model.entity.Measure;
// Mechanical conversion from Java (repository/MeasureRepository.java). Complex bodies are marked with TODO.
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
using SemBackDotNet10.SecurityDomain.Infrastructure.Persistence;
using global::Common.repository;
using global::Common.service.interf;
using global::Common.model.dto;
using global::Common.controller.dto;
using SemBackDotNet10.Common.enums;
using SemBackDotNet10.Common.exception;
using global::Common.messages;
using global::Common.service.impl;
using SemBackDotNet10.Base.repository;


namespace SemBackDotNet10.measureDomain.repository;

/// <summary>معادل MeasureRepository.java (اینترفیس جاوا) — در C# با پیشوند I.</summary>
public interface MeasureRepository : global::SemBackDotNet10.Base.repository.ISemRepository<Measure,long?>
{
    // TODO(signature): );
    // @Query(value = "select measure from Measure measure where measure.measureCatalog.id=:measureCatalogId ")
    List<Measure> findAllByMeasureCatalogId(long? measureCatalogId);
    List<Measure> findAllByMeasureCatalogIdAndDeletedIsFalse(long? measureCatalogId);
    // TODO(signature): "and measure.deleted = false  ") List<Measure> findAllByMeasureCatalogIdAndPlanPeriodId(@Param("measureCatalogId") Long measureCatalogId , @Param("planId") Long
    // TODO(signature): value = """ SELECT NEW com.irisaco.mss.sem.measureDomain.model.dto.MeasureObjectDto( o.id, o.title, ( SELECT COUNT(m.id) FROM Measure m LEFT JOIN m.measureCatal
    // TODO(signature): );
    // TODO(signature): );
    // TODO(signature): "from Measure m " + "left join m.strategyObject o " + "left join m.measureCatalog c " + "left join m.planPeriod pp " + "left join c.planingLevel pl " + "left jo
    // TODO(signature): "from Measure m " + "left join m.strategyObject o " + "left join m.measureCatalog c left join c.measureRepository mr " + "left join m.planPeriod pp " + "left jo
    // @Query(value = "select m from Measure m where m.strategyObject.id=:objectId")
    List<Measure> findAllByObjectId(long? objectId);
    // TODO(signature): ) MeasureDto findMeasureById(@Param("id") Long id);
    // @Query(value = "select m from Measure m where m.measureCatalog.id=:MeasureCatalogId")
    List<Measure> findAllMeasureByMeasureCatalogId(long? MeasureCatalogId);
    // TODO(signature): ) MeasureDto findMeasureDtoByMeasureCatalogId(@Param("PlanId") Long PlanId,@Param("MeasureCatalogId") Long MeasureCatalogId);
    // @Query(value = "exec  [dbo].[CountActiveMeasures] :planId "   , nativeQuery = true)
    long? CountActiveMeasures(long? planId);
    // @Query(value = "exec  [dbo].[CountActiveMeasuresByPlanIdLevelId] :planId , :levelId"   , nativeQuery = true)
    long? CountActiveMeasuresByPlanIdLevelId(long? planId, long? levelId);
    Measure findFirstByMeasureCatalog_Id(long? measureCatalogId);
    // @Query(value = "select m from Measure m where m.measureCatalog.planingLevel.parent = null and m.planPeriod.planId = :planId")
    List<Measure> findEvolutionCommitteeMeasure(long? planId);
    // @Query(value = "select m from Measure m where m.strategyObject.id = :strategyObjectId and m.deleted = false")
    List<Measure> findByStrategyObjectIdAndDeletedIsFalse(long? strategyObjectId);
    Measure findByMeasureCatalogIdAndStrategyObjectIdAndDeletedIsTrue(long? measureCatalogId, long? strategyObjectId);
    // @Query(value = "select m from Measure m where m.tashilgar.id = :tashilgarId and m.planPeriod.planId = :planId and m.deleted = false")
    List<Measure> findByTashilgarIdAndPlanIdAndDeletedIsFalse(long? tashilgarId, long? planId);
    // @Query(value = "select m from Measure m where m.modir.id = :modirId and m.planPeriod.planId = :planId and m.deleted = false")
    List<Measure> findByModirIdAndPlanIdAndDeletedIsFalse(long? modirId, long? planId);
    // @Query(value = "SELECT m FROM Measure m WHERE m.confirmstatus not in (0,20)")
    List<Measure> inConfirmProcessMeasure();
    // TODO(signature): (@Param("confirmStatus") Long confirmStatus, @Param("ignoredConfirmStatus") List<Long> ignoredConfirmStatus ,
    // TODO(signature): (@Param("confirmStatus") Long confirmStatus, @Param("planId") Long planId ,
    // TODO(signature): (@Param("confirmStatus") Long confirmStatus, @Param("ignoredConfirmStatus") List<Long> ignoredConfirmStatus ,
    // TODO(signature): (@Param("confirmStatus") Long confirmStatus,
    // TODO(signature): "m.confirmstatus = :confirmStatus " + "AND m.deleted = true " + "and m.taskUrl is not null " + "AND m.confirmstatus not in :ignoredConfirmStatus  and m.planPeri
    // @Query(value = "select m from Measure m where m.strategyObject.id in :strategyObjectIds and m.deleted = true ")
    List<Measure> findAllByStrategyObjectIdAndDeletedIsTrue(List<long?> strategyObjectIds);
    bool checkIsNotPlannedById(long? id);
    void updatePlannedById(long? measureId, bool planned);
    // TODO(signature): );
    // TODO(signature): );
    long? findReplannedStatusByMeasureId(long? measureId);
    // TODO(signature): );
    // TODO(signature): );
    // TODO(signature): """) Measure findByMeasureCatalogIdAndDeletedFalse(@Param("catalogId") Long catalogId , @Param("planId") Long planId);
    List<Measure> findAllByMeasureCatalogIds(List<long?> ancestorIds);
    // TODO(signature): );
    List<long?> findAllIdsByMeasureCatalogIds(List<long?> ancestorIds);
    List<Measure> findAllWithTaskUrlIsNotNull();
    void updateAddToSupremeLevelById(long? measureId, bool addToSupremeLevel);
    // TODO(signature): """) Long findIdByMeasureCatalogId(@Param("catalogId") Long catalogId);
    bool existsByMeasureCatalogId(long? catalogId, long? planId);
    string? findMeasureNameByMeasureId(long? measureId);
    // TODO(signature): """) List<Long> findAllByMeasureRepositoryId(@Param("repositoryId") Long repositoryId);
    void restoreByCatalogId(long? measureIds);
    // TODO(signature): );
    // TODO(signature): );
    // TODO(signature): );
    // TODO(signature): );
    bool existsByMeasureCatalogId(long? catalogId);
}
