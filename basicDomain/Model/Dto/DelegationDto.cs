namespace SemBackDotNet10.basicDomain.Model.Dto;

/// <summary>معادل DelegationDto جاوا.</summary>
public class DelegationDto
{
    public long Id { get; set; }

    public long? FromUserId { get; set; }

    public long? ToUserId { get; set; }

    public string? FromUserName { get; set; }

    public string? FromUserDisplayName { get; set; }

    public string? ToUserName { get; set; }

    public string? ToUserDisplayName { get; set; }

    public DateTime? DelegationStartDate { get; set; }

    public DateTime? DelegationEndDate { get; set; }

    public bool DelegationStatus { get; set; }

    public string? DelegationStatusDesc { get; set; }
}
