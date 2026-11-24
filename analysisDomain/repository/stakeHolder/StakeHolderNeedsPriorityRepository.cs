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

public interface IStakeHolderNeedsPriorityRepository : IBaseRepository<StakeholderNeedsPriority>
{
    Task<PagedResult<StakeHolderNeedsPriorityDto>> FindAllStakeHolderNeedsPriorityPageAsync(long? stakeHolderId, string? filter, PageRequest pageable, CancellationToken ct = default);

    Task<List<StakeHolderNeedsPriorityDto>> FindAllStakeHolderNeedsPriorityListAsync(long? stakeHolderId, string? filter, CancellationToken ct = default);

    Task<List<StakeHolderNeedAnswerDto>> GetStakeHolderNeedsAnswerByStakeHolderGroupIdAsync(long stakeHolderGroupId, CancellationToken ct = default);
}

public class StakeHolderNeedsPriorityRepository : BaseRepository<StakeholderNeedsPriority>, IStakeHolderNeedsPriorityRepository
{
    public StakeHolderNeedsPriorityRepository(SemDbContext context) : base(context) { }

    private static IQueryable<StakeHolderNeedsPriorityDto> Project(IQueryable<StakeholderNeedsPriority> query) =>
        query.Select(needs => new StakeHolderNeedsPriorityDto
        {
            StakeHolderNeedsPriorityId = needs.Id,
            StakeHolderId = needs.StakeHolderId,
            NeedTitle = needs.NeedTitle,
            Importance = needs.Importance,
            Impact = needs.Impact,
            StakeHolderName = needs.StakeHolder.StakeHolderTitle
        });

    public async Task<PagedResult<StakeHolderNeedsPriorityDto>> FindAllStakeHolderNeedsPriorityPageAsync(long? stakeHolderId, string? filter, PageRequest pageable, CancellationToken ct = default)
    {
        // در JPQL مقایسه stk.id با null هیچ ردیفی برنمی‌گرداند
        if (stakeHolderId == null)
            return new PagedResult<StakeHolderNeedsPriorityDto> { Items = new List<StakeHolderNeedsPriorityDto>(), TotalElements = 0, Page = pageable.Page, Size = pageable.Size };

        var query = Project(Query.Where(n => n.StakeHolderId == stakeHolderId
                                             && (filter == null || n.NeedTitle.Contains(filter))));
        var total = await query.LongCountAsync(ct);
        var items = await global::Common.utils.PageUtils.Apply(query, pageable).ToListAsync(ct);
        return new PagedResult<StakeHolderNeedsPriorityDto> { Items = items, TotalElements = total, Page = pageable.Page, Size = pageable.Size };
    }

    public async Task<List<StakeHolderNeedsPriorityDto>> FindAllStakeHolderNeedsPriorityListAsync(long? stakeHolderId, string? filter, CancellationToken ct = default) =>
        await Project(Query.Where(n => (stakeHolderId == null || n.StakeHolderId == stakeHolderId)
                                       && (filter == null || n.NeedTitle.Contains(filter))))
            .ToListAsync(ct);

    public async Task<List<StakeHolderNeedAnswerDto>> GetStakeHolderNeedsAnswerByStakeHolderGroupIdAsync(long stakeHolderGroupId, CancellationToken ct = default)
    {
        var rows = await Query
            .Where(n => n.StakeHolder.StakeHolderGroupId == stakeHolderGroupId && n.AnsweringType != null)
            .Select(n => new
            {
                StakeHolderId = n.StakeHolderId,
                StakeHolderTitle = n.StakeHolder.StakeHolderTitle,
                NeedId = n.Id,
                NeedTitle = n.NeedTitle,
                n.AnsweringType,
                n.StakeHolder.BehaviorStrategy
            })
            .ToListAsync(ct);

        return rows
            .Select(r => new StakeHolderNeedAnswerDto(r.StakeHolderId, r.StakeHolderTitle, r.NeedId, r.NeedTitle, r.AnsweringType, r.BehaviorStrategy))
            .ToList();
    }
}
