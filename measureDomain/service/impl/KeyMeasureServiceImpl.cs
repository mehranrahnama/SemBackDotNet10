using global::SemBackDotNet10.measureDomain.controller.dto.keyMeasureDtos;
// Mechanical conversion from Java (service/impl/KeyMeasureServiceImpl.java). Complex bodies are marked with TODO.
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
using PlanPeriodRepository = global::SemBackDotNet10.basicDomain.Repositories.IPlanPeriodRepository;
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

/// <summary>معادل KeyMeasureServiceImpl.java — تزریق وابستگی‌ها با constructor injection انجام شده است.</summary>
public class KeyMeasureServiceImpl : IKeyMeasureService
{
    private readonly KeyMeasureRepository _keyMeasureRepository;
    private readonly MeasureCatalogRepository _measureCatalogRepository;
    private readonly PlanPeriodRepository _planPeriodRepository;
    private readonly CommentRepository _commentRepository;
    private readonly HistoryRepository _historyRepository;

    public KeyMeasureServiceImpl(KeyMeasureRepository keyMeasureRepository, MeasureCatalogRepository measureCatalogRepository,
        PlanPeriodRepository planPeriodRepository, CommentRepository commentRepository, HistoryRepository historyRepository)
    {
        _keyMeasureRepository = keyMeasureRepository;
        _measureCatalogRepository = measureCatalogRepository;
        _planPeriodRepository = planPeriodRepository;
        _commentRepository = commentRepository;
        _historyRepository = historyRepository;
    }

    public CommentRepository GetCommentRepository() => _commentRepository;
    public HistoryRepository GetHistoryRepository() => _historyRepository;
    public KeyMeasureRepository GetRepository() => _keyMeasureRepository;

    public KeyMeasureRepository getRepository() => GetRepository();
    public void create(List<KeyMeasureRequestDto> requestList)
        => CreateBatchAsync(requestList).GetAwaiter().GetResult();

    private async Task CreateBatchAsync(List<KeyMeasureRequestDto> requestList)
    {
        var pending = new List<KeyMeasure>();
        var keys = new HashSet<(long? CatalogId, long? PlanId)>();
        foreach (var request in requestList)
        {
            ValidateValue(request.TargetNumericValue, request.TargetTextValue, "target");
            ValidateValue(request.ActualNumericValue, request.ActualTextValue, "actual");
            var catalogId = request.MeasureCatalogId;
            var planId = request.PlanId;
            if (!keys.Add((catalogId, planId)) || (await _keyMeasureRepository.FindAllAsync(
                    x => x.MeasureCatalog.Id == catalogId && x.PlanPeriod.PlanId == planId)).Count != 0)
                throw new BusinessException($"KeyMeasure already exists for MeasureCatalogId={catalogId} and PlanId={planId}");

            var catalog = await _measureCatalogRepository.FindByIdAsync(catalogId)
                ?? throw new BusinessException($"MeasureCatalog not found: {catalogId}");
            var period = (planId.HasValue ? await _planPeriodRepository.FindByPlanIdAsync(planId.Value) : null)
                ?? throw new BusinessException($"PlanPeriod not found: {planId}");
            pending.Add(new KeyMeasure
            {
                MeasureCatalog = catalog,
                PlanPeriod = period,
                TargetNumericValue = request.TargetNumericValue,
                ActualNumericValue = request.ActualNumericValue,
                TargetTextValue = request.TargetTextValue,
                ActualTextValue = request.ActualTextValue
            });
        }

        if (pending.Count > 0)
            await _keyMeasureRepository.SaveAllAsync(pending);
    }

    private static void ValidateValue(decimal? numeric, string? text, string name)
    {
        if (numeric.HasValue && !string.IsNullOrWhiteSpace(text))
            throw new ArgumentException($"{name} numeric value and text value cannot both be set.");
    }
    // TODO(signature): ) { List<Long> measureCatalogIds = keyMeasureRepository.findDistinctMeasureCatalogIds( request.filter() ); if (measureCatalogIds.isEmpty()) { return List.of(); 
    public void update(long? id, KeyMeasureRequestDto request)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — request)
        throw new NotImplementedException();
    }
    public void delete(long? id)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — id)
        throw new NotImplementedException();
    }
    // TODO(signature): List<Long> measureCatalogIds ) { Map<Long, List<KeyMeasureInfo>> itemsByMeasureCatalog = items.stream() .collect(Collectors.groupingBy( KeyMeasureInfo::measureC
    // TODO(signature): ) { KeyMeasureInfo first = items.getFirst(); List<KeyMeasureYearValueInfo> values = items.stream() .map(this::toYearValueInfo) .toList(); return new KeyMeasureG
    // TODO(signature): ) { return new KeyMeasureYearValueInfo( item.keyMeasureId(), item.planId(), item.targetNumericValue(), item.actualNumericValue(), item.targetTextValue(), item.a
    // TODO(signature): ) { validateSingleValueType(request.targetNumericValue(), request.targetTextValue(), "target"); validateSingleValueType(request.actualNumericValue(), request.ac
    // TODO(signature): ) { if (numericValue != null && hasText(textValue)) { throw new IllegalArgumentException( fieldName + " numeric value and text value cannot both be set." ); } }
    bool hasText(string? value)
    {
        // TODO: بدنه‌ی متد از جاوا منتقل شود — value)
        throw new NotImplementedException();
    }
}
