using System;

namespace SemBackDotNet10.actionDomain.model.dto;

public class ActionActPlanDto
{
    public long? Id { get; set; }
    public long? PlanId { get; set; }
    public long? Month { get; set; }
    public decimal? PreviousPlanValue { get; set; }
    public decimal? PlanValue { get; set; }
    public decimal? PlanValueNew { get; set; }
    public decimal? ActualValue { get; set; }
    public decimal? ActualValueNew { get; set; }
    public long? ActionPlanCatalogId { get; set; }
    public long? ActionPlanId { get; set; }
    public string? Deviation { get; set; }
    public string? Correction { get; set; }
    public long? ActualBudget { get; set; }
    public bool IsPlanned { get; set; }
    public long? Confirmstatus { get; set; }
    public string? OutCome { get; set; }
    public DateTime? Correctiondeadlinedate { get; set; }
    public DateTime? Correctionstartdateaction { get; set; }
    public long? Correctionexecuter { get; set; }
    public decimal? CorrectionActualValue { get; set; }
    public string? ExecutorUserName { get; set; }
    public string? ExecutorDisplayName { get; set; }
    public string? Year { get; set; }
    public bool IsDeleted { get; set; }
    public float? ActionPlanWeight { get; set; }
    public long? RoyalVisionActId { get; set; }
    public long? CurrentMonth { get; set; }
}
