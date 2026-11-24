#nullable enable

namespace SemBackDotNet10.baseDomain.mapper;

public interface IStrategyObjectMapper
{
    StrategyObject? CloneForPlanningLevel(
        StrategyObject? source,
        PlaningLevel? planningLevel,
        Perspective? perspective);

    StrategyObject? CloneForTarget(
        StrategyObject? source,
        PlaningLevel? targetLevel,
        PlanPeriod? targetPlanPeriod,
        Perspective? perspective);
}

public sealed class StrategyObjectMapper : IStrategyObjectMapper
{
    public StrategyObject? CloneForPlanningLevel(
        StrategyObject? source,
        PlaningLevel? planningLevel,
        Perspective? perspective)
    {
        if (source is null && planningLevel is null && perspective is null)
            return null;

        var result = new StrategyObject();

        if (source is not null)
        {
            result.Title = source.Title;
            result.Code = source.Code;
            result.Weight = source.Weight;
            result.PlanPeriod = source.PlanPeriod;
            result.Deleted = source.Deleted;
        }

        result.Perspective = perspective;
        result.PlaningLevel = planningLevel;

        // Id, MainStrategyId, ThemeId, OriginId, Version, CreatedDate,
        // CreatedBy, ModifiedDate and ModifiedBy retain their defaults.
        return result;
    }

    public StrategyObject? CloneForTarget(
        StrategyObject? source,
        PlaningLevel? targetLevel,
        PlanPeriod? targetPlanPeriod,
        Perspective? perspective)
    {
        if (source is null && targetLevel is null &&
            targetPlanPeriod is null && perspective is null)
            return null;

        var result = new StrategyObject();

        if (source is not null)
        {
            result.Title = source.Title;
            result.Code = source.Code;
            result.Weight = source.Weight;
            result.Deleted = source.Deleted;
        }

        result.Perspective = perspective;
        result.PlaningLevel = targetLevel;
        result.PlanPeriod = targetPlanPeriod;

        // Id, MainStrategyId, ThemeId, OriginId, Version, CreatedDate,
        // CreatedBy, ModifiedDate and ModifiedBy retain their defaults.
        return result;
    }
}
