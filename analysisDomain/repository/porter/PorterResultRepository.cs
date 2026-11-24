using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.analysisDomain.model.dto.porter;
using SemBackDotNet10.analysisDomain.model.entity.porter;
using SemBackDotNet10.basicDomain.Repositories;
using SemBackDotNet10.Data;

namespace SemBackDotNet10.analysisDomain.repository.porter;

public interface IPorterResultRepository : IBaseRepository<PorterResult>
{
    Task<PorterResultDto?> GetAllByProductMarketAsync(long? productId, long? marketId, long? porterGroupId, CancellationToken ct = default);
}

public class PorterResultRepository : BaseRepository<PorterResult>, IPorterResultRepository
{
    public PorterResultRepository(SemDbContext context) : base(context) { }

    public Task<PorterResultDto?> GetAllByProductMarketAsync(long? productId, long? marketId, long? porterGroupId, CancellationToken ct = default) =>
        Query
            .Where(pr => productId != null && marketId != null && porterGroupId != null
                         && pr.ProductId == productId && pr.MarketId == marketId && pr.PorterGroupId == porterGroupId)
            .Select(pr => new PorterResultDto
            {
                Id = pr.Id,
                PlanId = pr.Market.PlanId,
                Result = pr.Result,
                Description = pr.Description
            })
            .FirstOrDefaultAsync(ct);
}
