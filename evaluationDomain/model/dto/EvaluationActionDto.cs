// Mechanical conversion from Java (model/dto/EvaluationActionDto.java). Complex bodies are marked with TODO.
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



namespace SemBackDotNet10.evaluationDomain.model.dto;

/// <summary>معادل EvaluationActionDto.java — lombok @Getter/@Setter/@NoArgsConstructor در C# به property عمومی تبدیل شد.</summary>
public class EvaluationActionDto
{
    public string? DesLevel { get; set; }
    public long? Month { get; set; }
    public string? ActionManager { get; set; }
    public string? Position { get; set; }
    public string? Description { get; set; }
    public long? PlanId { get; set; }
    public List<string?> RaterNames { get; set; }
    public bool? Disabled { get; set; }
}
