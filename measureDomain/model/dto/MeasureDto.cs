// Mechanical conversion from Java (model/dto/MeasureDto.java). Complex bodies are marked with TODO.
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


namespace SemBackDotNet10.measureDomain.model.dto;

/// <summary>معادل MeasureDto.java — lombok @Getter/@Setter/@NoArgsConstructor در C# به property عمومی تبدیل شد.</summary>
public class MeasureDto
{
    public long? Id { get; set; }
    public string? StrategyObjectTitle { get; set; }
    public string? MeasureName { get; set; }
    public string? Tashilgar { get; set; }
    public string? Modir { get; set; }
    public int MeasurePeriod { get; set; }
    public string? DesMeasureF { get; set; }
    public float? Weight { get; set; }
    public long? MeasureCatalogId { get; set; }
    public long? StrategyObjectId { get; set; }
    public long? PlanId { get; set; }
    public decimal? CurrentValue { get; set; }
    public decimal? CurrentYearTarget { get; set; }
    public long? LevelId { get; set; }
    public string? DesLevel { get; set; }
    public long? MeasureRepositoryId { get; set; }
    public decimal? OneYearLaterTarget { get; set; }
    public decimal? TwoYearLaterTarget { get; set; }
    public Byte? DataType { get; set; }
    public int DesiredMethod { get; set; }
    public bool SendToUpperLevels { get; set; }
    public bool SendToParentOnly { get; set; }
    public bool AddToSubLevels { get; set; }
    public string? Formula { get; set; }
    public string? FormulaDes { get; set; }
    public bool? AddedToDashboard { get; set; }
    public string? TargetTheory { get; set; }
    public bool ZeroTarget { get; set; }
    public bool FlgKpi { get; set; }
    public bool ZeroInitial { get; set; }
    public decimal? PerfectExtreme { get; set; }
    public decimal? WarnExtreme { get; set; }
    public byte MeasurePayeshType { get; set; }
    public string? OutCome { get; set; }
    public int IsPlanned { get; set; }
    public bool IsReplanned { get; set; }
    public long? ReplannedStatus { get; set; }
    public bool Deleted { get; set; }
    public decimal? BenchmarkValue { get; set; }
    public long? TashilgarId { get; set; }
    public long? ModirId { get; set; }
    public string? MeasureCode { get; set; }
    public string? CommitteeCode { get; set; }
    public List<long?> CoWorkersId { get; set; }
    public List<long?> BeneficiariesId { get; set; }
    public List<MeasureCoworkerDto> MeasureCoworkerDtoList { get; set; }
    public List<MeasureBeneficiaryDto> MeasureBeneficiaryDtoList { get; set; }
    public UpperLevelStrategyObjectDto TopLevelStrategyObject { get; set; }
    public long? MeasureConfirmStatus { get; set; }
    // TODO(ctor): سازنده‌ی چندآرگومانی جاوا — به‌صورت object initializer یا سازنده‌ی صریح منتقل شود:
    // 
    // TODO(ctor): سازنده‌ی چندآرگومانی جاوا — به‌صورت object initializer یا سازنده‌ی صریح منتقل شود:
    // 
    // TODO(ctor): سازنده‌ی چندآرگومانی جاوا — به‌صورت object initializer یا سازنده‌ی صریح منتقل شود:
    // 
}
