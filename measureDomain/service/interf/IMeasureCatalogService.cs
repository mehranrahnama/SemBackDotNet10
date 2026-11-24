using MeasureRepository = global::SemBackDotNet10.measureDomain.model.entity.MeasureRepository;
// Mechanical conversion from Java (service/interf/IMeasureCatalogService.java). Complex bodies are marked with TODO.
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
using SemBackDotNet10.measureDomain.controller.dto.measureCatalogDtos;



namespace SemBackDotNet10.measureDomain.service.interf;

/// <summary>معادل IMeasureCatalogService.java (اینترفیس جاوا) — در C# با پیشوند I.</summary>
public interface IMeasureCatalogService : IProcessService<MeasureCatalog,long?,MeasureCatalogRepository>
{
    MeasureCatalog findByIdOrElseThrow(long? id);
    MeasureCatalogRepository getRepository();
    MeasureCatalog findMeasurePeriod(long? dimMeasureId, long? planId, long? levelId);
    long? findIdByRepositoryIdAndLevelId(long? measureRepositoryId, long? levelId);
    List<long?> getUserLevelIds(long? userId);
    MeasureCatalogDto mapToDto(MeasureCatalog measureCatalog);
    MeasureCatalog mapToEntity(MeasureCatalogDto measureCatalogDto);
    void UpdateMeasureConfirmStatus(long? id, string? outCome);
    void deleteMeasureCatalog(long? id, HttpRequest request);
    bool expireMeasureCatalogTasks(long? tenantId);
    MeasureCatalog findOrInsertMeasureCatalog(System.Text.Json.Nodes.JsonObject measureProcess, MeasureRepository measureRepository);
    void changeModir(long? oldModirId, long? newModirId, long? planId);
    void changeTashilgar(long? oldModirId, long? newModirId, long? planId);
    SearchDto getMeasureCatalogModir(string? body, long? levelId, int page, int size, string? orderBy, SortDirection direction);
    void updateMeasurePeriod(long? measureCatalogId, int measurePeriod);
    void assignFacilitatorAndManager(long? planningLevelId, long? facilitatorId, long? managerId);
    int findMeasurePeriodByMeasureId(long? measureId);
    void restoreMeasureCatalogFromLog(int measurePeriod, long? measureId);
    List<MeasureCatalog> createMeasureCatalogForPlanningLevels(
        IReadOnlyList<global::SemBackDotNet10.basicDomain.Model.Entity.PlaningLevel> levels, MeasureCatalog source);
    List<MeasureCatalog> findAllWithTaskUrlIsNotNull();
    SearchDto findAllMeasureCatalog(MeasureCatalogRequestDto requestDto);
}
