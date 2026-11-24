using global::SemBackDotNet10.Common.Pagination;
using Common.utils;
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.analysisDomain.model.dto.porter;
using SemBackDotNet10.analysisDomain.model.entity.porter;

using SemBackDotNet10.Data;
using System.Linq.Dynamic.Core;

namespace SemBackDotNet10.analysisDomain.repository.porter;

public interface IProductRepository : IBaseRepository<Product>
{
    Task<global::SemBackDotNet10.Common.Pagination.PagedResult<ProductDto>> FindAllPageAsync(long productGroupId, string? filter, PageRequest pageRequest, CancellationToken ct = default);

    Task<global::SemBackDotNet10.Common.Pagination.PagedResult<ProductDto>> FindAllByPlanIdAsync(long planId, string? filter, PageRequest pageRequest, CancellationToken ct = default);

    Task<bool> ExistsByIdAsync(long id, CancellationToken ct = default);
}

public class ProductRepository : BaseRepository<Product>, IProductRepository
{
    public ProductRepository(SemDbContext context) : base(context) { }

    public async Task<global::SemBackDotNet10.Common.Pagination.PagedResult<ProductDto>> FindAllPageAsync(long productGroupId, string? filter, PageRequest pageRequest, CancellationToken ct = default)
    {
        var query = Query
            .Where(p => p.ProductGroupId == productGroupId && (filter == null || p.Title.Contains(filter)))
            .Select(p => new ProductDto(p.Id, p.Title, p.ProductGroup.Id, p.ProductGroup.Title));

        var total = await query.LongCountAsync(ct);
        var items = await query.Apply(pageRequest).ToListAsync(ct);
        return new global::SemBackDotNet10.Common.Pagination.PagedResult<ProductDto> { Items = items, TotalElements = total, Page = pageRequest.Page, Size = pageRequest.Size };
    }

    public async Task<global::SemBackDotNet10.Common.Pagination.PagedResult<ProductDto>> FindAllByPlanIdAsync(long planId, string? filter, PageRequest pageRequest, CancellationToken ct = default)
    {
        var query = Query
            .Where(p => p.ProductGroup.PlanId == planId && (filter == null || p.Title.Contains(filter)))
            .Select(p => new ProductDto(p.Id, p.Title, p.ProductGroup.Id, p.ProductGroup.Title));

        var total = await query.LongCountAsync(ct);
        var items = await query.Apply(pageRequest).ToListAsync(ct);
        return new global::SemBackDotNet10.Common.Pagination.PagedResult<ProductDto> { Items = items, TotalElements = total, Page = pageRequest.Page, Size = pageRequest.Size };
    }

    public Task<bool> ExistsByIdAsync(long id, CancellationToken ct = default) =>
        Query.AnyAsync(p => p.Id == id, ct);
}
