// Mechanical conversion from Java (model/dto/ActionPayeshInfoDto.java). Complex bodies are marked with TODO.
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

/// <summary>معادل ActionPayeshInfoDto.java — lombok @Getter/@Setter/@NoArgsConstructor در C# به property عمومی تبدیل شد.</summary>
public class ActionPayeshInfoDto
{
    public int LevelId { get; set; }
    public string? DesLevel { get; set; }
    public string? ReportStatus { get; set; }
    public int ActionDataByStrategyObjectId { get; set; }
    public int PlanId { get; set; }
    public string? Modir_measure { get; set; }
    public string? Masool_action { get; set; }
    public string? ActionMasool { get; set; }
    public string? ActionName { get; set; }
    public decimal? Weight { get; set; }
    public decimal? WarnExtreme { get; set; }
    public decimal? PerfectExtreme { get; set; }
    public decimal? PlanValue1 { get; set; }
    public decimal? PlanValue2 { get; set; }
    public decimal? PlanValue3 { get; set; }
    public decimal? PlanValue4 { get; set; }
    public decimal? PlanValue5 { get; set; }
    public decimal? PlanValue6 { get; set; }
    public decimal? PlanValue7 { get; set; }
    public decimal? PlanValue8 { get; set; }
    public decimal? PlanValue9 { get; set; }
    public decimal? PlanValue10 { get; set; }
    public decimal? PlanValue11 { get; set; }
    public decimal? PlanValue12 { get; set; }
    public decimal? ActualValue1 { get; set; }
    public decimal? ActualValue2 { get; set; }
    public decimal? ActualValue3 { get; set; }
    public decimal? ActualValue4 { get; set; }
    public decimal? ActualValue5 { get; set; }
    public decimal? ActualValue6 { get; set; }
    public decimal? ActualValue7 { get; set; }
    public decimal? ActualValue8 { get; set; }
    public decimal? ActualValue9 { get; set; }
    public decimal? ActualValue10 { get; set; }
    public decimal? ActualValue11 { get; set; }
    public decimal? ActualValue12 { get; set; }
}
