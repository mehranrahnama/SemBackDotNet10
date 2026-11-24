// Mechanical conversion from Java (model/dto/EvaluationFeedBackDto.java). Complex bodies are marked with TODO.
// وابستگی‌های اعلام‌شده: basicDomain/Common/base هنوز ارسال نشده‌اند.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SemBackDotNet10.evaluationDomain.model.entity;
using SemBackDotNet10.evaluationDomain.model.dto;
using SemBackDotNet10.evaluationDomain.repository;
using SemBackDotNet10.evaluationDomain.service.interf;
using global::Common.model.entity;
using SemBackDotNet10.basicDomain.Model.Entity;
using SemBackDotNet10.basicDomain.Model.Dto;
using SemBackDotNet10.baseDomain.repository;
using SemBackDotNet10.basicDomain.service;
using SemBackDotNet10.actionDomain.model.entity;
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
using global::Common.logging;
using global::Common.model.dto;
using global::Common.utils.cartable;
using global::Common.utils.security;
using global::Common.utils.userRoles;


namespace SemBackDotNet10.evaluationDomain.model.dto;

/// <summary>معادل EvaluationFeedBackDto.java — lombok @Getter/@Setter/@NoArgsConstructor در C# به property عمومی تبدیل شد.</summary>
public class EvaluationFeedBackDto
{
    public long? Id { get; set; }
    public double? Weight { get; set; }
    public EvaluationFeedBackQuestionDto EvaluationFeedBackQuestionDto { get; set; }
    public string? StrengthPoint { get; set; }
    public string? StrengthImprovement { get; set; }
    public decimal? Score { get; set; }
    public long? LevelId { get; set; }
    public string? DesLevel { get; set; }
    public long? PlanId { get; set; }
    public long? FromMonth { get; set; }
    public long? ToMonth { get; set; }
    public string? MasterRater { get; set; }
    public string? Raters { get; set; }
    public long? ConfirmStatus { get; set; }
}
