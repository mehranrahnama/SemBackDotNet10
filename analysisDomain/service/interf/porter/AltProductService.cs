using SemBackDotNet10.analysisDomain.model.dto.porter;
using SemBackDotNet10.analysisDomain.model.entity.porter;

namespace SemBackDotNet10.analysisDomain.service.interf.porter;

public interface IAltProductService
{
    Task<AltProduct> FindByIdOrElseThrowAsync(long? id, CancellationToken ct = default);

    Task<AltProductDto> CreateNewAltProductAsync(AltProductDto altProductDto, CancellationToken ct = default);

    Task UpdateAltProductAsync(AltProductDto altProductDto, CancellationToken ct = default);

    Task DeleteAltProductByIdAsync(long? altProductId, CancellationToken ct = default);

    Task<List<AltProductDto>> GetAllAltProductByProductIdAsync(long? productId, CancellationToken ct = default);
}
