namespace SemBackDotNet10.SecurityDomain.Model.Dto;

public sealed class UserDto
{
    public long Id { get; set; }
    public string? UserName { get; set; }
    public string? NameF { get; set; }
    public string? FamilyF { get; set; }
    public string? NationalCode { get; set; }
    public long? TenantId { get; set; }
    public bool? Retired { get; set; }
    public bool? LoginDisabled { get; set; }
}
