// Mechanical conversion from Java (model/dto/MeasurePayeshPCNInfoDto.java). Complex bodies are marked with TODO.
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

/// <summary>معادل MeasurePayeshPCNInfoDto.java — lombok @Getter/@Setter/@NoArgsConstructor در C# به property عمومی تبدیل شد.</summary>
public class MeasurePayeshPCNInfoDto
{
    public int MeasureDataByStrategyObjectId { get; set; }
    public string? ReportStatus { get; set; }
    public string? DesLevel { get; set; }
    public string? CurrentDate { get; set; }
    public int LevelId { get; set; }
    public string? Modir_measure { get; set; }
    public decimal? Weight { get; set; }
    public decimal? PlanValue { get; set; }
    public decimal? MonthPlanValue { get; set; }
    public decimal? ActualValue { get; set; }
    public decimal? DailyValue { get; set; }
    public decimal? MeasurePCN { get; set; }
    public string? Masool_measure { get; set; }
    public int MeasureId { get; set; }
    public string? MeasureName { get; set; }
    public string? MeasureOrigin { get; set; }
    public int ConfirmStatus { get; set; }
    public string? DesiredMethod { get; set; }
    public string? MeasurePayeshType { get; set; }
    public string? MeasurePeriod { get; set; }
    public decimal? WarnExtreme { get; set; }
    public decimal? PerfectExtreme { get; set; }
    public decimal? WarnExtremeFromSetting { get; set; }
    public decimal? PerfectExtremeFromSetting { get; set; }
}
