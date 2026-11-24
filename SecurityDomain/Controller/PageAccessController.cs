using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.Controllers;
using SemBackDotNet10.SecurityDomain.Infrastructure.Security;
using SemBackDotNet10.SecurityDomain.Model.Dto;
using SemBackDotNet10.SecurityDomain.Persistence;

namespace SemBackDotNet10.SecurityDomain.Controller;

[ApiController]
[Route("api/v1")]
public sealed class PageAccessController(AppDbContext db, UserContext current) : ControllerBase
{
    [HttpGet("pageAllData/list/byUser")]
    [CheckAccess]
    public async Task<IActionResult> GetPageData(CancellationToken ct)
    {
        var userId = current.UserId;
        var tenantId = current.TenantId;
        var isAdmin = User.IsInRole("Administrators");
        var pages = await db.Database.SqlQuery<PageAccessDto>($"""
            SELECT CAST(p.PageId AS bigint) AS Id, p.DesF, p.Uri, p.PageName
            FROM dbo.tblPage p
            WHERE {isAdmin} = 1 OR EXISTS (
                SELECT 1 FROM dbo.tblPageAccess pa
                JOIN dbo.tblUserRoles ur ON ur.RoleId = pa.RoleId
                JOIN dbo.tblUser u ON u.UserId = ur.UserId
                WHERE pa.PageId = p.PageId AND u.UserId = {userId} AND u.TenantId = {tenantId})
            """).ToListAsync(ct);
        return Ok(pages);
    }
}
