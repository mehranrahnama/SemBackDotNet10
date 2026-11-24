using SemBackDotNet10.analysisDomain.model.dto.porter;
using SemBackDotNet10.analysisDomain.model.entity.porter;

namespace SemBackDotNet10.analysisDomain.service.interf.porter;

public interface IProductThreatService
{
    Task<ProductThreat> FindByIdOrElseThrowAsync(long? id, CancellationToken ct = default);

    Task CreateNewProductThreatAsync(List<ProductThreatDto> productThreatDto, CancellationToken ct = default);

    Task<List<ProductThreatDto>> GetAllByProductMarketAsync(string? body, CancellationToken ct = default);

    Task<List<ProductThreatDto>> EditProductThreatAsync(List<ProductThreatDto> productThreatDto, CancellationToken ct = default);
}
