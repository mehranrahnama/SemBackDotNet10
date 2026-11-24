#nullable enable

using SemBackDotNet10.baseDomain.context;

namespace SemBackDotNet10.baseDomain.resolver;

public interface IStrategyObjectResolver
{
    StrategyObject Resolve(FindOrCreateStrategyResolutionContext context);
}
