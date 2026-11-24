using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.basicDomain.Model.Dto;
using SemBackDotNet10.Data;
using SemBackDotNet10.SecurityDomain.Infrastructure.Security;
using RoleDto = SemBackDotNet10.SecurityDomain.Model.Dto.RoleDto;
using SemBackDotNet10.SecurityDomain.Persistence;

namespace SemBackDotNet10.service.impl;

public sealed class UserPageReader(SemDbContext db, AppDbContext security, UserContext current)
{
    public async Task<ServicePage<UserDto>> FindAllAsync(string? filter, long levelId, ServicePageRequest page, CancellationToken ct)
    {
        if (levelId != 0)
        {
            var levelExists = await db.PlaningLevels.AnyAsync(level => level.Id == levelId && level.FactoryId == current.TenantId, ct);
            var allowed = current.IsAdministrator || await db.PlaningLevelUsers.AnyAsync(link => link.LevelId == levelId && link.UserId == current.UserId, ct);
            if (!levelExists || !allowed) throw new UnauthorizedAccessException();
        }

        var query = db.Users.AsNoTracking().Where(user => user.TenantId == current.TenantId);
        if (levelId != 0)
            query = query.Where(user => db.PlaningLevelUsers.Any(link => link.UserId == user.UserId && link.LevelId == levelId));
        if (!string.IsNullOrEmpty(filter))
        {
            var pattern = "%" + filter + "%";
            query = query.Where(user => EF.Functions.Like(user.UserId.ToString(), pattern) ||
                EF.Functions.Like((user.NameF ?? "") + " " + (user.FamilyF ?? ""), pattern) ||
                EF.Functions.Like(user.NationalCode!, pattern) ||
                db.PlaningLevelUsers.Any(link => link.UserId == user.UserId && link.Level != null &&
                    link.Level.FactoryId == current.TenantId && EF.Functions.Like(link.Level.DesLevel, pattern)));
        }

        var total = await query.LongCountAsync(ct);
        var descending = page.Direction == SortDirection.Descending;
        query = page.OrderBy.ToLowerInvariant() switch
        {
            "id" or "userid" => descending ? query.OrderByDescending(user => user.UserId) : query.OrderBy(user => user.UserId),
            "username" => descending ? query.OrderByDescending(user => user.UserName) : query.OrderBy(user => user.UserName),
            "namef" => descending ? query.OrderByDescending(user => user.NameF) : query.OrderBy(user => user.NameF),
            "familyf" => descending ? query.OrderByDescending(user => user.FamilyF) : query.OrderBy(user => user.FamilyF),
            _ => throw new ArgumentException("Unknown sort property.")
        };
        var users = await query.Skip(page.Offset).Take(page.Size).Select(user => new
        {
            user.UserId, user.UserName, user.NameF, user.FamilyF, user.Retired, user.CellPhoneNo,
            user.InternalPhoneNum, user.NationalCode, user.PersonnelNumber, user.PositionId
        }).ToListAsync(ct);
        var ids = users.Select(user => user.UserId).ToArray();
        var positionIds = users.Where(user => user.PositionId.HasValue).Select(user => (long)user.PositionId!.Value).Distinct().ToArray();
        var positions = await db.Set<basicDomain.Model.Entity.Position>().Where(position => positionIds.Contains(position.Id))
            .Select(position => new { position.Id, position.DesPost }).ToDictionaryAsync(position => position.Id, position => position.DesPost, ct);
        var memberships = await db.PlaningLevelUsers.AsNoTracking()
            .Where(link => link.UserId.HasValue && ids.Contains(link.UserId.Value) && link.Level != null && link.Level.FactoryId == current.TenantId)
            .Select(link => new { UserId = link.UserId!.Value, Level = new PlaningLevelDto
            {
                Id = link.Level!.Id, FactoryId = link.Level.FactoryId, Code = link.Level.Code,
                DesLevel = link.Level.DesLevel, DesThreeChar = link.Level.DesThreeChar,
                TypeLevel = link.Level.TypeLevel, CreateDate = link.Level.CreateDate,
                Operational = link.Level.Operational, ParentId = link.Level.ParentId,
                ResponsibleId = link.Level.ResponsibleId, FacilitatorId = link.Level.FacilitatorId,
                ResponsibleNameF = link.Level.Responsible == null ? null : link.Level.Responsible.NameF,
                ResponsibleFamilyF = link.Level.Responsible == null ? null : link.Level.Responsible.FamilyF,
                FacilitatorNameF = link.Level.Facilitator == null ? null : link.Level.Facilitator.NameF,
                FacilitatorFamilyF = link.Level.Facilitator == null ? null : link.Level.Facilitator.FamilyF
            }}).ToListAsync(ct);
        var roles = await security.UserRoles.AsNoTracking().Where(role => ids.Contains(role.UserId))
            .Select(role => new { role.UserId, Role = new RoleDto { Id = role.Role.Id, RoleName = role.Role.RoleName, RoleDes = role.Role.RoleDes } })
            .ToListAsync(ct);
        var result = users.Select(user => new UserDto
        {
            Id = user.UserId, UserName = user.UserName, NameF = user.NameF, FamilyF = user.FamilyF,
            Fullname = $"{user.NameF} {user.FamilyF}", Retired = user.Retired == true,
            CellPhoneNo = user.CellPhoneNo, ExtensionNo = user.InternalPhoneNum, NationalCode = user.NationalCode,
            PersonnelNumber = (long?)user.PersonnelNumber ?? user.UserId,
            DesPost = user.PositionId.HasValue ? positions.GetValueOrDefault(user.PositionId.Value) ?? "" : "",
            PlaningLevelDtoList = memberships.Where(link => link.UserId == user.UserId).Select(link => (PlaningLevelDto?)link.Level).ToList(),
            Roles = roles.Where(role => role.UserId == user.UserId).Select(role => (RoleDto?)role.Role).ToList()
        }).ToList();
        return new(result, total, page.Page, page.Size);
    }
}
