namespace SemBackDotNet10.SecurityDomain.Model.Dto;

public sealed class UserAccountDto
{
    public long Id { get; set; }
    public string? AccountName { get; set; }
    public string? Password { get; set; }
    public long? UserId { get; set; }
    public global::SemBackDotNet10.basicDomain.Model.Dto.UserDto? UserDto { get; set; }
    public long? ProviderId { get; set; }
}
