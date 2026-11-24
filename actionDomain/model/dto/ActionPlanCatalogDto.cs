// Mechanical conversion from Java (model/dto/ActionPlanCatalogDto.java). Complex bodies are marked with TODO.
// وابستگی‌های اعلام‌شده: basicDomain/Common/base هنوز ارسال نشده‌اند.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;



namespace SemBackDotNet10.actionDomain.model.dto;

/// <summary>معادل ActionPlanCatalogDto.java — lombok @Getter/@Setter/@NoArgsConstructor در C# به property عمومی تبدیل شد.</summary>
public class ActionPlanCatalogDto
{
    public long? Id { get; set; }
    public string? Code { get; set; }
    public string? Description { get; set; }
    public string? StartDate { get; set; }
    public string? EndDate { get; set; }
    public int? TotalBudget { get; set; }
    public long? ActionConfirmStatus { get; set; }
    public long? LevelId { get; set; }
    public string? DesLevel { get; set; }
    public bool? AddToSupremeLevel { get; set; }
    public bool? DeleteStatus { get; set; }
    public long? PlanId { get; set; }
    public long? PayeshgarId { get; set; }
    public long? ModirId { get; set; }
    public string? Payeshgar { get; set; }
    public string? Modir { get; set; }
    public string? OutCome { get; set; }
    public string? DesThreeChar { get; set; }
    public DateTime? CreateDate { get; set; }
    // TODO(ctor): سازنده‌ی چندآرگومانی جاوا — به‌صورت object initializer یا سازنده‌ی صریح منتقل شود:
    // 
}
