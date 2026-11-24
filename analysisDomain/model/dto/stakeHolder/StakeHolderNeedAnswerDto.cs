using SemBackDotNet10.analysisDomain.model.entity.enums;

namespace SemBackDotNet10.analysisDomain.model.dto.stakeHolder;

public class StakeHolderNeedAnswerDto
{
    public StakeHolderDto? StakeHolderDto { get; set; }
    public StakeHolderNeedsPriorityDto? StakeHolderNeedsPriorityDto { get; set; }
    public AnsweringType? AnsweringType { get; set; }
    public AnsweringTypeDto? AnsweringTypeDto { get; set; }
    public BehaviorStrategy? BehaviorStrategy { get; set; }

    public StakeHolderNeedAnswerDto()
    {
    }

    public StakeHolderNeedAnswerDto(long? stakeHolderId, string? stakeHolderTitle,
        long? stakeHolderNeedsId, string? stakeHolderNeedsTitle, AnsweringType? answeringType, BehaviorStrategy? behaviorStrategy)
    {
        StakeHolderDto = new StakeHolderDto(stakeHolderId, stakeHolderTitle, behaviorStrategy);
        StakeHolderNeedsPriorityDto = new StakeHolderNeedsPriorityDto(stakeHolderNeedsId, stakeHolderNeedsTitle);
        AnsweringType = answeringType;
    }
}
