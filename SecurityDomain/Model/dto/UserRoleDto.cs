namespace SemBackDotNet10.SecurityDomain.Model.Dto;

public class UserRoleDto
{
    public long? Id { get; set; }
    public string? RoleName { get; set; }
    public long? UserId { get; set; }
    public string? RoleDes { get; set; }
    public bool? Check { get; set; }
    public List<RoleDto>? RoleDtoList { get; set; }

    public UserRoleDto()
    {
    }

    public UserRoleDto(long? id, string? roleName, bool? check, string? roleDes)
    {
        Id = id;
        RoleName = roleName;
        Check = check;
        RoleDes = roleDes;
    }

    public UserRoleDto(long? id, string? roleName)
    {
        Id = id;
        RoleName = roleName;
    }
}
