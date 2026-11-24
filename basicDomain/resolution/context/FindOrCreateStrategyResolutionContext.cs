#nullable enable

using SemBackDotNet10.baseDomain.enums;

namespace SemBackDotNet10.baseDomain.context;

public sealed record FindOrCreateStrategyResolutionContext(
    PlaningLevel TargetLevel,
    StrategyObject Source,
    long? CatalogId,
    ResolutionRelationType RelationType
);
