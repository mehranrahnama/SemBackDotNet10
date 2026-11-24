// Mechanical conversion from Java (model/dto/MeasureActPlanDto.java). Complex bodies are marked with TODO.
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

/// <summary>معادل MeasureActPlanDto.java — lombok @Getter/@Setter/@NoArgsConstructor در C# به property عمومی تبدیل شد.</summary>
public class MeasureActPlanDto
{
    public long? Id { get; set; }
    public long? Month { get; set; }
    public decimal? PlanValue { get; set; }
    public decimal? PlanValueNew { get; set; }
    public long? ConfirmStatus { get; set; }
    public decimal? MonthPlanValue { get; set; }
    public decimal? MonthPlanValueNew { get; set; }
    public decimal? ActualValue { get; set; }
    public decimal? ActualValueNew { get; set; }
    public long? PlanId { get; set; }
    public bool IsPlanned { get; set; }
    public long? DimMeasureId { get; set; }
    public string? Deviation { get; set; }
    public string? Correction { get; set; }
    public DateTime? CorrectionStartDateAction { get; set; }
    public DateTime? GetCorrectionDeadlineDate { get; set; }
    public long? CorrectionExecuterId { get; set; }
    public string? CorrectionExecuterUserName { get; set; }
    public string? CorrectionExecuterDisplayName { get; set; }
    public decimal? CorrectionActualValue { get; set; }
    public decimal? DailyValue { get; set; }
    public decimal? DailyValueNew { get; set; }
    public bool Deleted { get; set; }
    public int DesiredMethod { get; set; }
    public float? MeasureWeight { get; set; }
    public long? RoyalVisionActId { get; set; }
    public bool Active { get; set; }
    // TODO(ctor): سازنده‌ی چندآرگومانی جاوا — به‌صورت object initializer یا سازنده‌ی صریح منتقل شود:
    // 
}
