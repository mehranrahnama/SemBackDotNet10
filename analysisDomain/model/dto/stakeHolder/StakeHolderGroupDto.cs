namespace SemBackDotNet10.analysisDomain.model.dto.stakeHolder;

public class StakeHolderGroupDto
{
    public long? StakeHolderGroupId { get; set; }
    public string? StakeHolderGroupTitle { get; set; }
    public long? PlanId { get; set; }

    public StakeHolderGroupDto()
    {
    }

    public StakeHolderGroupDto(long? stakeHolderGroupId, string? stakeHolderGroupTitle, long? planId)
    {
        StakeHolderGroupId = stakeHolderGroupId;
        StakeHolderGroupTitle = stakeHolderGroupTitle;
        PlanId = planId;
    }

    public StakeHolderGroupDto(long? stakeHolderGroupId, string? stakeHolderGroupTitle)
    {
        StakeHolderGroupId = stakeHolderGroupId;
        StakeHolderGroupTitle = stakeHolderGroupTitle;
    }
}
