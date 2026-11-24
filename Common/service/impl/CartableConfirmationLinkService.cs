using Common.model.dto;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.Data;

namespace Common.service.impl;

public sealed record ConfirmationLinkRequest(string? EntityName, string? OperationType, string? Outcome);

public sealed class CartableConfirmationLinkService(SemDbContext db)
{
    public Task<List<CartableStatusDto>> FindAsync(ConfirmationLinkRequest request, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);
        return db.Database.SqlQuery<CartableStatusDto>($"""
            SELECT CAST(cs.CartableStatusId AS bigint) AS Id, cs.EntityName, cs.OutCome,
                   CAST(cs.OldStatus AS bigint) AS OldStatus, CAST(cs.NewStatus AS bigint) AS NewStatus,
                   cs.OperationType, CAST(cs.FromRoleId AS bigint) AS FromRoleId, fr.RoleDes AS FromRoleName,
                   CAST(cs.ToRoleId AS bigint) AS ToRoleId, tr.RoleDes AS ToRoleName,
                   cs.OldStatusMeaning, cs.NewStatusMeaning
            FROM dbo.tblCartableStatus cs
            INNER JOIN dbo.tblRoles fr ON fr.RoleId = cs.FromRoleId
            LEFT JOIN dbo.tblRoles tr ON tr.RoleId = cs.ToRoleId
            WHERE cs.EntityName = {request.EntityName}
              AND ({request.OperationType} IS NULL OR cs.OperationType = {request.OperationType})
              AND cs.OutCome = {request.Outcome}
            ORDER BY cs.OldStatus
            """).ToListAsync(ct);
    }
}
