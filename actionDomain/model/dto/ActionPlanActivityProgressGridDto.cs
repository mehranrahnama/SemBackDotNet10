// Mechanical conversion from Java (model/dto/ActionPlanActivityProgressGridDto.java). Complex bodies are marked with TODO.
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

/// <summary>معادل ActionPlanActivityProgressGridDto.java — lombok @Getter/@Setter/@NoArgsConstructor در C# به property عمومی تبدیل شد.</summary>
public class ActionPlanActivityProgressGridDto
{
    public long? ActivityProgressId { get; set; }
    public long? Month { get; set; }
    public long? PlanId { get; set; }
    public decimal? ActualValue { get; set; }
    public string? ActivityName { get; set; }
    public string? ActivityDateStart { get; set; }
    public string? ActivityDateEnd { get; set; }
    public float? ActivityWeight { get; set; }
    public string? ActionName { get; set; }
    public string? PlanStartDate { get; set; }
    public string? PlanEndDate { get; set; }
    public string? Payeshgar { get; set; }
    public string? Modir { get; set; }
    public bool? Replanned { get; set; }
    public string? AccountableName { get; set; }
    // TODO(ctor): سازنده‌ی چندآرگومانی جاوا — به‌صورت object initializer یا سازنده‌ی صریح منتقل شود:
    // 
}
