// Mechanical conversion from Java (model/dto/ActionPlanDto.java). Complex bodies are marked with TODO.
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

/// <summary>معادل ActionPlanDto.java — lombok @Getter/@Setter/@NoArgsConstructor در C# به property عمومی تبدیل شد.</summary>
public class ActionPlanDto
{
    public long? Id { get; set; }
    public string? StrategyObjectDescription { get; set; }
    public string? ActionPlanCatalogDescription { get; set; }
    public string? Modir { get; set; }
    public string? CommitteeCode { get; set; }
    public decimal? YearStartPcn { get; set; }
    public decimal? YearCompletePcn { get; set; }
    public string? ActionPlanCatalogCode { get; set; }
    public bool? DeleteStatus { get; set; }
    public bool AddedToDashboard { get; set; }
    public long? ModirId { get; set; }
    public string? Payeshgar { get; set; }
    public List<ActionActPlanPlanningDto> ActionActPlans { get; set; }
    public long? PayeshgarId { get; set; }
    public int Priority { get; set; }
    public float? Weight { get; set; }
    public long? ActionPlanCatalogId { get; set; }
    public int IsPlanned { get; set; }
    public List<ActionPlanActivityDto> Activities { get; set; }
    public long? Type { get; set; }
    public string? StartDate { get; set; }
    public string? EndDate { get; set; }
    public long? Status { get; set; }
    public long? LevelId { get; set; }
    public bool SendToUpperLevels { get; set; }
    public bool SendToParentOnly { get; set; }
    public string? LevelDes { get; set; }
    public string? ActivityName { get; set; }
    public string? StartActivityDate { get; set; }
    public string? EndActivityDate { get; set; }
    public string? ActivityAccountant { get; set; }
    public List<ActionPlanBeneficiaryDto> ActionPlanBeneficiaryDtoList { get; set; }
    public List<ActionPlanCoworkerDto> ActionPlanCoworkerDtoList { get; set; }
    public UpperLevelStrategyObjectDto? TopLevelStrategyObject { get; set; }
    // TODO(ctor): سازنده‌ی چندآرگومانی جاوا — به‌صورت object initializer یا سازنده‌ی صریح منتقل شود:
    // 
    // TODO(ctor): سازنده‌ی چندآرگومانی جاوا — به‌صورت object initializer یا سازنده‌ی صریح منتقل شود:
    // 
}
