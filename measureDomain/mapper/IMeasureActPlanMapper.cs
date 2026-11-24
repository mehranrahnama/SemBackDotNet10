// Mechanical conversion from Java (mapper/IMeasureActPlanMapper.java). Complex bodies are marked with TODO.
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
using MeasureEntity = SemBackDotNet10.measureDomain.model.entity.Measure;
using SemBackDotNet10.measureDomain.controller.dto.measureActPlanDtos;


namespace SemBackDotNet10.measureDomain.mapper;

/// <summary>معادل IMeasureActPlanMapper.java (اینترفیس جاوا) — در C# با پیشوند I.</summary>
public interface IMeasureActPlanMapper
{
    // @Mapping(target = "id", ignore = true)
    // @Mapping(target = "measure", source = "targetMeasure")
    // @Mapping(target = ".", source = "base")
    // @Mapping(target = "taskUrl", source = "base.taskUrl")
    // @Mapping(target = "confirmstatus", source = "base.confirmstatus")
    // @Mapping(target = "deleted", source = "base.deleted")
    // @Mapping(target = "outCome", source = "base.outCome")
    // @Mapping(target = "deleteFirstStatus", ignore = true)
    // @Mapping(target = "replannedForHistory", ignore = true)
    // @Mapping(target = "replanStatusForHistory", ignore = true)
    // @Mapping(target = "createdby", ignore = true)
    // @Mapping(target = "createddate", ignore = true)
    // @Mapping(target = "modifiedby", ignore = true)
    // @Mapping(target = "modifieddate", ignore = true)
    // @Mapping(target = "version", ignore = true)
    MeasureActPlan clone(MeasureActPlan source, MeasureEntity targetMeasure);
    // TODO(signature): default void clearAuditFields(@MappingTarget MeasureActPlan target) { target.setCreatedby(null); target.setCreateddate(null); target.setModifiedby(null); target
    // @Mapping(target = "id", ignore = true)
    // @Mapping(target = "planValue", source = "planValue")
    // @Mapping(target = "actualValue", source = "actualValue")
    // @Mapping(target = "monthPlanValue", source = "monthPlanValue")
    // @Mapping(target = "measure", ignore = true)
    // @Mapping(target = "month", ignore = true)
    void copyPlanValueAndActualValueAndMonthPlanValue(MeasureActPlan source, MeasureActPlan target);
    // @Mapping(target = "id", ignore = true)
    // @Mapping(target = "dailyValue", source = "dailyValue")
    // @Mapping(target = "actualValue", source = "actualValue")
    // @Mapping(target = "measure", ignore = true)
    // @Mapping(target = "month", ignore = true)
    void copyDailyValueAndActualValue(MeasureActPlan source, MeasureActPlan target);
    // TODO(signature): ) { if (source.getTaskUrl() == null || target.getId() == null) { return; } target.setTaskUrl( source.getTaskUrl() .replaceAll("entityId=\\d+", "entityId=" + tar
    MeasurePeriodCalculationDto mapToMeasurePeriodCalculationDto(MeasureActPlanDto dto);
    List<MeasurePeriodCalculationDto> mapToMeasurePeriodCalculationDtos(List<MeasureActPlanDto> dtos);
    // TODO(signature): }) }) MeasurePeriodCalculationDto mapEntityToMeasurePeriodCalculationDto(MeasureActPlan plan);
    List<MeasurePeriodCalculationDto> mapEntityToMeasurePeriodCalculationDtos(List<MeasureActPlan> plans);
}
