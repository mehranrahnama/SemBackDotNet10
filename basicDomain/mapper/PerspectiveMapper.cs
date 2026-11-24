#nullable enable


namespace SemBackDotNet10.baseDomain.mapper;

public interface IPerspectiveMapper
{
    Perspective? CloneForPlanningLevel(
        Perspective? source,
        PlaningLevel? planningLevel,
        Perspective? parent);
}

public sealed class PerspectiveMapper : IPerspectiveMapper
{
    public Perspective? CloneForPlanningLevel(
        Perspective? source,
        PlaningLevel? planningLevel,
        Perspective? parent)
    {
        // MapStruct returns null when all source parameters are null.
        if (source is null && planningLevel is null && parent is null)
            return null;

        var result = new Perspective();

        if (source is not null)
        {
            result.PlanPeriod = source.PlanPeriod;
            if (source.PlanPeriod is not null)
                result.PlanId = source.PlanPeriod.PlanId;

            result.DesperspectF = source.DesperspectF;
            result.DesperspectE = source.DesperspectE;
            result.Weight = source.Weight;
            result.Orders = source.Orders;
            result.Rank = source.Rank;
            result.Code = source.Code;
        }

        result.PlaningLevel = planningLevel;
        result.Parent = parent;

        // Id, OriginId, Version, CreatedDate, CreatedBy,
        // ModifiedDate and ModifiedBy intentionally retain their defaults.
        return result;
    }
}
