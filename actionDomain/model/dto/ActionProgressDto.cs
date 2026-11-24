// Mechanical conversion from Java (model/dto/ActionProgressDto.java). Complex bodies are marked with TODO.
// وابستگی‌های اعلام‌شده: basicDomain/Common/base هنوز ارسال نشده‌اند.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SemBackDotNet10.actionDomain.model.entity;
using SemBackDotNet10.actionDomain.model.dto;
using SemBackDotNet10.actionDomain.controller.dto;
using SemBackDotNet10.actionDomain.mapper;
using SemBackDotNet10.actionDomain.repository;
using SemBackDotNet10.actionDomain.service.interf;
using SemBackDotNet10.actionDomain.domain;
using SemBackDotNet10.actionDomain.strategy.confirmStatusHandler.context;
using SemBackDotNet10.actionDomain.strategy.confirmStatusHandler.factory;
using SemBackDotNet10.actionDomain.strategy.confirmStatusHandler.handler;



namespace SemBackDotNet10.actionDomain.model.dto;

/// <summary>معادل ActionProgressDto.java — lombok @Getter/@Setter/@NoArgsConstructor در C# به property عمومی تبدیل شد.</summary>
public class ActionProgressDto
{
    public long? ActionActPlanId { get; set; }
    public string? ActionPlanCatalogDescription { get; set; }
    public string? HadafName { get; set; }
    public string? PayeshgarName { get; set; }
    public decimal? PlanValue { get; set; }
    public decimal? ActualValue { get; set; }
    public long? ActualBudget { get; set; }
    public string? Deviation { get; set; }
    public string? Correction { get; set; }
    public long? ActionActPlanConfirmStatus { get; set; }
    public long? ModirId { get; set; }
    public string? ModirName { get; set; }
    public long? PayeshgarId { get; set; }
    public long? LevelId { get; set; }
    public bool HasAccess { get; set; }
    public bool HasCorrectionEMS { get; set; }
    public bool HasUploadedFile { get; set; }
    public long? Month { get; set; }
    public DateTime? CorrectionStartDateAction { get; set; }
    public DateTime? CorrectionDeadlineDate { get; set; }
    public long? CorrectionExecuter { get; set; }
    public decimal? CorrectionActualValue { get; set; }
    public string? ExecuterName { get; set; }
    public long? ActionPlanCatalogId { get; set; }
    public string? TaskUrl { get; set; }
    public decimal? ActionPCN { get; set; }
    public string? DesLevel { get; set; }
    public bool AddToSupremeLevel { get; set; }
    public string? EndDate { get; set; }
    public string? StartDate { get; set; }
    public bool IsRoot { get; set; }
    public List<ActionPlanActivityDto> Activities { get; set; } = new();
    // TODO(ctor): سازنده‌ی چندآرگومانی جاوا — به‌صورت object initializer یا سازنده‌ی صریح منتقل شود:
    // 
    // TODO(ctor): سازنده‌ی چندآرگومانی جاوا — به‌صورت object initializer یا سازنده‌ی صریح منتقل شود:
    // 
}
