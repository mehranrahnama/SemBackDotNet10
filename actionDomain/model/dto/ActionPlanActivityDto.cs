// Mechanical conversion from Java (model/dto/ActionPlanActivityDto.java). Complex bodies are marked with TODO.
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

/// <summary>معادل ActionPlanActivityDto.java — lombok @Getter/@Setter/@NoArgsConstructor در C# به property عمومی تبدیل شد.</summary>
public class ActionPlanActivityDto
{
    public long? Id { get; set; }
    public string? ActivityName { get; set; }
    public string? DateStart { get; set; }
    public string? DateEnd { get; set; }
    public long? AccountableId { get; set; }
    public string? AccountableName { get; set; }
    public float? Weight { get; set; }
    public string? EntityId { get; set; }
    public bool HasUploadedFile { get; set; }
    public List<global::Common.controller.dto.response.StoredAttachmentResDto> Attachments { get; set; } = new();
    public long? ActionPlanCatalogId { get; set; }
}
