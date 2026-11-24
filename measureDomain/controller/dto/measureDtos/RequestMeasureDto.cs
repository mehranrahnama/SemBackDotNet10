// Mechanical conversion from Java (controller/dto/measureDtos/RequestMeasureDto.java). Complex bodies are marked with TODO.
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


namespace SemBackDotNet10.measureDomain.controller.dto.measureDtos;

/// <summary>معادل RequestMeasureDto.java — lombok @Getter/@Setter/@NoArgsConstructor در C# به property عمومی تبدیل شد.</summary>
public class RequestMeasureDto
{
    public long? Id { get; set; }
    public long? MeasureRepositoryId { get; set; }
    public long? MeasureCatalogId { get; set; }
    public string? MeasureName { get; set; }
    public long? StrategyObjectId { get; set; }
    public string? StrategyObjectTitle { get; set; }
    public long? UpperLevelStrategyObjectId { get; set; }
    public long? PlanId { get; set; }
    public long? LevelId { get; set; }
    public long? ModirId { get; set; }
    public long? TashilgarId { get; set; }
    public decimal? CurrentValue { get; set; }
    public decimal? CurrentYearTarget { get; set; }
    public decimal? OneYearLaterTarget { get; set; }
    public decimal? TwoYearLaterTarget { get; set; }
    public string? TargetTheory { get; set; }
    public bool? ZeroTarget { get; set; }
    public bool? FlgKpi { get; set; }
    public bool? ZeroInitial { get; set; }
    public bool SendToUpperLevels { get; set; }
    public bool SendToParentOnly { get; set; }
    public decimal? PerfectExtreme { get; set; }
    public decimal? WarnExtreme { get; set; }
    public decimal? BenchmarkValue { get; set; }
    public long? MeasureConfirmStatus { get; set; }
    public List<long>? CoWorkersId { get; set; }
    public List<long>? BeneficiariesId { get; set; }
    public string? OutCome { get; set; }
}
