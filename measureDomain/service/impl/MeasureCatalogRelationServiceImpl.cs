using SearchDto = global::SemBackDotNet10.Base.model.dto.SearchDto;
// Mechanical conversion from Java (service/impl/MeasureCatalogRelationServiceImpl.java). Complex bodies are marked with TODO.
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
using MeasureDomainMeasureService = SemBackDotNet10.measureDomain.service.interf.IMeasureService;


namespace SemBackDotNet10.measureDomain.service.impl;

/// <summary>معادل MeasureCatalogRelationServiceImpl.java — تزریق وابستگی‌ها با constructor injection انجام شده است.</summary>
public class MeasureCatalogRelationServiceImpl : IMeasureCatalogRelationService
{
    private readonly IMeasureCatalogRelationMapper _measureCatalogRelationMapper;
    private readonly MeasureDomainMeasureService _measureService;
    private readonly MeasureCatalogRelationRepository _measureCatalogRelationRepository;
    private readonly global::SemBackDotNet10.basicDomain.service.IPlanPeriodService _planPeriodService;

    public MeasureCatalogRelationServiceImpl(
        IMeasureCatalogRelationMapper measureCatalogRelationMapper,
        MeasureDomainMeasureService measureService,
        MeasureCatalogRelationRepository measureCatalogRelationRepository,
        global::SemBackDotNet10.basicDomain.service.IPlanPeriodService planPeriodService)
    {
        _measureCatalogRelationMapper = measureCatalogRelationMapper;
        _measureService = measureService;
        _measureCatalogRelationRepository = measureCatalogRelationRepository;
        _planPeriodService = planPeriodService;
    }

    public MeasureCatalogRelationRepository GetRepository() => _measureCatalogRelationRepository;

    public MeasureCatalogRelationRepository getRepository() => GetRepository();

    public SearchDto findByMainCatalogId(
        long? mainRelId,
        string? filter,
        int page,
        int size,
        string? orderBy,
        SortDirection direction)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — direction)
        throw new NotImplementedException();
    }

    public SearchDto getmainMeasureCatalogByLevelIdAndPlanId(long? levelId, long? planId, string? filter, int page, int size, string? orderBy, SortDirection direction)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — direction)
        throw new NotImplementedException();
    }

    public void editMeasureCatalogRelation(List<MeasureCatalogRelationDto> measureCatalogRelationDto)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — measureCatalogRelationDto)
        throw new NotImplementedException();
    }

    public MeasureCatalogRelation findByIdOrElseThrow(long? id)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — id)
        throw new NotImplementedException();
    }
    // @Transactional()
    public void delete(long? id)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — id)
        throw new NotImplementedException();
    }
    // @Transactional
    public void create(MeasureCatalogRelationDto dto)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — dto)
        throw new NotImplementedException();
    }
    public List<MeasureCatalogRelation> findByMainMeasureCatalog(MeasureCatalog mainMeasureCatalog)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — mainMeasureCatalog)
        throw new NotImplementedException();
    }
    public void createRelationForPlaningLevel(IReadOnlyList<MeasureCatalog> catalogs,
        IReadOnlyList<PlaningLevel> levels, long planId)
    {
        if (catalogs.Count == 0 || levels.Count == 0)
            return;

        var period = _planPeriodService.FindPlanPeriodByPlanId(planId);
        var relations = new List<MeasureCatalogRelation>();
        foreach (var child in catalogs)
        {
            if (child?.PlaningLevel?.Parent is not { } parentLevel)
                continue;

            var parent = catalogs.FirstOrDefault(candidate => candidate?.PlaningLevel?.Id == parentLevel.Id);
            if (parent is null)
                continue;

            try
            {
                var existing = _measureCatalogRelationRepository.FindAllAsync(relation =>
                        relation.PlanPeriod.PlanId == period.PlanId &&
                        relation.MainMeasureCatalog.Id == parent.Id && relation.RelatedMeasureCatalog.Id == child.Id)
                    .GetAwaiter().GetResult();
                if (existing.Count != 0)
                    continue;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceWarning("Failed to check existence for catalog {0}: {1}", child.Id, ex.Message);
                continue;
            }

            relations.Add(new MeasureCatalogRelation
            {
                PlanPeriod = period,
                MainMeasureCatalog = parent,
                RelatedMeasureCatalog = child
            });
        }

        if (relations.Count == 0)
            return;

        try
        {
            _measureCatalogRelationRepository.SaveAllAsync(relations).GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.TraceError("Error saving catalog relations: {0}", ex);
            throw new global::SemBackDotNet10.Common.exception.BusinessException("در فرآیند ذخیره روابط برنامه‌ریزی خطایی رخ داده است.");
        }
    }
    // @Transactional(propagation = Propagation.MANDATORY)
    public void deleteByRelatedCatalogId(long? relatedCatalogId, long? planId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — planId)
        throw new NotImplementedException();
    }
    public List<long?> findAncestorIds(long? leafId)
    {
        return findAncestorIds(leafId, _planPeriodService.FindActivePlanPeriod().PlanId);
    }
    public List<long?> findAncestorIds(long? leafId, long? planId)
    {
        var seen = new HashSet<long?>();
        var ancestors = new List<long?>();
        var current = leafId;
        while (true)
        {
            var parents = _measureCatalogRelationRepository.findParentIds(current, planId);
            if (parents.Count == 0)
                return ancestors;
            if (parents.Count > 1)
                throw new global::SemBackDotNet10.Common.exception.BusinessException("شاخص های در روابط محاسباتی نمیتوانند بیش از یک والد داشته باشند.");
            current = parents[0];
            if (!seen.Add(current))
                throw new global::SemBackDotNet10.Common.exception.BusinessException("ساختار شاخص‌ها دارای حلقه است و معتبر نمی‌باشد.");
            ancestors.Add(current);
        }
    }
    // @Transactional
    public void deleteRelationsForMeasureCatalog(long? catalogId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — catalogId)
        throw new NotImplementedException();
    }
    public HashSet<MeasureCatalog> findConnectedMeasureCatalogs(long? catalogId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — catalogId)
        throw new NotImplementedException();
    }
    public HashSet<MeasureCatalog> findConnectedMeasureCatalogs(long? catalogId, long? planId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — planId)
        throw new NotImplementedException();
    }
    // @Transactional(propagation = Propagation.MANDATORY)
    public void deleteByMainCatalogId(long? mainCatalogId, long? planId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — planId)
        throw new NotImplementedException();
    }
    // @Transactional(propagation = Propagation.MANDATORY)
    public void deleteByCatalogIds(HashSet<long?> catalogIds, long? planId)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — planId)
        throw new NotImplementedException();
    }
    public bool existsByRelatedCatalogId(long? catalogId, long? planId)
    {
        return _measureCatalogRelationRepository.FindAllAsync(relation =>
                relation.RelatedMeasureCatalog.Id == catalogId && relation.PlanPeriod.PlanId == planId)
            .GetAwaiter().GetResult().Count != 0;
    }
}
