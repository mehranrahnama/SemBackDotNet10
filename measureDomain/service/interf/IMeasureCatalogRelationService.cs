// Mechanical conversion from Java (service/interf/IMeasureCatalogRelationService.java). Complex bodies are marked with TODO.
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
using SemBackDotNet10.Common.exception;

using SemBackDotNet10.Base.Service.Inter;
using SemBackDotNet10.Base.model.dto;



namespace SemBackDotNet10.measureDomain.service.interf;

/// <summary>معادل IMeasureCatalogRelationService.java (اینترفیس جاوا) — در C# با پیشوند I.</summary>
public interface IMeasureCatalogRelationService : IBaseService<MeasureCatalogRelation,long?,MeasureCatalogRelationRepository>
{
    SearchDto findByMainCatalogId(long? mainRelId, string? filter, int page, int size, string? orderBy, SortDirection direction);
    SearchDto getmainMeasureCatalogByLevelIdAndPlanId(long? levelId, long? planId, string? filter, int page, int size, string? orderBy, SortDirection direction);
    void editMeasureCatalogRelation(List<MeasureCatalogRelationDto> measureCatalogRelationDto);
    MeasureCatalogRelation findByIdOrElseThrow(long? id);
    void delete(long? id);
    void create(MeasureCatalogRelationDto measureCatalogRelationDto);
    List<MeasureCatalogRelation> findByMainMeasureCatalog(MeasureCatalog mainMeasureCatalog);
    // TODO(signature): Long planId);
    void deleteByRelatedCatalogId(long? relatedCatalogId, long? planId);
    void createRelationForPlaningLevel(IReadOnlyList<MeasureCatalog> catalogs,
        IReadOnlyList<global::SemBackDotNet10.basicDomain.Model.Entity.PlaningLevel> levels, long planId);
    List<long?> findAncestorIds(long? id);
    List<long?> findAncestorIds(long? leafId, long? planId);
    void deleteRelationsForMeasureCatalog(long? catalogId);
    HashSet<MeasureCatalog> findConnectedMeasureCatalogs(long? catalogId);
    HashSet<MeasureCatalog> findConnectedMeasureCatalogs(long? catalogId, long? planId);
    void deleteByMainCatalogId(long? mainCatalogId, long? planId);
    void deleteByCatalogIds(HashSet<long?> catalogIds, long? planId);
    bool existsByRelatedCatalogId(long? catalogId, long? planId);
}
