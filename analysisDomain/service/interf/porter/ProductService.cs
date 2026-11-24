using SearchDto = global::SemBackDotNet10.Base.model.dto.SearchDto;
using SortDirection = global::SemBackDotNet10.Common.enums.SortDirection;
using SemBackDotNet10.analysisDomain.model.dto.porter;
using SemBackDotNet10.analysisDomain.model.entity.porter;
using global::Common.model.dto;
using SemBackDotNet10.Common.utils;

namespace SemBackDotNet10.analysisDomain.service.interf.porter;

public interface IProductService
{
    Task<Product> FindByIdOrElseThrowAsync(long? id, CancellationToken ct = default);

    Task<ProductDto> CreateNewProductAsync(ProductDto productDto, CancellationToken ct = default);

    Task UpdateProductAsync(ProductDto productDto, CancellationToken ct = default);

    Task DeleteProductByIdAsync(long productId, CancellationToken ct = default);

    Task<SearchDto> GetAllProductPageByProductGroupAsync(int page, int size, string? orderBy, SortDirection? direction, string? body, CancellationToken ct = default);

    Task<SearchDto> FindAllProductByPlanIdAsync(int page, int size, string? orderBy, SortDirection? direction, string? body, CancellationToken ct = default);
}
