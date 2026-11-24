namespace SemBackDotNet10.webserviceDomain.domain.dto;

public class ActionPlanWebServiceProjection
{
    public ActionPlanWebServiceProjection()
    {
    }

    public ActionPlanWebServiceProjection(
        long? actionPlanCatalogId, string? actionPlanTitle, string? startDate, string? dueDate,
        long? superVisorPersonId, string? superVisorPersonFullName, long? managerPersonId,
        string? managerPersonFullName, string? chartName, string? chartCode, float weight)
    {
        ActionPlanCatalogId = actionPlanCatalogId;
        ActionPlanTitle = actionPlanTitle;
        StartDate = startDate;
        DueDate = dueDate;
        SuperVisorPersonId = superVisorPersonId;
        SuperVisorPersonFullName = superVisorPersonFullName;
        ManagerPersonId = managerPersonId;
        ManagerPersonFullName = managerPersonFullName;
        ChartName = chartName;
        ChartCode = chartCode;
        Weight = weight;
    }

    public long? ActionPlanCatalogId { get; set; }
    public string? ActionPlanTitle { get; set; }
    public string? StartDate { get; set; }
    public string? DueDate { get; set; }
    public long? SuperVisorPersonId { get; set; }
    public string? SuperVisorPersonFullName { get; set; }
    public long? ManagerPersonId { get; set; }
    public string? ManagerPersonFullName { get; set; }
    public string? ChartName { get; set; }
    public string? ChartCode { get; set; }
    public float Weight { get; set; }
}
