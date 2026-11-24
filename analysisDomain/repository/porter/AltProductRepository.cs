using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.analysisDomain.model.dto.porter;
using SemBackDotNet10.analysisDomain.model.entity.porter;
using SemBackDotNet10.basicDomain.Repositories;
using SemBackDotNet10.Data;

namespace SemBackDotNet10.analysisDomain.repository.porter;

public interface IAltProductRepository : IBaseRepository<AltProduct>
{
    Task<List<AltProductDto>> GetAllAltProductByProductIdAsync(long? productId, CancellationToken ct = default);
}

public class AltProductRepository : BaseRepository<AltProduct>, IAltProductRepository
{
    public AltProductRepository(SemDbContext context) : base(context) { }

    public Task<List<AltProductDto>> GetAllAltProductByProductIdAsync(long? productId, CancellationToken ct = default) =>
        Query
            .Where(ap => productId != null && ap.ProductId == productId)
            .Select(ap => new AltProductDto(ap.Id, ap.ProductId, ap.Product.Title, ap.Title))
            .ToListAsync(ct);
}
