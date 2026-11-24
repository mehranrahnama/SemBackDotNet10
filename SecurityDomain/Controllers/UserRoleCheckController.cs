using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.Controllers;
using SemBackDotNet10.SecurityDomain.Persistence;
using SemBackDotNet10.SecurityDomain.Service.Interf;
using SemBackDotNet10.SecurityDomain.Tenancy;

namespace SemBackDotNet10.SecurityDomain.Controllers;

[ApiController]
[Route("api/v1/roles")]
public sealed class UserRoleCheckController(AppDbContext db, TenantContext tenant, IUserRoleService roles) : ControllerBase
{
    [HttpGet("by-user-check/{userId:long}")]
    [CheckAccess]
    public async Task<IActionResult> GetByUser(long userId, CancellationToken cancellationToken)
    {
        if (!tenant.HasTenant)
            return Forbid();

        if (userId is <= 0 or > int.MaxValue || !await db.Users.AnyAsync(
                user => user.UserId == userId && user.TenantId == tenant.TenantId, cancellationToken))
            return NotFound();

        return Ok(roles.FindUesrRole(userId));
    }
}
