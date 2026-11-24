using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.analysisDomain.model.dto.porter;
using SemBackDotNet10.analysisDomain.model.entity.porter;
using SemBackDotNet10.basicDomain.Repositories;
using SemBackDotNet10.Data;

namespace SemBackDotNet10.analysisDomain.repository.porter;

public interface IProductThreatRepository : IBaseRepository<ProductThreat>
{
    Task<List<ProductThreatDto>> GetAllByProductMarketAsync(long? productId, long? marketId, long? porterGroupId, CancellationToken ct = default);

    Task<bool> ExistsByProductIdAsync(long productId, CancellationToken ct = default);
}

public class ProductThreatRepository : BaseRepository<ProductThreat>, IProductThreatRepository
{
    public ProductThreatRepository(SemDbContext context) : base(context) { }

    public Task<List<ProductThreatDto>> GetAllByProductMarketAsync(long? productId, long? marketId, long? porterGroupId, CancellationToken ct = default) =>
        Query
            .Where(pt => productId != null && pt.ProductId == productId
                         && marketId != null && pt.MarketId == marketId
                         && porterGroupId != null && pt.Porter.PorterGroupId == porterGroupId)
            .Select(pt => new ProductThreatDto(
                pt.Id,
                pt.Market.Id, pt.Market.Title, pt.Market.PlanId,
                pt.Product.Id, pt.Product.Title,
                pt.Product.ProductGroup.Id, pt.Product.ProductGroup.Title,
                pt.Porter.Id, pt.Porter.Title, pt.Porter.NoThreat, pt.Porter.MiddleThreat, pt.Porter.SeriousThreat,
                pt.Threat))
            .ToListAsync(ct);

    public Task<bool> ExistsByProductIdAsync(long productId, CancellationToken ct = default) =>
        Query.AnyAsync(pt => pt.ProductId == productId, ct);
}
