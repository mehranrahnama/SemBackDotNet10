using SemBackDotNet10.analysisDomain.model.entity.enums;

namespace SemBackDotNet10.analysisDomain.model.dto.stakeHolder;

public class StakeHolderDto
{
    public long? StakeHolderId { get; set; }
    public long? StakeholderGroupId { get; set; }
    public string? StakeHolderGroupTitle { get; set; }
    public string? StakeholderTitle { get; set; }
    public double? Power { get; set; }
    public double? Interest { get; set; }
    public BehaviorStrategy? BehaviorStrategy { get; set; }

    public StakeHolderDto()
    {
    }

    public StakeHolderDto(long? stakeHolderId, long? stakeholderGroupId, string? stakeHolderGroupTitle,
        string? stakeholderTitle, double? power, double? interest, BehaviorStrategy? behaviorStrategy)
    {
        StakeHolderId = stakeHolderId;
        StakeholderGroupId = stakeholderGroupId;
        StakeHolderGroupTitle = stakeHolderGroupTitle;
        StakeholderTitle = stakeholderTitle;
        Power = power;
        Interest = interest;
        BehaviorStrategy = behaviorStrategy;
    }

    public StakeHolderDto(long? stakeHolderId, string? stakeholderTitle)
    {
        StakeHolderId = stakeHolderId;
        StakeholderTitle = stakeholderTitle;
    }

    public StakeHolderDto(long? stakeHolderId, string? stakeholderTitle, BehaviorStrategy? behaviorStrategy)
    {
        StakeHolderId = stakeHolderId;
        StakeholderTitle = stakeholderTitle;
        BehaviorStrategy = behaviorStrategy;
    }

    public StakeHolderDto(long? stakeHolderId, long? stakeholderGroupId, string? stakeholderTitle)
    {
        StakeHolderId = stakeHolderId;
        StakeholderGroupId = stakeholderGroupId;
        StakeholderTitle = stakeholderTitle;
    }

    public StakeHolderDto(long? stakeHolderId, string? stakeholderTitle, long? stakeholderGroupId, string? stakeHolderGroupTitle)
    {
        StakeHolderId = stakeHolderId;
        StakeholderTitle = stakeholderTitle;
        StakeholderGroupId = stakeholderGroupId;
        StakeHolderGroupTitle = stakeHolderGroupTitle;
    }

    public StakeHolderDto(long? stakeHolderId, string? stakeholderTitle, long? stakeholderGroupId, string? stakeHolderGroupTitle,
        double? power, double? interest)
    {
        StakeHolderId = stakeHolderId;
        StakeholderTitle = stakeholderTitle;
        StakeholderGroupId = stakeholderGroupId;
        StakeHolderGroupTitle = stakeHolderGroupTitle;
        Power = power;
        Interest = interest;
    }
}
