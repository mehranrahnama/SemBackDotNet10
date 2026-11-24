// Mechanical conversion from Java (repository/MeasureCatalogRepository.java). Complex bodies are marked with TODO.
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

/// <summary>معادل MeasureCatalogRepository.java (اینترفیس جاوا) — در C# با پیشوند I.</summary>
public interface MeasureCatalogRepository : global::SemBackDotNet10.Base.repository.ISemRepository<MeasureCatalog,long?>
{
    // TODO(signature): " where m.id = :dimMeasureId and m.planPeriod.planId = :planId and mc.planingLevel.id = :levelId" ) " where m.id = :dimMeasureId and m.planPeriod.planId = :plan
    MeasureCatalog findMeasureCatalogByMeasureIdAndPlanIdAndLevelId(long? dimMeasureId, long? planId, long? levelId);
    long? findIdByRepositoryIdAndLevelId(long? measureRepositoryId, long? levelId);
    // @Query(value = "select c from MeasureCatalog c where c.measureRepository.id=:measureRepositoryId and c.deleted=false")
    List<MeasureCatalog> findByMeasureRepositoryId(long? measureRepositoryId);
    // @Query(value = "select c from MeasureCatalog c where c.measureRepository.id=:measureRepositoryId and c.deleted=false and c.planingLevel.id=:levelId")
    List<MeasureCatalog> findByMeasureRepositoryIdAndPlaningLevelId(long? measureRepositoryId, long? levelId);
    List<long?> getCurrentUserLevelIds(long? userId);
    MeasureCatalog findByMeasureRepositoryIdAndPlaningLevel_Id(long? measureRepositoryId, long? planingLevelId);
    // @Query(value = "select m.id from MeasureCatalog m where m.measureRepository.id = :measureRepositoryId")
    List<long?> findMeasureCatalogIdsByMeasureRepositoryId(long? measureRepositoryId);
    MeasureCatalog findByCode(string? measureCatalogCode);
    List<MeasureCatalog> findByMeasureNameAndLevelIdAndDeletedIsFalse(string? MeasureName, List<long?> levelIds);
    // @Query(value = "select mc from MeasureCatalog mc where mc.modir.id = :oldModirId and mc.deleted = false")
    List<MeasureCatalog> findByModirIdAndDeletedIsFalse(long? oldModirId);
    // @Query(value = "select mc from MeasureCatalog mc where mc.tashilgar.id = :oldTashilgarId and mc.deleted = false")
    List<MeasureCatalog> findByTashilgarIdAndDeletedIsFalse(long? oldTashilgarId);
    // TODO(signature): ) Set<User> findAllModirByLevelId(Long levelId , String filter);
    List<MeasureCatalog> findByConfirmStatusAndDeletedIsFalseAndConfirmStatusIgnoreCase(long? confirmStatus, List<long?> ignoreConfirmStatus, string? assigneeRole, string? prefix, string? filter);
    List<MeasureCatalog> findByConfirmStatusAndDeletedIsFalse(long? confirmStatus, string? assigneeRole, string? prefix, string? filter);
    // TODO(signature): (Long confirmStatus, List<Long> ignoreConfirmStatus , @Param("assigneeRole") String assigneeRole , String prefix, String filter);
    // TODO(signature): (Long confirmStatus , @Param("assigneeRole") String assigneeRole , String prefix, String filter);
    long? findConfirmStatusById(long? id);
    // TODO(signature): );
    // TODO(signature): );
    int findMeasurePeriodByMeasureId(long? measureId);
    // TODO(signature): );
    // TODO(signature): );
    List<MeasureCatalog> findAllWithTaskUrlIsNotNull();
    // TODO(signature): );
    List<MeasureCatalog> findAllByIds(List<long?> catalogIds);
    // TODO(signature): List<MeasureCatalogDto> findAllMeasureDetailByLevelIdAndMeasureType(
    // TODO(signature): List<MeasureCatalogDto> findAllMeasureDetailByLevelId(
    // TODO(signature): """) List<String> findAllDesLevelsByIds(@Param("catalogIds") Set<Long> catalogIds);
}
