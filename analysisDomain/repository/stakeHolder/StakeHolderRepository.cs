using global::SemBackDotNet10.Common.Pagination;
using Common.utils;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.analysisDomain.model.dto.stakeHolder;
using SemBackDotNet10.analysisDomain.model.entity.stakeHolder;

using SemBackDotNet10.Data;
using System.Linq.Dynamic.Core;

namespace SemBackDotNet10.analysisDomain.repository.stakeHolder;

public interface IStakeHolderRepository : IBaseRepository<StakeHolder>
{
    Task<global::SemBackDotNet10.Common.Pagination.PagedResult<StakeHolderDto>> FindAllStakeHoldersByStakeHolderGroupIdAsync(PageRequest pageable, string filter, long? planId, long? groupId, CancellationToken ct = default);

    Task<List<StakeHolder>> FindStakeHolderByStakeHolderGroupIdAsync(long? stakeHolderGroupId, CancellationToken ct = default);

    Task<List<StakeHolderDto>> FindAllStakeHoldersByPlanIdWithPriorityAsync(long? planId, CancellationToken ct = default);
}

public class StakeHolderRepository : BaseRepository<StakeHolder>, IStakeHolderRepository
{
    public StakeHolderRepository(SemDbContext context) : base(context) { }

    public async Task<global::SemBackDotNet10.Common.Pagination.PagedResult<StakeHolderDto>> FindAllStakeHoldersByStakeHolderGroupIdAsync(PageRequest pageable, string filter, long? planId, long? groupId, CancellationToken ct = default)
    {
        var query = Query
            .Where(stk => stk.StakeHolderGroup != null)
            .Where(stk => groupId == null || stk.StakeHolderGroupId == groupId)
            .Where(stk => planId == null || stk.StakeHolderGroup!.PlanId == planId)
            .Where(stk => filter == null || stk.StakeHolderTitle.Contains(filter))
            .Select(stk => new StakeHolderDto
            {
                StakeHolderId = stk.Id,
                StakeholderGroupId = stk.StakeHolderGroupId,
                StakeholderTitle = stk.StakeHolderTitle
            });

        var total = await query.LongCountAsync(ct);
        var items = await query.Apply(pageable).ToListAsync(ct);
        return new global::SemBackDotNet10.Common.Pagination.PagedResult<StakeHolderDto> { Items = items, TotalElements = total, Page = pageable.Page, Size = pageable.Size };
    }

    public Task<List<StakeHolder>> FindStakeHolderByStakeHolderGroupIdAsync(long? stakeHolderGroupId, CancellationToken ct = default) =>
        Query.Where(stk => stk.StakeHolderGroupId == stakeHolderGroupId).ToListAsync(ct);

    public async Task<List<StakeHolderDto>> FindAllStakeHoldersByPlanIdWithPriorityAsync(long? planId, CancellationToken ct = default)
    {
        // در JPQL مقایسه با null هیچ ردیفی برنمی‌گرداند
        if (planId == null)
            return new List<StakeHolderDto>();

        return await Query
            .Where(stk => stk.StakeHolderGroup != null && stk.StakeHolderGroup.PlanId == planId
                          && stk.Power != null && stk.Interest != null)
            .Select(stk => new StakeHolderDto
            {
                StakeHolderId = stk.Id,
                StakeholderTitle = stk.StakeHolderTitle,
                StakeholderGroupId = stk.StakeHolderGroupId,
                StakeHolderGroupTitle = stk.StakeHolderGroup!.StakeHolderGroupTitle,
                Power = stk.Power,
                Interest = stk.Interest
            })
            .ToListAsync(ct);
    }
}
