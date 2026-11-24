using SearchDto = global::SemBackDotNet10.Base.model.dto.SearchDto;
// Mechanical conversion from Java (service/interf/IMeasureActPlanService.java). Complex bodies are marked with TODO.
// وابستگی‌های اعلام‌شده: basicDomain/Common/base هنوز ارسال نشده‌اند.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SemBackDotNet10.measureDomain.model.entity;
using MeasureEntity = SemBackDotNet10.measureDomain.model.entity.Measure;
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
using SemBackDotNet10.Base.Service.Inter;
using SemBackDotNet10.measureDomain.controller.dto.measureActPlanDtos;


namespace SemBackDotNet10.measureDomain.service.interf;

/// <summary>معادل IMeasureActPlanService.java (اینترفیس جاوا) — در C# با پیشوند I.</summary>
public interface IMeasureActPlanService : IProcessService<MeasureActPlan,long?,MeasureActPlanRepository>
{
    MeasureActPlan findByIdOrElseThrow(long? id);
    SearchDto findAll(long? id, int page, int size, string? orderBy, SortDirection direction);
    SearchDto findAll_V2(long? id, int page, int size, string? orderBy, SortDirection direction);
    void createAndCopyMeasureActPlans(MeasureEntity sourceMeasure);
    SearchDto createMeasureActPlan(MeasureActPlanRequestDto requestDto, int page, int size, string? orderBy, SortDirection direction, bool replan);
    SearchDto getCustomMeasureActPlan(string? body, int page, int size, string? orderBy, SortDirection direction, bool replan);
    void MeasureActRePlan(MeasureActPlanRequestDto requestDto, int page, int size, string? orderBy, SortDirection direction);
    bool? EditMeasureActPlan(MeasureActPlanMeasureDto paramMapList);
    void parseCalculateAchievedPCNFormula(string? formula);
    List<MeasureActPlanCalc> findMeasureActPlanCalcByMeasureActPlanId(long? measureActPlanId);
    MeasureProgressDto findMeasureProgressByMeasureActPlanId(long? measureActPlanId);
    List<MeasureActPlanDto> findAllMeasureActPlanDtos();
    List<MeasureActPlanDto> findAllmeasureActPlansByMeasureId(long? measureId);
    List<MeasureActPlanDto> findAllMeasureActPlansOfMeasure(long? measureActPlanId);
    SearchDto loadMeasureProgressGrid(Dictionary<string?,string?> headers, string? body, int page, int size, string? orderBy, SortDirection direction);
    void editDailyValues(List<MeasureProgressDto> measureProgressDtos);
    void setTaskUrl(List<MeasureProgressDto> measureProgressDtos);
    MeasureActPlanGetDeviationDto editDeviation(MeasureProgressDto measureProgressDtos);
    MeasureActPlanDto editCorrection(MeasureProgressDto measureProgressDtos);
    void editStatus(MeasureProgressDto measureProgressDto);
    List<MeasureActPlanDto> getCorrectionByMeasureId(long? id);
    MeasureWrapperDTO getMeasureActPlanByMeasureId(long? measureId, int page, int size, string? orderBy, SortDirection direction);
    MeasureWrapperDTO getCustomMeasureActPlanByMeasureId(long? measureId, int page, int size, string? orderBy, SortDirection direction);
    bool expireMeasureActPlanTasks(long? tenantId);
    void expireMeasureActPlans(List<MeasureActPlan> measureActPlans);
    // TODO(signature): MeasureReplanActionInfoDto actionInfo ) throws Exception;
    void removeExpiredMeasureActPlan(List<MeasureActPlan> expiredMeasureActPlans);
    void restoreMeasureActPlans(long? measureId);
    void calculateActualAndDailyValues(string? body);
    decimal? getCorrectionActualValueForHOSCO(long? measureActPlanId);
    decimal? calcMeasurePCN(long? measureActPlanId);
    SearchDto insertEmptyMeasureActPlan(MeasureActPlanRequestDto requestDto, int page, int size, string? orderBy, SortDirection direction);
    void deleteMeasureActPlan(long? measureId);
    void newPlanningByMeasure(NewPlanningDto dto);
    List<MeasureActPlan> findAllWithTaskUrlIsNotNull();
    List<SemBackDotNet10.measureDomain.repository.projection.IKeyValueView> findAllWithNullActualValue();
    void calculateValuesForProgresses(Dictionary<long?,Byte> measureIds);
}
