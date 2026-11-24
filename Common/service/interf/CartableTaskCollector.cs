using Common.model.entity;
namespace Common.service.interf;

// Partial extraction of CartableService.getTaskList only. Not a replacement for CartableService.
public enum CartableBranch
{
    MeasureCatalog, DeletedMeasureCatalog, Measure, MeasureReplan, DeletedMeasure,
    MeasureProgress, EvaluationMeasure, ActionPlanCatalog, ActionReplan,
    DeletedActionPlanCatalog, ActionPlan, DeletedActionPlan, ActionProgress,
    EvaluationAction, EvaluationFeedback
}
public interface ICartableTaskQueries
{
    IReadOnlyList<string> GetAllEntityNames();
    // Implementations must preserve Java filtering, authorization, mapping and ordering.
    // No default empty implementation is supplied: missing integration must fail explicitly.
    IReadOnlyList<Cartable> Query(CartableBranch branch, long? planId, string entityName,
        long? userId, string? filter, bool? completed);
}
public sealed class CartableTaskCollector(ICartableTaskQueries queries)
{
    public List<Cartable> GetTaskList(long? planId, long? userId, string? filter,
        bool? completed, string? orderBy, string? sortDirection)
    {
        // Java accepts but does not use orderBy/sortDirection in this method.
        var result = new List<Cartable>();
        foreach (var entity in queries.GetAllEntityNames())
        {
            CartableBranch[] branches = entity switch
            {
                "MeasureCatalog" => [CartableBranch.MeasureCatalog, CartableBranch.DeletedMeasureCatalog],
                "Measure" => [CartableBranch.Measure, CartableBranch.MeasureReplan, CartableBranch.DeletedMeasure],
                "MeasureActPlan" => [CartableBranch.MeasureProgress],
                "EvaluationMeasure" => [CartableBranch.EvaluationMeasure],
                "ActionPlanCatalog" => [CartableBranch.ActionPlanCatalog, CartableBranch.ActionReplan, CartableBranch.DeletedActionPlanCatalog],
                "ActionPlan" => [CartableBranch.ActionPlan, CartableBranch.DeletedActionPlan],
                "ActionActPlan" => [CartableBranch.ActionProgress],
                "EvaluationAction" => [CartableBranch.EvaluationAction],
                "EvaluationFeedback" => [CartableBranch.EvaluationFeedback],
                _ => []
            };
            foreach (var branch in branches)
                result.AddRange(queries.Query(branch, planId, entity, userId, filter, completed));
        }
        return result;
    }
}
