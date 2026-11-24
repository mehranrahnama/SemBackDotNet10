#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace SemBackDotNet10.baseDomain.repository;

public interface IDelegationRepository : ISemRepository<Delegation, long>
{
    Task<List<Delegation>> FindByFromUserAndToUserAndDelegationStartDateAsync(long fromUserId, long toUserId, CancellationToken ct = default);
    Task<PageResult<DelegationDto>> FindAllDelegationAsync(PageRequest page, string? filter, CancellationToken ct = default);
    Task<List<long>> FindAllByToUserAndCurrentDateAsync(long userId, DateTime date, CancellationToken ct = default);
    Task<List<Delegation>> FindAllByDelegationEndDateAsync(CancellationToken ct = default);
}

public sealed class DelegationRepository(DbContext db)
    : SemRepository<Delegation, long>(db), IDelegationRepository
{
    public Task<List<Delegation>>
        FindByFromUserAndToUserAndDelegationStartDateAsync(
            long fromUserId,
            long toUserId,
            CancellationToken ct = default)
    {
        return Set
            .Where(d =>
                d.FromUserId == fromUserId &&
                d.ToUserId == toUserId)
            .ToListAsync(ct);
    }


    public Task<PageResult<DelegationDto>> FindAllDelegationAsync(
        PageRequest page,
        string? filter,
        CancellationToken ct = default)
    {
        var query = Set
            .AsNoTracking()
            .Where(d =>
                d.DelegationStatus &&
                (
                    filter == null ||

                    (
                        d.FromUser != null &&
                        EF.Functions.Like(
                            (d.FromUser.NameF ?? "") + " " +
                            (d.FromUser.FamilyF ?? ""),
                            "%" + filter + "%")
                    ) ||

                    (
                        d.ToUser != null &&
                        EF.Functions.Like(
                            (d.ToUser.NameF ?? "") + " " +
                            (d.ToUser.FamilyF ?? ""),
                            "%" + filter + "%")
                    )
                ))
            .OrderBy(d => d.Id)
            .Select(d => new DelegationDto
            {
                Id = d.Id,

                FromUserId = d.FromUserId,
                ToUserId = d.ToUserId,

                FromUserName = d.FromUser != null
                    ? d.FromUser.UserName
                    : null,

                FromUserDisplayName = d.FromUser != null
                    ? (d.FromUser.NameF ?? "") + " " +
                      (d.FromUser.FamilyF ?? "")
                    : null,

                ToUserName = d.ToUser != null
                    ? d.ToUser.UserName
                    : null,

                ToUserDisplayName = d.ToUser != null
                    ? (d.ToUser.NameF ?? "") + " " +
                      (d.ToUser.FamilyF ?? "")
                    : null,

                DelegationStartDate = d.DelegationStartDate,
                DelegationEndDate = d.DelegationEndDate,
                DelegationStatus = d.DelegationStatus,

                DelegationStatusDesc = d.DelegationStatus
                    ? "فعال"
                    : "غیرفعال"
            });

        return PageAsync(query, page, ct);
    }


    public Task<List<long>> FindAllByToUserAndCurrentDateAsync(long userId, DateTime date, CancellationToken ct = default)
    {
        return Db.Database.SqlQuery<long>($"SELECT CAST(d.fromUser AS bigint) AS [Value] FROM dbo.tblDelegation d WHERE d.toUser={userId} AND d.delegationStatus=1 AND {date} BETWEEN d.delegationStartDate AND d.delegationEndDate").ToListAsync(ct);
    }

    public Task<List<Delegation>> FindAllByDelegationEndDateAsync(CancellationToken ct = default)
    {
        return Set.FromSqlRaw("SELECT * FROM dbo.tblDelegation d WHERE DATEDIFF(second,d.delegationEndDate,SYSDATETIME()) >= 0").ToListAsync(ct);
    }
}
