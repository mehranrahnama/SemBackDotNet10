// Mechanical conversion from Java (model/dto/ActionActReplanDto.java). Complex bodies are marked with TODO.
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

/// <summary>معادل ActionActReplanDto.java — lombok @Getter/@Setter/@NoArgsConstructor در C# به property عمومی تبدیل شد.</summary>
public class ActionActReplanDto
{
    public long? Id { get; set; }
    public string? Year { get; set; }
    public long? Month { get; set; }
    public decimal? PlanValue { get; set; }
    public decimal? PlanValueNew { get; set; }
    public decimal? ActualValue { get; set; }
    public decimal? ActualValueNew { get; set; }
    public string? ReplanOutCome { get; set; }
    public string? DesLevel { get; set; }
    public string? ActionName { get; set; }
    public string? StartDate { get; set; }
    public string? EndDate { get; set; }
    public string? ReasonEndDateChange { get; set; }
    public long? ReplannedStatus { get; set; }
    public bool IsReplanned { get; set; }
}
