namespace SemBackDotNet10.analysisDomain.model.dto.stakeHolder;

public class StakeHolderNeedsPriorityDto
{
    public long? StakeHolderNeedsPriorityId { get; set; }
    public long? StakeHolderId { get; set; }
    public string? NeedTitle { get; set; }
    public long? Importance { get; set; }
    public long? Impact { get; set; }
    public string? StakeHolderName { get; set; }

    public StakeHolderNeedsPriorityDto()
    {
    }

    public StakeHolderNeedsPriorityDto(long? stakeHolderNeedsPriorityId, long? stakeHolderId, string? needTitle,
        long? importance, long? impact, string? stakeHolderName)
    {
        StakeHolderNeedsPriorityId = stakeHolderNeedsPriorityId;
        StakeHolderId = stakeHolderId;
        NeedTitle = needTitle;
        Importance = importance;
        Impact = impact;
        StakeHolderName = stakeHolderName;
    }

    public StakeHolderNeedsPriorityDto(long? stakeHolderNeedsPriorityId, string? needTitle)
    {
        StakeHolderNeedsPriorityId = stakeHolderNeedsPriorityId;
        NeedTitle = needTitle;
    }
}
