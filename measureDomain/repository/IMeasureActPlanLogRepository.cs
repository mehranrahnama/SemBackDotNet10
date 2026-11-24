using Measure = global::SemBackDotNet10.measureDomain.model.entity.Measure;
// Mechanical conversion from Java (repository/MeasureActPlanLogRepository.java). Complex bodies are marked with TODO.
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

/// <summary>معادل MeasureActPlanLogRepository.java (اینترفیس جاوا) — در C# با پیشوند I.</summary>
public interface MeasureActPlanLogRepository : global::SemBackDotNet10.Base.repository.ISemRepository<MeasureActPlanLog,long?>
{
    //    @Query("select mapl from MeasureActPlanLog mapl where mapl.measure.id = :id")
    List<MeasureActPlanLog> findByMeasure(Measure measure);
    List<MeasureActPlanLog> measureId(long? measureId);
    // TODO(signature): """) Optional<List<MeasureActPlanLog>> findByMeasureIdAndNoNewValueSorted(@Param("measureId") Long measureId);
    List<MeasureActPlanLog> findTop12ByMeasureIdOrderByCreatedDateDesc(long? measureId);
}
