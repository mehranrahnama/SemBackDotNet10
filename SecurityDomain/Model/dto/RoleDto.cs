namespace SemBackDotNet10.SecurityDomain.Model.Dto;

public class RoleDto
{
    public long? Id { get; set; }
    public string? RoleName { get; set; }
    public string? RoleDes { get; set; }

    public RoleDto()
    {
    }

    public RoleDto(long? id, string? roleName, string? roleDes)
    {
        Id = id;
        RoleName = roleName;
        RoleDes = roleDes;
    }

    public RoleDto(string? roleName)
    {
        RoleName = roleName;
    }
}
