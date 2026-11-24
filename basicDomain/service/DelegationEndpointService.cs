using Common.exception;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.baseDomain.repository;

namespace SemBackDotNet10.basicDomain.service;

public sealed class DelegationEndpointService(SemDbContext db)
{
    private SemBackDotNet10.baseDomain.repository.DelegationRepository Repository => new(db);

    public Task<PageResult<DelegationDto>> PageAsync(int page, int size, string? filter, CancellationToken ct)
        => Repository.FindAllDelegationAsync(new PageRequest(page, size), filter, ct);

    public async Task<DelegationDto> SaveAsync(DelegationDto dto, bool edit, CancellationToken ct)
    {
        var item = edit ? await db.Delegations.Include(d => d.FromUser).Include(d => d.ToUser)
            .SingleOrDefaultAsync(d => d.Id == dto.Id, ct) ?? throw new BusinessException("چنین تفویضی وجود ندارد")
            : new Delegation();
        if (edit && item.DelegationStartDate == dto.DelegationStartDate && item.DelegationEndDate == dto.DelegationEndDate)
            return Map(item);
        if (dto.DelegationStartDate is not DateTime start || start < DateTime.Today)
            throw new BusinessException("تاریخ شروع معتبر نیست");
        if (dto.DelegationEndDate is not DateTime end || end < start)
            throw new BusinessException("تاریخ پایان نمی تواند قبل از تاریخ شروع باشد");
        if (edit && (item.FromUserId != dto.FromUserId || item.ToUserId != dto.ToUserId))
            throw new BusinessException("شما مجاز به تغییر کاربر نمی باشید");
        if (!edit)
        {
            if (dto.FromUserId is not long from || dto.ToUserId is not long to)
                throw new BusinessException("کاربر پیدا نشد");
            if ((await Repository.FindByFromUserAndToUserAndDelegationStartDateAsync(from, to, ct)).Count != 0)
                throw new BusinessException("برای این کاربر تفویض فعال وجود دارد");
            item.FromUser = await db.Users.SingleOrDefaultAsync(u => u.UserId == from, ct)
                ?? throw new BusinessException("کاربر پیدا نشد (fromUser)");
            item.ToUser = await db.Users.SingleOrDefaultAsync(u => u.UserId == to, ct)
                ?? throw new BusinessException("کاربر پیدا نشد (toUser)");
            item.FromUserId = from;
            item.ToUserId = to;
            db.Delegations.Add(item);
        }
        item.DelegationStartDate = start;
        item.DelegationEndDate = end;
        item.DelegationStatus = true;
        await db.SaveChangesAsync(ct);
        return Map(item);
    }

    public async Task<bool> DeactivateAsync(long id, CancellationToken ct)
    {
        var item = await Repository.FindByIdAsync(id, ct);
        if (item is null) return false;
        item.DelegationStatus = false;
        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeactivateExpiredAsync(CancellationToken ct)
    {
        foreach (var item in await Repository.FindAllByDelegationEndDateAsync(ct))
            item.DelegationStatus = false;
        await db.SaveChangesAsync(ct);
        return true;
    }

    private static DelegationDto Map(Delegation item) => new()
    {
        Id = item.Id,
        FromUserId = item.FromUserId, ToUserId = item.ToUserId,
        FromUserName = item.FromUser?.UserName, ToUserName = item.ToUser?.UserName,
        FromUserDisplayName = item.FromUser is { } from ? $"{from.NameF} {from.FamilyF}" : null,
        ToUserDisplayName = item.ToUser is { } to ? $"{to.NameF} {to.FamilyF}" : null,
        DelegationStartDate = item.DelegationStartDate, DelegationEndDate = item.DelegationEndDate,
        DelegationStatus = item.DelegationStatus, DelegationStatusDesc = item.DelegationStatus ? "فعال" : "غیرفعال"
    };
}
