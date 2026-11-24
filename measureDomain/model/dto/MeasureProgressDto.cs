// Mechanical conversion from Java (model/dto/MeasureProgressDto.java). Complex bodies are marked with TODO.
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

/// <summary>معادل MeasureProgressDto.java — lombok @Getter/@Setter/@NoArgsConstructor در C# به property عمومی تبدیل شد.</summary>
public class MeasureProgressDto
{
    public long? Id { get; set; }
    public string? MeasureName { get; set; }
    public string? MeasureUnitName { get; set; }
    public int MeasurePeriod { get; set; }
    public int DesiredMethod { get; set; }
    public decimal? PlanValue { get; set; }
    public decimal? MonthPlanValue { get; set; }
    public byte MeasurePayeshType { get; set; }
    public byte MeasureType { get; set; }
    public decimal? ActualValue { get; set; }
    public decimal? DailyValue { get; set; }
    public string? Deviation { get; set; }
    public string? Correction { get; set; }
    public long? MeasureActConfirmStatus { get; set; }
    public long? LevelID { get; set; }
    public long? Tashilgar { get; set; }
    public long? TashilgarHamkar { get; set; }
    public string? PayeshgarName { get; set; }
    public long? Modir { get; set; }
    public string? ModirName { get; set; }
    public bool HasAccess { get; set; }
    public bool HasCorrectionEMS { get; set; }
    public bool HasUploadedFile { get; set; }
    public DateTime? Correctiondeadlinedate { get; set; }
    public DateTime? Correctionstartdateaction { get; set; }
    public long? Correctionexecuter { get; set; }
    public decimal? CorrectionActualValue { get; set; }
    public bool IsRoot { get; set; }
    public string? ExecuterName { get; set; }
    public long? ConfirmStatus { get; set; }
    public string? OutCome { get; set; }
    public string? TaskUrl { get; set; }
    public long? CurrentMonth { get; set; }
    public decimal? MeasurePCN { get; set; }
    public string? StrategyObjTitle { get; set; }
    public string? DesLevel { get; set; }
    public long? MeasureId { get; set; }
    public long? Month { get; set; }
    // TODO(ctor): سازنده‌ی چندآرگومانی جاوا — به‌صورت object initializer یا سازنده‌ی صریح منتقل شود:
    // 
    // TODO(ctor): سازنده‌ی چندآرگومانی جاوا — به‌صورت object initializer یا سازنده‌ی صریح منتقل شود:
    // 
    // TODO(ctor): سازنده‌ی چندآرگومانی جاوا — به‌صورت object initializer یا سازنده‌ی صریح منتقل شود:
    // 
}
