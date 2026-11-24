// Mechanical conversion from Java (model/dto/MeasureRepositoryDto.java). Complex bodies are marked with TODO.
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

/// <summary>معادل MeasureRepositoryDto.java — lombok @Getter/@Setter/@NoArgsConstructor در C# به property عمومی تبدیل شد.</summary>
public class MeasureRepositoryDto
{
    public long? Id { get; set; }
    public long? CatalogId { get; set; }
    public string? MeasureName { get; set; }
    public string? PlaningLevelDesLevel { get; set; }
    public long? PlaningLevelId { get; set; }
    public string? MeasureUnitDescription { get; set; }
    public byte CatalogDataType { get; set; }
    public int DesiredMethod { get; set; }
    public byte MeasureType { get; set; }
    public long? MeasureConfirmStatus { get; set; }
    public long? MeasureConfirmStatusMC { get; set; }
    public byte MeasurePayeshType { get; set; }
    public int CatalogMeasurePeriod { get; set; }
    public long MeasureUnitId { get; set; }
    public bool AddToSubLevels { get; set; }
    public string? Formula { get; set; }
    public string? FormulaDes { get; set; }
    public string? Tashilgar { get; set; }
    public string? TashilgarHamkar { get; set; }
    public string? Modir { get; set; }
    public string? Hamesh { get; set; }
    public long? PlanId { get; set; }
    public string? Code { get; set; }
    public string? DesThreeChar { get; set; }
    public bool MeasureCatalogDeleted { get; set; }
    public bool PlanChanged { get; set; }
    public MeasureCatalogDto MeasureCatalogDto { get; set; } = new();
    public List<MeasureCatalogDto> MeasureCatalogDtos { get; set; } = [];
    public List<long> LevelIds { get; set; } = [];
    // TODO(ctor): سازنده‌ی چندآرگومانی جاوا — به‌صورت object initializer یا سازنده‌ی صریح منتقل شود:
    // 
    // TODO(ctor): سازنده‌ی چندآرگومانی جاوا — به‌صورت object initializer یا سازنده‌ی صریح منتقل شود:
    // 
    // TODO(ctor): سازنده‌ی چندآرگومانی جاوا — به‌صورت object initializer یا سازنده‌ی صریح منتقل شود:
    // 
    // TODO(ctor): سازنده‌ی چندآرگومانی جاوا — به‌صورت object initializer یا سازنده‌ی صریح منتقل شود:
    // 
    // TODO(ctor): سازنده‌ی چندآرگومانی جاوا — به‌صورت object initializer یا سازنده‌ی صریح منتقل شود:
    // 
    // TODO(ctor): سازنده‌ی چندآرگومانی جاوا — به‌صورت object initializer یا سازنده‌ی صریح منتقل شود:
    // 
}
