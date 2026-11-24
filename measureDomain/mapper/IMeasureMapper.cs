using Measure = global::SemBackDotNet10.measureDomain.model.entity.Measure;
// Mechanical conversion from Java (mapper/IMeasureMapper.java). Complex bodies are marked with TODO.
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


namespace SemBackDotNet10.measureDomain.mapper;

/// <summary>معادل IMeasureMapper.java (اینترفیس جاوا) — در C# با پیشوند I.</summary>
public interface IMeasureMapper
{
    // TODO(signature): BigDecimal currentYearTarget );
    // TODO(signature): }) }) Measure clone(Measure base, MeasureCatalog catalog, StrategyObject strategyObject, PlaningLevel level);
    // @Mapping(target = "id", ignore = true)
    // @Mapping(target = "confirmstatus", source = "confirmstatus")
    // @Mapping(target = "replannedStatus", source = "replannedStatus")
    // @Mapping(target = "replannedForHistory", source = "replannedForHistory")
    // @Mapping(target = "measureCatalog", ignore = true)
    // @Mapping(target = "strategyObject", ignore = true)
    // @Mapping(target = "weight", ignore = true)
    // @Mapping(target = "planned", source = "planned")
    // @Mapping(target = "taskUrl", source = "taskUrl")
    // @Mapping(target = "outCome", source = "outCome")
    // @Mapping(target = "sendToParentOnly", ignore = true)
    // @Mapping(target = "addToSupremeLevel", ignore = true)
    void copyPlanningValuesForTargetMeasure(Measure source, Measure target);
    // TODO(signature): default void forcePlannedCopy(Measure source, @MappingTarget Measure target) { target.setPlanned(source.isPlanned()); }
    // TODO(signature): ) { if (source.getTaskUrl() == null || target.getId() == null) { return; } target.setTaskUrl( source.getTaskUrl() .replaceAll("entityId=\\d+", "entityId=" + tar
}
