#nullable enable
using System;
using System.Collections.Generic;
using SemBackDotNet10.SecurityDomain.Model.Dto;

namespace SemBackDotNet10.basicDomain.Model.Dto;

public class UserDto
{
    public long? Id { get; set; }
    public long? UserAccountId { get; set; }
    public string? UserName { get; set; }
    public string? NameF { get; set; }
    public string? FamilyF { get; set; }
    public string? Fullname { get; set; }
    public bool Retired { get; set; }
    internal bool? LoginDisabled { get; set; }
    internal long? TenantId { get; set; }
    public string? CellPhoneNo { get; set; }
    public string? ExtensionNo { get; set; }
    public string? DesPost { get; set; }
    public long? PostId { get; set; }
    public string? DesOrganF { get; set; }
    public string? NationalCode { get; set; }
    public long? PlaningLevelId { get; set; }
    public string? DesLevel { get; set; }
    public List<PlaningLevelDto?>? PlaningLevelDtoList { get; set; }
    public List<RoleDto?>? Roles { get; set; }
    public DateTime? LastFailedLoginAttempt { get; set; }
    public DateTime? LastSuccessfulLoginAttempt { get; set; }
    public string? Password { get; set; }
    public long? PersonnelNumber { get; set; }

    public UserDto() { }

    public UserDto(long? id, string? userName, string? nameF, string? familyF, bool retired, string? cellPhoneNo, string? extensionNo, long? postId, string? desPost, long? planingLevelId, string? desLevel, string? nationalCode)
    {
        Id = id;
        UserName = userName;
        NameF = nameF;
        FamilyF = familyF;
        Retired = retired;
        CellPhoneNo = cellPhoneNo;
        ExtensionNo = extensionNo;
        PostId = postId;
        DesPost = desPost;
        PlaningLevelId = planingLevelId;
        DesLevel = desLevel;
        NationalCode = nationalCode;
    }

    public UserDto(long? id, long? userAccountId, string? nameF, string? familyF, string? userName)
    {
        Id = id;
        UserAccountId = userAccountId;
        NameF = nameF;
        FamilyF = familyF;
        UserName = userName;
    }

    public UserDto(long? id, long? userAccountId, string? nameF, string? familyF, string? userName, string? desPost, DateTime? lastFailedLoginAttempt, DateTime? lastSuccessfulLoginAttempt)
    {
        Id = id;
        UserAccountId = userAccountId;
        NameF = nameF;
        FamilyF = familyF;
        UserName = userName;
        DesPost = desPost;
        LastFailedLoginAttempt = lastFailedLoginAttempt;
        LastSuccessfulLoginAttempt = lastSuccessfulLoginAttempt;
    }

    public UserDto(long? id, string? nameF, string? familyF)
    {
        Id = id;
        NameF = nameF;
        FamilyF = familyF;
    }
}
