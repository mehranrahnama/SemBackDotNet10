using PageRequest = global::Common.utils.PageRequest;
using global::SemBackDotNet10.Common.Pagination;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.analysisDomain.model.dto.stakeHolder;
using SemBackDotNet10.analysisDomain.model.entity.stakeHolder;
using SemBackDotNet10.basicDomain.Repositories;
using global::Common.model.dto;
using SemBackDotNet10.Common.utils;
using SemBackDotNet10.Data;

namespace SemBackDotNet10.analysisDomain.repository.stakeHolder;

public interface IStakeHolderGroupRepository : IBaseRepository<StakeHolderGroup>
{
    Task<PagedResult<StakeHolderGroupDto>> FindAllAsync(PageRequest pageable, string filter, long? planId, CancellationToken ct = default);

    Task<List<StakeHolderGroupDto>> FindAllByPlanIdAsync(long planId, CancellationToken ct = default);
}

public class StakeHolderGroupRepository : BaseRepository<StakeHolderGroup>, IStakeHolderGroupRepository
{
    public StakeHolderGroupRepository(SemDbContext context) : base(context) { }

    public async Task<PagedResult<StakeHolderGroupDto>> FindAllAsync(PageRequest pageable, string filter, long? planId, CancellationToken ct = default)
    {
        // در JPQL مقایسه planId با null هیچ ردیفی برنمی‌گرداند
        if (planId == null)
            return new PagedResult<StakeHolderGroupDto> { Items = new List<StakeHolderGroupDto>(), TotalElements = 0, Page = pageable.Page, Size = pageable.Size };

        var query = Query
            .Where(g => g.PlanId == planId && g.StakeHolderGroupTitle.Contains(filter))
            .Select(g => new StakeHolderGroupDto
            {
                StakeHolderGroupId = g.Id,
                StakeHolderGroupTitle = g.StakeHolderGroupTitle,
                PlanId = g.PlanId
            });

        var total = await query.LongCountAsync(ct);
        var items = await global::Common.utils.PageUtils.Apply(query, pageable).ToListAsync(ct);
        return new PagedResult<StakeHolderGroupDto> { Items = items, TotalElements = total, Page = pageable.Page, Size = pageable.Size };
    }

    public async Task<List<StakeHolderGroupDto>> FindAllByPlanIdAsync(long planId, CancellationToken ct = default) =>
        await Query
            .Where(g => g.PlanId == planId)
            .Select(g => new StakeHolderGroupDto
            {
                StakeHolderGroupId = g.Id,
                StakeHolderGroupTitle = g.StakeHolderGroupTitle,
                PlanId = g.PlanId
            })
            .ToListAsync(ct);
}
