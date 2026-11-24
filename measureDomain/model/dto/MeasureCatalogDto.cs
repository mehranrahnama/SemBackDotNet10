// Mechanical conversion from Java (model/dto/MeasureCatalogDto.java). Complex bodies are marked with TODO.
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

/// <summary>معادل MeasureCatalogDto.java — lombok @Getter/@Setter/@NoArgsConstructor در C# به property عمومی تبدیل شد.</summary>
public class MeasureCatalogDto
{
    public long? Id { get; set; }
    public decimal? WorstMin { get; set; }
    public string? TargetTheory { get; set; }
    public bool ZeroTarget { get; set; }
    public bool ZeroInitial { get; set; }
    public Byte? DataType { get; set; }
    public int? MeasurePeriod { get; set; }
    public int? ParentId { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("deleted")]
    public bool IsDeleted { get; set; }
    public long? MeasureConfirmStatus { get; set; }
    public decimal? MeasureMaxValue { get; set; }
    public int? MeasureStatus { get; set; }
    public int? OriginId { get; set; }
    public long? PlaningLevelId { get; set; }
    public long? MeasureRepositoryId { get; set; }
    public long? ModirId { get; set; }
    [System.Text.Json.Serialization.JsonConverter(typeof(global::Common.utils.deserializer.OptionalLongDeserializer))]
    public long? TashilgarId { get; set; }
    public long? TashilgarHamkarId { get; set; }
    public bool AddToSupremeCom { get; set; }
    public string? TaskUrl { get; set; }
    public string? OutCome { get; set; }
    public string? MeasureName { get; set; }
    // TODO(ctor): سازنده‌ی چندآرگومانی جاوا — به‌صورت object initializer یا سازنده‌ی صریح منتقل شود:
    // 
}
