// Mechanical conversion from Java (controller/dto/ActionDTO.java). Complex bodies are marked with TODO.
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



namespace SemBackDotNet10.actionDomain.controller.dto;

/// <summary>معادل ActionDTO.java — lombok @Getter/@Setter/@NoArgsConstructor در C# به property عمومی تبدیل شد.</summary>
public class ActionDTO
{
    public string? DesIncomplianceCrprr { get; set; }
    public string? DesRootReasonCrprr { get; set; }
    public string? DatDeadlineAction { get; set; }
    public string? DesOwnAction { get; set; }
    public string? LkpCodExecutorAction { get; set; }
    public string? NamFacilatorCrprr { get; set; }
    public string? DatStartAction { get; set; }
    public string? NamJobpoAction { get; set; }
    public string? NamWorkReqAction { get; set; }
    public long? NumProgressAction { get; set; }
    public string? NamGoalCrprr { get; set; }
    public string? NamPlanLevCrprr { get; set; }
    public string? NamActionStrgCrprr { get; set; }
    public long? NumMonthCrprr { get; set; }
    public long? NumPeriodPlanCrprr { get; set; }
    public long? NumAmountPlanCrprr { get; set; }
    public long? NumProgressCrprr { get; set; }
}
