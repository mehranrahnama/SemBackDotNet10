using global::SemBackDotNet10.Common.Pagination;
// Mechanical conversion from Java (repository/MeasureCatalogRelationRepository.java). Complex bodies are marked with TODO.
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

/// <summary>معادل MeasureCatalogRelationRepository.java (اینترفیس جاوا) — در C# با پیشوند I.</summary>
public interface MeasureCatalogRelationRepository : global::SemBackDotNet10.Base.repository.ISemRepository<MeasureCatalogRelation,long?>
{
    // @Query(value = "select relation from MeasureCatalogRelation relation where relation.relatedMeasureCatalog.id=:relatedCatalogId ")
    List<MeasureCatalogRelation> findByRelatedCatalogId(long? relatedCatalogId);
    List<MeasureCatalogRelation> findAllByMainMeasureCatalog_Id(long? mainMeasureCatalogId);
    PagedResult<MeasureCatalogRelationDto> findByMainCatalogId(long? PlanId, long? mainRelId, Pageable pageable, string? param);
    // TODO(signature): " ") List<MeasureCatalogRelationDto> findmainMeasureCatalogByLevelIdAndPlanId(@Param("levelId") Long levelId, @Param("PlanId") Long PlanId , @Param("param") Str
    List<MeasureCatalogRelationDto> findByMainCatalogIdAndRelatedMeasure(long? mainRelId, long? relatedMeasure);
    List<MeasureCatalogRelation> findByMainMeasureCatalog(MeasureCatalog mainMeasureCatalog);
    List<MeasureCatalogRelation> findByMainMeasureCatalogOrRelatedMeasureCatalog(MeasureCatalog mainMeasureCatalog, MeasureCatalog relatedMeasureCatalog);
    void deleteAllByMainMeasureCatalogIdOrRelatedMeasureCatalogId(HashSet<long?> catalogIds);
    // TODO(signature): );
    void deleteByRelatedMeasureCatalogId(long? relatedCatalogId);
    List<long?> findParentIds(long? childId, long? activePlanId);
    int deleteAllByMainMeasureCatalogId(long? catalogId);
    // TODO(signature): );
    bool existsByMainMeasureCatalogIdOrRelatedMeasureCatalogId(long? mainMeasureCatalogId, long? relatedMeasureCatalogId);
    // TODO(signature): """) List<MeasureCatalog> findChildIds(@Param("catalogId") Long catalogId , @Param("activePlanId") Long activePlanId );
    // TODO(signature): );
    void deleteByMainMeasureCatalogId(long? mainMeasureCatalogId);
    // TODO(signature): );
    // TODO(signature): );
    // TODO(signature): );
    // TODO(signature): );
    // TODO(signature): );
    // TODO(signature): );
}
