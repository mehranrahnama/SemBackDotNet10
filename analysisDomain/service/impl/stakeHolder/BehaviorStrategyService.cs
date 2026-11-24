using SemBackDotNet10.analysisDomain.model.dto.stakeHolder;
using SemBackDotNet10.analysisDomain.model.entity.enums;

namespace SemBackDotNet10.analysisDomain.service.impl.stakeHolder;

public class BehaviorStrategyService
{
    public List<BehaviorStrategyDto> GetAllBehaviorStrategy() =>
        Enum.GetValues<BehaviorStrategy>()
            .Select(v => new BehaviorStrategyDto(v.GetId(), v.GetPersianName()))
            .ToList();

    public BehaviorStrategy FindByIdOrElseThrow(long id)
    {
        foreach (var strategy in Enum.GetValues<BehaviorStrategy>())
        {
            if (strategy.GetId() == id)
                return strategy;
        }
        throw new ArgumentException("Invalid BehaviorStrategy id: " + id);
    }
}
