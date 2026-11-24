using SearchDto = global::SemBackDotNet10.Base.model.dto.SearchDto;
using SortDirection = global::SemBackDotNet10.Common.enums.SortDirection;
using SemBackDotNet10.analysisDomain.model.dto.porter;
using SemBackDotNet10.analysisDomain.model.entity.porter;
using global::Common.model.dto;
using SemBackDotNet10.Common.utils;

namespace SemBackDotNet10.analysisDomain.service.interf.porter;

public interface IProductGroupService
{
    Task<ProductGroup> FindByIdOrElseThrowAsync(long? id, CancellationToken ct = default);

    Task<ProductGroupDto> CreateProductGroupAsync(ProductGroupDto productGroupDto, CancellationToken ct = default);

    Task<SearchDto> FindAllPageAsync(int page, int size, string? orderBy, SortDirection? direction, string? body, CancellationToken ct = default);

    Task<List<ProductGroupDto>> FindAllProductGroupsAsync(long? planId, CancellationToken ct = default);

    Task<ProductGroupDto> EditProductGroupAsync(ProductGroupDto productGroupDto, CancellationToken ct = default);

    Task DeleteProductGroupAsync(long? id, CancellationToken ct = default);

    Task<ProductGroup> MapToEntityAsync(ProductGroupDto productGroupDto, CancellationToken ct = default);

    ProductGroupDto MapToDto(ProductGroup productGroup);
}
