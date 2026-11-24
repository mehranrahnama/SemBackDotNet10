using MeasureRepository = global::SemBackDotNet10.measureDomain.model.entity.MeasureRepository;
using global::SemBackDotNet10.measureDomain.controller.dto.measureActPlanDtos;
// Mechanical conversion from Java (service/interf/IMeasureReplanLogService.java). Complex bodies are marked with TODO.
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
using SemBackDotNet10.Base.Service.Inter;



namespace SemBackDotNet10.measureDomain.service.interf;

/// <summary>معادل IMeasureReplanLogService.java (اینترفیس جاوا) — در C# با پیشوند I.</summary>
public interface IMeasureReplanLogService : IProcessService<MeasureReplanLog,long?,MeasureReplanLogRepository>
{
    void createOrUpdateMeasureReplanLog(Measure measure, MeasureCatalog measureCatalog, MeasureRepository measureRepository, int measurePeriod, int desiredMethod, int measurePayeshType, decimal? currentYearTarget);
    MeasureReplanLog findFirstByMeasureOrderByCreatedDate(Measure measure);
    void createMeasureReplanLogs(NewPlanningDto newPlanningDto, MeasureReplanLogCreationDto measureReplanLogCreationDto);
    MeasureReplanLog getLatestReplanLog(long? id);
    bool existsPreviousReplanLog(long? measureId, long? logId);
}
